using System;
using PX.Data;
using PX.Objects.CA;

namespace Velixo.EBanking
{
    public sealed class PaymentMethodExt : PXCacheExtension<PX.Objects.CA.PaymentMethod>
    {
        [PXBool]
        [PXUIField(DisplayName = "Integrated Processing")]
        public bool IntegratedProcessing { get; set; }
        public abstract class integratedProcessing : PX.Data.BQL.BqlBool.Field<integratedProcessing> { }
    }
}
