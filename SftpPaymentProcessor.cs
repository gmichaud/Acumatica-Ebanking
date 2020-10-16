using System;
using System.Collections.Generic;
using System.Linq;
using PX.CCProcessingBase.Interfaces.V2;
using PgpCore;
using Renci.SshNet;
using System.IO;
using PX.Objects.PM;
using PX.SM;
using PX.Data;

namespace Velixo.EBanking
{
    internal class SftpPaymentProcessor : IAPPaymentProcessor
    {
        private string _host;
        private int _port;
        private string _username;
        private string _password;
        private string _sshIdentityCert;
        private string _pgpPublicKeyCert;
        private string _pgpPrivateKeyCert;
        private string _path;

        public SftpPaymentProcessor(IEnumerable<SettingsValue> settings)
        {
            _host = settings.Where(s => s.DetailID == SftpProcessingPlugin.SettingsKeys.Key.Host).SingleOrDefault()?.Value;
            _port = int.Parse(settings.Where(s => s.DetailID == SftpProcessingPlugin.SettingsKeys.Key.Port).SingleOrDefault()?.Value);
            _username = settings.Where(s => s.DetailID == SftpProcessingPlugin.SettingsKeys.Key.Username).SingleOrDefault()?.Value;
            _password = settings.Where(s => s.DetailID == SftpProcessingPlugin.SettingsKeys.Key.Password).SingleOrDefault()?.Value;
            _sshIdentityCert = settings.Where(s => s.DetailID == SftpProcessingPlugin.SettingsKeys.Key.SshIdentityCert).SingleOrDefault()?.Value;
            _pgpPublicKeyCert = settings.Where(s => s.DetailID == SftpProcessingPlugin.SettingsKeys.Key.PgpPublicKeyCert).SingleOrDefault()?.Value;
            _pgpPrivateKeyCert = settings.Where(s => s.DetailID == SftpProcessingPlugin.SettingsKeys.Key.PgpPrivateKeyCert).SingleOrDefault()?.Value;
            _path = settings.Where(s => s.DetailID == SftpProcessingPlugin.SettingsKeys.Key.Path).SingleOrDefault()?.Value;
        }

        public void UploadFile(string fileName, byte[] file)
        {
            var certGraph = PXGraph.CreateInstance<CertificateMaintenance>();

            using (var sourceStream = new MemoryStream(file))
            using(var targetStream = new MemoryStream())
            { 
                if(String.IsNullOrEmpty(_pgpPublicKeyCert) ^ String.IsNullOrEmpty(_pgpPrivateKeyCert))
                {
                    throw new Exception("When using PGP, you must specify both the public key and private key.");
                }
                else if (!String.IsNullOrEmpty(_pgpPublicKeyCert) && !String.IsNullOrEmpty(_pgpPrivateKeyCert))
                {
                    CetrificateFile pgpPublicKeyCert = certGraph.CertificateFile.Select(_pgpPublicKeyCert);
                    if (pgpPublicKeyCert == null || pgpPublicKeyCert.FileID == null) throw new Exception("SSH Certificate can't be found.");

                    CetrificateFile pgpPrivateKeyCert = certGraph.CertificateFile.Select(_pgpPrivateKeyCert);
                    if (pgpPrivateKeyCert == null || pgpPrivateKeyCert.FileID == null) throw new Exception("SSH Certificate can't be found.");

                    using (PGP pgp = new PGP())
                    {
                        pgp.EncryptStreamAndSign(sourceStream, targetStream, 
                            GetFile(pgpPublicKeyCert.FileID.Value), 
                            GetFile(pgpPrivateKeyCert.FileID.Value), pgpPrivateKeyCert.Password, 
                            true, true);
                    }
                }
                else
                {
                    //PGP Keys not configured, simply copy input file to target stream
                    sourceStream.CopyTo(targetStream);
                }

                AuthenticationMethod authenticationMethod;
                if (!String.IsNullOrEmpty(_sshIdentityCert))
                {
                    CetrificateFile sshCert = certGraph.CertificateFile.Select(_sshIdentityCert);
                    if (sshCert == null || sshCert.FileID == null) throw new Exception("SSH Certificate can't be found.");
                    using(var sshCertStream = GetFile(sshCert.FileID.Value))
                    { 
                        authenticationMethod = new PrivateKeyAuthenticationMethod(_username, new PrivateKeyFile(sshCertStream, sshCert.Password));
                    }
                }
                else if (!String.IsNullOrEmpty(_password))
                {
                    authenticationMethod = new PasswordAuthenticationMethod(_username, _password);
                }
                else
                {
                    throw new Exception("You must enter a password or specify a SSH private key for the SFTP connection.");
                }

                using (var client = new SftpClient(new ConnectionInfo(_host, _port, _username, authenticationMethod)))
                {
                    client.Connect();
                    client.WriteAllBytes(_path + (_path.EndsWith("/") ? "" : "/") + fileName, targetStream.ToArray());
                }
            }
        }

        private Stream GetFile(Guid fileID)
        {
            var fileAccessor = PXGraph.CreateInstance<UploadFileMaintenance>();
            var fileInfo = fileAccessor.GetFile(fileID);
            var ms = new MemoryStream(fileInfo.BinData);
            ms.Position = 0;
            return ms;
        }
    }
}
