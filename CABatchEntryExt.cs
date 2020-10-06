using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Compilation;
using PX.CCProcessingBase.Interfaces.V2;
using PX.Data;
using PX.Data.Update.ExchangeService;
using PX.Objects.CA;
using PX.Objects.CA.Repositories;
using PX.SM;

namespace Velixo.EBanking
{
    public class CABatchEntryPXExt : PXGraphExtension<CABatchEntry>
    {
        public PXAction<CABatch> Transfer;
        [PXButton]
        [PXUIField(DisplayName = "Transfer", MapEnableRights = PXCacheRights.Update,
            MapViewRights = PXCacheRights.Update,
            Visibility = PXUIVisibility.Visible)]
        protected IEnumerable transfer(PXAdapter adapter)
        {
            if (PXLongOperation.Exists(Base.UID))
            {
                throw new ApplicationException(PX.Objects.GL.Messages.PrevOperationNotCompleteYet);
            }

            Base.Actions.PressSave();
            CABatch doc = Base.Document.Current;
            CABatchExt docExt = PXCache<CABatch>.GetExtension<CABatchExt>(doc);

            if (doc.Released == true && doc.ExportTime.HasValue && !String.IsNullOrEmpty(docExt.UsrProcessingCenterID) && !docExt.UsrFileTransferTime.HasValue)
            {
                PXLongOperation.StartOperation(Base, delegate () { TransferDocProcessingCenter(Base, doc, docExt); });
            }
            return adapter.Get();
        }

        public static void TransferDocProcessingCenter(PXGraph graph, CABatch doc, CABatchExt docExt)
        {
            var pcGraph = PXGraph.CreateInstance<CCProcessingCenterMaint>();
            pcGraph.ProcessingCenter.Current = pcGraph.ProcessingCenter.Search<CCProcessingCenter.processingCenterID>(docExt.UsrProcessingCenterID);

            var processingCenter = pcGraph.ProcessingCenter.Current;

            if (processingCenter != null)
            {
                var providerIsDirectDeposit = true; //PXMultipleProviderTypeSelectorAttribute.IsProvider<CCProcessingCenter.processingTypeName, IDDPaymentProcessing>(pcGraph.ProcessingCenter.Cache, processingCenter);
                if (providerIsDirectDeposit)
                {
                    IAPPaymentProcessor processor = null;

                    try
                    {
                        var settings = GetSettings(graph, doc, docExt);
                        Type providerType = PXBuildManager.GetType(processingCenter.ProcessingTypeName, true);
                        var provider = (ICCProcessingPlugin)Activator.CreateInstance(providerType);
                        processor = provider.CreateProcessor<IAPPaymentProcessor>(settings);
                    }
                    catch (Exception ex)
                    {
                        throw new PXException(Messages.FailedToCreateDirectDepositProvider, ex.Message);
                    }
                    var fileNotes = PXNoteAttribute.GetFileNotes(graph.Views[graph.PrimaryView].Cache, doc);
                    if (fileNotes.Length == 1)
                    {
                        var fileNote = fileNotes[0];
                        UploadFileMaintenance upload = PXGraph.CreateInstance<UploadFileMaintenance>();
                        var file = upload.GetFile(fileNote);
                        
                        processor.UploadFile(file.FullName, file.BinData);
                        docExt.UsrFileTransferTime = DateTime.Now;
                        graph.Views[graph.PrimaryView].Cache.Update(doc);
                        graph.Actions.PressSave();
                    }
                    else
                    {
                        if (fileNotes.Length > 1)
                        {
                            throw new Exception(Messages.ACHTransferFailMoreThanOneAttachment);
                        }
                        else
                        {
                            throw new PXException(Messages.ACHTransferFailNoAttachment);
                        }
                    }
                }
                else
                {
                    throw new PXException(Messages.ACHTransferFailProviderIsNotForDirectDeposit);
                }
            }
            else
            {
                throw new PXException(Messages.ACHTransferFailNoProcessingCenterSelected);
            }
        }

        private static IEnumerable<SettingsValue> GetSettings(PXGraph graph, CABatch doc, CABatchExt docExt)
        {
            var detailRepository = new CCProcessingCenterDetailRepository(graph);

            List<SettingsValue> result = new List<SettingsValue>();
            foreach (CCProcessingCenterDetail setting in detailRepository.FindAllProcessingCenterDetails(docExt.UsrProcessingCenterID))
            {
                SettingsValue newSetting = new SettingsValue() { DetailID = setting.DetailID, Value = setting.Value };
                result.Add(newSetting);
            }
            return result;
        }

        public void CABatch_ProcessingCenterID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {
            var ret = string.Empty;
            foreach (PXResult<CCProcessingCenterPmntMethod,
                              CCProcessingCenter> proc in
                              PXSelectorAttribute.SelectAll<CABatchExt.usrProcessingCenterID>(sender, e.Row))
            {
                var pmtMethod = (CCProcessingCenterPmntMethod)proc;
                ret = pmtMethod.ProcessingCenterID;
                if (pmtMethod.IsDefault == true)
                    break;
            }

            e.NewValue = ret;
        }

        public void CABatch_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            CABatch row = e.Row as CABatch;
            if (row == null) return;
            CABatchExt rowExt = PXCache<CABatch>.GetExtension<CABatchExt>(row);

            bool isTransferable = row.Released == true &&
                                  row.ExportTime.HasValue &&
                                  !String.IsNullOrEmpty(rowExt.UsrProcessingCenterID) &&
                                  !rowExt.UsrFileTransferTime.HasValue;
            if (!(row.Released == true))
            {
                PXUIFieldAttribute.SetEnabled<CABatchExt.usrProcessingCenterID>(sender, row, true);
            }

            Transfer.SetEnabled(isTransferable);
        }
    }
}
