using System;
using System.Collections;
using System.Collections.Generic;
using PX.Data;
using PX.Objects;
using PX.Objects.CA;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.SM;

namespace Velixo.HsbcEBanking
{
    public class BankTransactionsSftpImport : PXGraph<BankTransactionsSftpImport>
    {
        private const string FilePrefix = "CD.BAI"; //Hardcoded for now, could be moved to CASetup if needed...

        [Serializable]
        [PXHidden]
        public class BankTransactionFile : IBqlTable
        {
            [PXBool]
            [PXUnboundDefault(false)]
            [PXUIFieldAttribute(DisplayName = "Selected")]
            public bool? Selected { get; set; }

            [PXString(255, IsKey = true)]
            [PXUIField(DisplayName = "File Name")]
            public string FileName { get; set; }

            [PXString(255, IsKey = true)]
            [PXUIField(DisplayName = "Full Name")]
            public string FullName { get; set; }
        }

        public PXSetup<CASetup> CASetup;
        public PXCancel<BankTransactionFile> Cancel;
        
        [PXFilterable]
        [PXVirtualDAC]
        public PXProcessing<BankTransactionFile> Files;

        public BankTransactionsSftpImport()
        {
            CASetup.Select();
            if(CASetup.Current.ImportToSingleAccount == true)
            {
                //When this is checked, the configuration of IStatementReader is done at the cash account level; separate SFTP would have to be configured in the cash account screen
                throw new PXException(Messages.ImportToSingleAccount);
            }

            var setupExt = PXCache<CASetup>.GetExtension<CASetupExt>(CASetup.Current);
            if(String.IsNullOrEmpty(setupExt.UsrStatementSftpAddress) || 
                String.IsNullOrEmpty(setupExt.UsrStatementSftpUserName) || 
                String.IsNullOrEmpty(setupExt.UsrStatementSftpPassword) || 
                String.IsNullOrEmpty(setupExt.UsrStatementSftpInboxPath))
            {
                throw new PXException(Messages.SftpNotConfigured);
            }

            Files.SetProcessDelegate((files) => ProcessFiles(files));
        }

        private static void ProcessFiles(List<BankTransactionFile> files)
        {
            bool hasErrors = false;

            var graph = PXGraph.CreateInstance<CABankTransactionsImport>();
            graph.CASetup.Select();
            var setupExt = PXCache<CASetup>.GetExtension<CASetupExt>(graph.CASetup.Current);

            using (var client = new Renci.SshNet.SftpClient(setupExt.UsrStatementSftpAddress, setupExt.UsrStatementSftpUserName, setupExt.UsrStatementSftpPassword))
            {
                client.Connect();
                for (int i = 0; i<files.Count; i++)
                {
                    var file = files[i];
                    try
                    {
                        var stream = new System.IO.MemoryStream();
                        client.DownloadFile(file.FullName, stream);
                        string nameWithExtension = file.FullName.EndsWith(".txt") ? file.FileName : file.FileName + ".txt";
                        var fileInfo = new FileInfo(Guid.NewGuid(), nameWithExtension, null, stream.ToArray());

                        graph.Clear();
                        graph.ImportStatement(fileInfo, false);

                        if(!String.IsNullOrEmpty(setupExt.UsrStatementSftpArchivePath))
                        {
                            var newPath = setupExt.UsrStatementSftpArchivePath + (setupExt.UsrStatementSftpArchivePath.EndsWith("/") ? "" : "/") + file.FileName;
                            client.RenameFile(file.FullName, newPath);
                        }
                        
                        PXProcessing.SetInfo(i, $"File {file.FileName} was processed. Size: {stream.Length} bytes");
                    }
                    catch(Exception ex)
                    {
                        PXProcessing.SetError(i, $"An error occured while processing file {file.FileName}: {ex.Message}");
                        hasErrors = true;
                    }
                }

                client.Disconnect();

                if(hasErrors)
                {
                    throw new PXException(PX.Objects.CA.Messages.ErrorsInProcessing);
                }
            }
        }

        protected virtual IEnumerable files()
        {
            bool found = false;
            foreach (var item in Files.Cache.Inserted)
            {
                found = true;
                yield return item;
            }

            if (found)
            { 
                yield break;
            }

            var setupExt = PXCache<CASetup>.GetExtension<CASetupExt>(CASetup.Current);
            using (var client = new Renci.SshNet.SftpClient(setupExt.UsrStatementSftpAddress, setupExt.UsrStatementSftpUserName, setupExt.UsrStatementSftpPassword))
            {
                client.Connect();
                foreach (var entry in client.ListDirectory(setupExt.UsrStatementSftpInboxPath))
                {
                    if(entry.Name.StartsWith(FilePrefix))
                    {
                        yield return Files.Insert(new BankTransactionFile { FileName = entry.Name, FullName = entry.FullName });
                    }
                }

                client.Disconnect();
            }

            Files.Cache.IsDirty = false;
        }
    }
}
