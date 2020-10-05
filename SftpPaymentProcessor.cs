using System;
using System.Collections.Generic;
using PX.CCProcessingBase.Interfaces.V2;


namespace Velixo.EBanking
{
    internal class SftpPaymentProcessor : IAPPaymentProcessor
    {
        public SftpPaymentProcessor(IEnumerable<SettingsValue> settingValues)
        {

        }

        public bool DoTransaction(string fileName, byte[] file, out string message)
        {
            throw new NotImplementedException();
        }
    }
}
