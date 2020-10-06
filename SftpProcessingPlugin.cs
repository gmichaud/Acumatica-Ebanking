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
        public static class SettingsKeys
        {
            public static class Key
            {
                //Not more Then 10 chars
                public const string Host = "HOST";
                public const string Port = "PORT";
                public const string Username = "USERNAME";
                public const string Password = "PASSWORD";
                public const string SshIdentityCert = "SSHCERT";
                public const string PgpPublicKeyCert = "PGPPUBLIC";
                public const string PgpPrivateKeyCert = "PGPPRIVATE";
                public const string Path = "PATH";
            }

            [PXLocalizable]
            public static class Descr
            {
                public const string Host = "Address of the SFTP Site";
                public const string Port = "Port used for connecting to the SFTP Site";
                public const string Username = "User Name";
                public const string Password = "Password";
                public const string SshIdentityCert = "SSH Private Key Certificate (optional)";
                public const string PgpPublicKeyCert = "PGP Encryption Public Key Certificate (optional)";
                public const string PgpPrivateKeyCert = "PGP Signature Private Key Certificate (optional)";
                public const string Path = "Folder Path (blank for root)";
            }
        }

        public IEnumerable<SettingsDetail> ExportSettings()
		{
            Dictionary<string, SettingsDetail> dictionary = new Dictionary<string, SettingsDetail>();
            dictionary[SettingsKeys.Key.Host] = new SettingsDetail
            {
                DetailID = SettingsKeys.Key.Host,
                Descr = PXLocalizer.Localize(SettingsKeys.Descr.Host),
                DefaultValue = "",
                IsEncryptionRequired = false
            };
            dictionary[SettingsKeys.Key.Port] = new SettingsDetail
            {
                DetailID = SettingsKeys.Key.Port,
                Descr = PXLocalizer.Localize(SettingsKeys.Descr.Port),
                DefaultValue = "22",
                IsEncryptionRequired = false
            };
            dictionary[SettingsKeys.Key.Username] = new SettingsDetail
            {
                DetailID = SettingsKeys.Key.Username,
                Descr = PXLocalizer.Localize(SettingsKeys.Descr.Username),
                DefaultValue = "",
                IsEncryptionRequired = false
            };
            dictionary[SettingsKeys.Key.Password] = new SettingsDetail
            {
                DetailID = SettingsKeys.Key.Password,
                Descr = PXLocalizer.Localize(SettingsKeys.Descr.Password),
                DefaultValue = "",
                IsEncryptionRequired = true
            };
            dictionary[SettingsKeys.Key.SshIdentityCert] = new SettingsDetail
            {
                DetailID = SettingsKeys.Key.SshIdentityCert,
                Descr = PXLocalizer.Localize(SettingsKeys.Descr.SshIdentityCert),
                DefaultValue = "",
                IsEncryptionRequired = false
            };
            dictionary[SettingsKeys.Key.PgpPublicKeyCert] = new SettingsDetail
            {
                DetailID = SettingsKeys.Key.PgpPublicKeyCert,
                Descr = PXLocalizer.Localize(SettingsKeys.Descr.PgpPublicKeyCert),
                DefaultValue = "",
                IsEncryptionRequired = false
            };
            dictionary[SettingsKeys.Key.PgpPrivateKeyCert] = new SettingsDetail
            {
                DetailID = SettingsKeys.Key.PgpPrivateKeyCert,
                Descr = PXLocalizer.Localize(SettingsKeys.Descr.PgpPrivateKeyCert),
                DefaultValue = "",
                IsEncryptionRequired = false
            };
            dictionary[SettingsKeys.Key.Path] = new SettingsDetail
            {
                DetailID = SettingsKeys.Key.Path,
                Descr = PXLocalizer.Localize(SettingsKeys.Descr.Path),
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
