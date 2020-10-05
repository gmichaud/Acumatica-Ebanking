using System;
using PX.Data;
using PX.Objects.CA;

namespace Velixo.EBanking
{
    public sealed class CASetupExt : PXCacheExtension<PX.Objects.CA.CASetup>
    {
        [PXDBString(255)]
        [PXUIField(DisplayName = "Address")]
        [PXUIEnabled(typeof(Where<CASetup.importToSingleAccount, NotEqual<True>>))]
        public string UsrStatementSftpAddress { get; set; }
        public abstract class usrStatementSftpAddress : PX.Data.BQL.BqlString.Field<usrStatementSftpAddress> { }

        [PXDBString(255)]
        [PXUIField(DisplayName = "User Name")]
        [PXUIEnabled(typeof(Where<CASetup.importToSingleAccount, NotEqual<True>>))]
        public string UsrStatementSftpUserName { get; set; }
        public abstract class usrStatementSftpUserName : PX.Data.BQL.BqlString.Field<usrStatementSftpUserName> { }

        [PXDBCryptString(255)]
        [PXUIField(DisplayName = "Password")]
        [PXUIEnabled(typeof(Where<CASetup.importToSingleAccount, NotEqual<True>>))]
        public string UsrStatementSftpPassword { get; set; }
        public abstract class usrStatementSftpPassword : PX.Data.BQL.BqlString.Field<usrStatementSftpPassword> { }

        [PXDBString(255)]
        [PXUIField(DisplayName = "Inbox Path")]
        [PXUIEnabled(typeof(Where<CASetup.importToSingleAccount, NotEqual<True>>))]
        public string UsrStatementSftpInboxPath { get; set; }
        public abstract class usrStatementSftpInboxPath : PX.Data.BQL.BqlString.Field<usrStatementSftpInboxPath> { }

        [PXDBString(255)]
        [PXUIField(DisplayName = "Archive Path")]
        [PXUIEnabled(typeof(Where<CASetup.importToSingleAccount, NotEqual<True>>))]
        public string UsrStatementSftpArchivePath { get; set; }
        public abstract class usrStatementSftpArchivePath : PX.Data.BQL.BqlString.Field<usrStatementSftpArchivePath> { }
    }
}
