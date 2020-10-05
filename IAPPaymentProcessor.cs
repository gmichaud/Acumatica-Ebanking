using PX.CCProcessingBase;
using PX.CCProcessingBase.Interfaces.V2;

namespace Velixo.EBanking
{
    public interface IAPPaymentProcessor
    {
        bool DoTransaction(string fileName, byte[] file, out string message);
    }
}
