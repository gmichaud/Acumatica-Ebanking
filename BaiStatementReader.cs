﻿using Velixo.EBanking.BaiParsing;
using PX.Data;
using PX.Objects.CA;
using PX.SM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Velixo.EBanking
{
    public class BaiStatementReader : IStatementReader
    {
        private const string OpeningLedgerTypeCode = "010";
        private const string ClosingLedgerTypeCode = "015";

        private TranslatedBaiFile _file;

        public bool AllowsMultipleAccounts()
        {
            return true;
        }
        
        public void ExportToNew<T>(FileInfo aFileInfo, T current, out List<CABankTranHeader> statements) where T : CABankTransactionsImport, new()
        {
            UploadFileMaintenance fileGraph = PXGraph.CreateInstance<UploadFileMaintenance>();
            statements = new List<CABankTranHeader>();
            T graph = null;
            if (current != null)
            {
                graph = current;
            }
            else
            {
                graph = PXGraph.CreateInstance<T>();
            }

            //Process file
            bool updateCurrent = graph.CASetup.Current.ImportToSingleAccount ?? false;
            bool allowEmptyFITID = graph.CASetup.Current.AllowEmptyFITID ?? false;

            foreach (Group group in _file.Groups)
            {
                foreach(Account account in group.Accounts)
                { 
                    ProcessAccount(graph, _file.FileCreationDateTime, group.AsOfDateTime, account, updateCurrent, allowEmptyFITID);
                    statements.Add(graph.Header.Current);
                }
            }

            //Attach uploaded file to statements that were created in Acumatica
            if (fileGraph.SaveFile(aFileInfo, FileExistsAction.CreateVersion))
            {
                if (aFileInfo.UID.HasValue)
                {
                    foreach (CABankTranHeader iStatement in statements)
                    {
                        if (Object.ReferenceEquals(graph.Header.Current, iStatement) == false)
                        {
                            graph.Header.Current = graph.Header.Search<CABankTranHeader.cashAccountID, CABankTranHeader.refNbr>(iStatement.CashAccountID, iStatement.RefNbr);
                        }
                        PXNoteAttribute.SetFileNotes(graph.Header.Cache, graph.Header.Current, aFileInfo.UID.Value);
                        graph.Save.Press();
                    }
                }
            }
        }

        private void ProcessAccount<T>(T graph, DateTime fileDate, DateTime asOfDate, Account account, bool updateCurrent, bool allowEmptyFITID = false)
            where T : CABankTransactionsImport
        {
            CashAccount acct = graph.cashAccountByExtRef.Select(account.CustomerAccountNumber);
            if (acct == null) throw new PXException(PX.Objects.CA.Messages.CashAccountWithExtRefNbrIsNotFoundInTheSystem, account.CustomerAccountNumber);

            if (graph.CASetup.Current.IgnoreCuryCheckOnImport != true && acct.CuryID != account.CurrencyCode)
                throw new PXException(PX.Objects.CA.Messages.CashAccountHasCurrencyDifferentFromOneInStatement, acct.CashAccountCD, acct.CuryID, account.CurrencyCode);

            CABankTranHeader header = null;
            if (updateCurrent == false || graph.Header.Current == null || graph.Header.Current.CashAccountID == null)
            {
                graph.Clear();
                header = new CABankTranHeader();
                header.CashAccountID = acct.CashAccountID;
                header = graph.Header.Insert(header);
                graph.Header.Current = header;
            }
            else
            {
                header = graph.Header.Current;
                if (header.CashAccountID.HasValue)
                {
                    if (header.CashAccountID != acct.CashAccountID)
                    {
                        throw new PXException(PX.Objects.CA.Messages.ImportedStatementIsMadeForAnotherAccount, acct.CashAccountCD, acct.Descr);
                    }
                }
            }
            
            header.BankStatementFormat = "BAI2";
            header.DocDate = fileDate.Date;
            header.StartBalanceDate = asOfDate.Date; //A BAI2 file covers a single day. 
            header.EndBalanceDate = asOfDate.Date;

            var openingLedgerFunds = account.FundsTypes.Where(ft => ft.TypeCode == OpeningLedgerTypeCode).FirstOrDefault();
            if (openingLedgerFunds != null)
            {
                header.CuryBegBalance = BaiFileHelpers.GetAmount(openingLedgerFunds.Amount, account.CurrencyCode);
            }

            var closingLedgerFunds = account.FundsTypes.Where(ft => ft.TypeCode == ClosingLedgerTypeCode).FirstOrDefault();
            if (closingLedgerFunds != null)
            {
                header.CuryEndBalance = BaiFileHelpers.GetAmount(closingLedgerFunds.Amount, account.CurrencyCode); 
            }

            header = graph.Header.Update(header);

            foreach (Detail statementDetail in account.Details)
            {
                CABankTran detail = new CABankTran();
                detail.CashAccountID = acct.CashAccountID;
                ProcessTransaction(detail, asOfDate, statementDetail);
                detail = graph.Details.Insert(detail);

                //Must be done after to avoid overwriting of debit by credit
                ProcessTransactionAfterInsert(detail, statementDetail, account.CurrencyCode);
                detail = graph.Details.Update(detail);
            }
            graph.Save.Press();
        }

        private static void ProcessTransaction(CABankTran detail, DateTime transactionDate, Detail statementDetail)
        {
            detail.TranCode = statementDetail.TypeCode;
            detail.TranDate = transactionDate.Date;
            detail.ExtTranID = (statementDetail.BankReferenceNumber == "NONREF") ? "" : RemoveLeadingZeros(statementDetail.BankReferenceNumber);
            detail.ExtRefNbr = (statementDetail.CustomerReferenceNumber == "NONREF") ? "" : RemoveLeadingZeros(statementDetail.CustomerReferenceNumber);
            detail.TranDesc = statementDetail.Text.Trim();
            if(detail.TranDesc.Length > 256)
            {
                detail.TranDesc = detail.TranDesc.Substring(0, 256);
            }
        }

        private static string RemoveLeadingZeros(string value)
        {
            //Reference numbers are zero padded. This will interfere with transaction matching in Acumatica.
            return value.TrimStart(new Char[] { '0' }); ;
        }

        private static void ProcessTransactionAfterInsert(CABankTran detail, Detail statementDetail, string accountCurrencyCode)
        {
            TransactionType tt = TransactionType.NotApplicable;
            int typeCode = int.Parse(statementDetail.TypeCode);
            
            //As per Appendix B - Transaction Type Codes of the HSBC BAI2 Statement Message Implementation Guide
            if(typeCode <= 399 || (typeCode >= 920 && typeCode <= 953))
            {
                tt = TransactionType.Credit;
            }
            else if((typeCode >= 400 && typeCode <= 699) || (typeCode >= 956))
            {
                tt = TransactionType.Debit;
            }

            switch (tt)
            {
                case TransactionType.Credit:
                    detail.CuryDebitAmt = BaiFileHelpers.GetAmount(statementDetail.Amount, accountCurrencyCode);
                    break;
                case TransactionType.Debit:
                    detail.CuryCreditAmt = BaiFileHelpers.GetAmount(statementDetail.Amount, accountCurrencyCode);
                    break;
                default:
                    throw new ApplicationException($"Unknown transaction type code: {statementDetail.TypeCode}; can't determine debit/credit");
            }
        }

        public bool IsValidInput(byte[] aInput)
        {
            try
            { 
                var parser = new BaiParser();
                var bai = parser.Parse(aInput);
                var translatedFile = BaiTranslator.Translate(bai);

                return translatedFile.NumberOfGroups > 0;
            }
            catch(Exception ex)
            {
                PXTrace.WriteError(ex);
                return false;
            }
        }

        public void Read(byte[] aInput)
        {
            var parser = new BaiParser();
            var bai = parser.Parse(aInput);
            _file = BaiTranslator.Translate(bai);
        }
    }
}
