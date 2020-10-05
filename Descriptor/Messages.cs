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

        public const string FailedToCreateDirectDepositProvider = "Failed to Create the Direct Deposit Provider. Please check that Plug-In(Type) is valid Type Name. {0}";
        public const string ACHTransferFailMoreThanOneAttachment = "More than one file is attached to the current document.";
        public const string ACHTransferFailNoAttachment = "No file is attached to the current document.";
        public const string ACHTransferFailProviderIsNotForDirectDeposit = "The processing center type is not compatible with Direct Deposit.";
        public const string ACHTransferFailNoProcessingCenterSelected = "No processing center has been selected.";
    }
}
