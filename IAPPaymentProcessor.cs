using PX.CCProcessingBase;
using PX.CCProcessingBase.Interfaces.V2;

namespace Velixo.EBanking
{
    public interface IAPPaymentProcessor
    {
        void UploadFile(string fileName, byte[] file);
    }
}
