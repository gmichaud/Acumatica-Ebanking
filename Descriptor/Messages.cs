using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Velixo.EBanking
{
    [PX.Common.PXLocalizable]
    public static class Messages
    {
        public const string XmlFailedValidation = "The XML file failed validation: {0}";
        public const string XmlValidationWarning = "XML Validation Warning: {0}";
        public const string DisableXmlSchemaValidation = "Disable XML Schema Validation (debug mode)";
        public const string ImportToSingleAccount = "This import process is designed to work with the 'Import to Single Account' checkbox unchecked in Cash Management Preferences.";
        public const string SftpNotConfigured = "Please configure SFTP settings from Cash Management Preferences.";
        public const string PaymentMethodAlreadyContainsRemittanceSettings = "This payment method already contains remittance settings. Aborting setup.";
    }
}
