using System;
using System.Collections.Generic;
using PX.CCProcessingBase;
using PX.CCProcessingBase.Attributes;
using PX.CCProcessingBase.Interfaces.V2;
using PX.Common;
using PX.Data;

namespace Velixo.EBanking
{
	[PXDisplayTypeName("SFTP Upload plug-in")]
	public class SftpProcessingPlugin : ICCProcessingPlugin
	{
        private static class SettingsKeys
        {
            //Not more Then 10 chars
            public static class Key
            {
                public const string URLConnection = "URLCONNECT";
                public const string Port = "PORT";
                public const string LoginID = "LOGINID";
                public const string TranKey = "TRANKEY";
                public const string BankName = "BANKNAME";
                public const string Path = "PATH";
            }

            public static class Default
            {
                public const string URLConnection = "";
                public const string Port = "";
                public const string LoginID = "";
                public const string TranKey = "";
                public const string BankName = "";
                public const string Path = "";
            }

            [PXLocalizable]
            public static class Descr
            {
                public const string URLConnection = "URL for connecting to the SFTP Site";
                public const string Port = "(Optional) Port used for connecting to the SFTP Site";
                public const string LoginID = "Your Login";
                public const string TranKey = "Your Password";
                public const string BankName = "Bank Name";
                public const string Path = "Folder Path (blank for root)";
            }
        }

        public IEnumerable<SettingsDetail> ExportSettings()
		{
            Dictionary<string, SettingsDetail> dictionary = new Dictionary<string, SettingsDetail>();
            dictionary[SettingsKeys.Key.BankName] = new SettingsDetail
            {
                DetailID = SettingsKeys.Key.BankName,
                Descr = PXLocalizer.Localize(SettingsKeys.Descr.BankName),
                DefaultValue = "",
                IsEncryptionRequired = false
            };

            return dictionary.Values;
        }

        public string ValidateSettings(SettingsValue setting)
		{
            return String.Empty; //All good!
		}

		public void TestCredentials(IEnumerable<SettingsValue> settingValues)
		{
			throw new NotImplementedException();
		}

		public T CreateProcessor<T>(IEnumerable<SettingsValue> settingValues) where T : class
		{
			if(typeof(T) == typeof(IAPPaymentProcessor))
            {
                return new SftpPaymentProcessor(settingValues) as T;
            }
            else
            {
                return default(T);
            }
		}
	}
}
