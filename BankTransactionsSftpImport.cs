using System;
using System.Collections;
using System.Collections.Generic;
using PX.Data;
using PX.Objects;
using PX.Objects.CA;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.IN;

namespace NexVue.HsbcEBanking
{
    public class BankTransactionsSftpImport : PXGraph<BankTransactionsSftpImport>
    {
        [Serializable]
        public class ImportFilter : IBqlTable
        {
            [CashAccount(null, typeof(Search<CashAccount.cashAccountID, Where<Match<Current<AccessInfo.userName>>>>))]
            public int? CashAccountID { get; set; }
        }

        [Serializable]
        public class BankTransactionFile : IBqlTable
        {
            [PXBool]
            [PXDefault(false)]
            [PXUIFieldAttribute(DisplayName = "Selected")]
            public bool? Selected { get; set; }

            [PXString(255, IsKey = true)]
            [PXUIField(DisplayName = "File Name")]
            public string FileName { get; set; }
        }

        public PXCancel<ImportFilter> Cancel;
        public PXFilter<ImportFilter> Filter;

        [PXFilterable]
        [PXVirtualDAC]
        public PXFilteredProcessing<BankTransactionFile, ImportFilter> Files;

        protected virtual void ImportFilter_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            var filterRow = e.Row as ImportFilter;
            Files.SetProcessDelegate<CABankTransactionsImport>((graph, file) => ProcessFile(graph, filterRow.CashAccountID, file));
        }

        protected virtual void ImportFilter_CashAccountID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            Files.Cache.Clear();
        }

        private static void ProcessFile(CABankTransactionsImport graph, int? cashAccountID, BankTransactionFile file)
        {
            throw new PXException("File " + file.FileName + " failed to process");
        }

        protected virtual IEnumerable files()
        {
            bool found = false;
            foreach (var item in Files.Cache.Inserted)
            {
                found = true;
                yield return item;
            }

            if (found || Filter.Current.CashAccountID == null)
            { 
                yield break;
            }

            using (var client = new Renci.SshNet.SftpClient("filegateway.us.hsbc.com", "HFG01989", "password"))
            {
                client.Connect();
                foreach (var entry in client.ListDirectory("/Inbox"))
                {
                    if(entry.Name.StartsWith("CD.BAI"))
                    {
                        yield return Files.Insert(new BankTransactionFile { FileName = entry.Name });
                    }
                }

                client.Disconnect();
            }

            Files.Cache.IsDirty = false;
        }
    }
}
