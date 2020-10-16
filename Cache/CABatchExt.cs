using System;
using PX.Data;
using PX.Objects.CA;

namespace Velixo.EBanking
{
    public sealed class CABatchExt : PXCacheExtension<PX.Objects.CA.CABatch>
    {
        [PXDBString(10, IsUnicode = true)]
        [PXSelector(typeof(Search2<CCProcessingCenterPmntMethod.processingCenterID,
            InnerJoin<CCProcessingCenter,
                On<CCProcessingCenterPmntMethod.processingCenterID,
                    Equal<CCProcessingCenter.processingCenterID>>>,
            Where<CCProcessingCenterPmntMethod.isActive, Equal<True>,
                And<CCProcessingCenterPmntMethod.paymentMethodID, Equal<Current<CABatch.paymentMethodID>>,
                And<Where<CCProcessingCenterPmntMethod.isDefault, Equal<True>, 
                    Or<CCProcessingCenter.cashAccountID, Equal<Current<CABatch.cashAccountID>>>>>>>>))]
        [PXUIField(DisplayName = "Proc. Center ID")]
        [PXFormula(typeof(Default<CABatch.paymentMethodID>))]
        public string UsrProcessingCenterID { get; set; }
        public abstract class usrProcessingCenterID : PX.Data.BQL.BqlString.Field<usrProcessingCenterID> { }

        [PXDBDate(PreserveTime = true)]
        [PXUIField(DisplayName = "File Transfer Time", Enabled = false)]
        public DateTime? UsrFileTransferTime { get; set; }
        public abstract class usrFileTransferTime : PX.Data.BQL.BqlDateTime.Field<usrFileTransferTime> { }
    }
}
