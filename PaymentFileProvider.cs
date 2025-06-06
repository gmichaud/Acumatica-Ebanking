﻿using PX.Api;
using PX.Data;
using PX.DataSync;
using PX.Objects.AP;
using PX.SM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace Velixo.EBanking
{
    public class PaymentFileProvider : PXSYBaseEncodedFileProvider, IPXSYProvider
    {
        private const string IncomingBatchNumber = "BatchNbr";
        private const string DisableXmlSchemaValidation = "DisableXmlSchemaValidation";

        //HSBC fields
        protected const string MsgId = "MsgId";
        protected const string CreDtTm = "CreDtTm";
        protected const string AuthstnCd = "AuthstnCd";
        protected const string InitgPtyId = "InitgPtyId";
        protected const string PmtInfId = "PmtInfId";
        protected const string PmtMtd = "PmtMtd";
        protected const string PmtTpInfSvcLvlCd = "PmtTpInfSvcLvlCd";
        protected const string PmtTpInfLclInstrmCd = "PmtTpInfLclInstrmCd";
        protected const string PmtTpInfLclInstrmPrty = "PmtTpInfLclInstrmPrty";
        protected const string ReqdExctnDt = "ReqdExctnDt";
        protected const string CtrlSum = "CtrlSum";
        protected const string DbtrNm = "DbtrNm";
        protected const string DbtrStrtNm = "DbtrStrtNm";
        protected const string DbtrBldgNb = "DbtrBldgNb";
        protected const string DbtrPstCd = "DbtrPstCd";
        protected const string DbtrTwnNm = "DbtrTwnNm";
        protected const string DbtrCtrySubDvsn = "DbtrCtrySubDvsn";
        protected const string DbtrCtry = "DbtrCtry";
        protected const string DbtrOrgId = "DbtrOrgId";
        protected const string DbtrOrgIdSchemeNm = "DbtrOrgIdSchemeNm";
        protected const string DbtrAcctId = "DbtrAcctId";
        protected const string DbtrAcctIbanId = "DbtrAcctIbanId";
        protected const string DbtrAcctCcy = "DbtrAcctCcy";
        protected const string DbtrFinInstnBic = "DbtrFinInstnBic";
        protected const string DbtrFinInstnClrSysMmbId = "DbtrFinInstnClrSysMmbId";
        protected const string DbtrFinInstnStrtNm = "DbtrFinInstnStrtNm";
        protected const string DbtrFinInstnBldgNb = "DbtrFinInstnBldgNb";
        protected const string DbtrFinInstnPstCd = "DbtrFinInstnPstCd";
        protected const string DbtrFinInstnTwnNm = "DbtrFinInstnTwnNm";
        protected const string DbtrFinInstnCtrySubDvsn = "DbtrFinInstnCtrySubDvsn";
        protected const string DbtrFinInstnCtry = "DbtrFinInstnCtry";
        protected const string CdtrIntrmyAgt1Bic = "CdtrIntrmyAgt1Bic";
        protected const string CdtrIntrmyAgt1BicCtry = "CdtrIntrmyAgt1BicCtry";
        protected const string ChrgBr = "ChrgBr";
        protected const string CdtTrfTxInfPmtInstrId = "CdtTrfTxInfPmtInstrId";
        protected const string CdtTrfTxInfPmtEndToEndId = "CdtTrfTxInfPmtEndToEndId";
        protected const string CdtTrfTxInfAmtCcy = "CdtTrfTxInfAmtCcy";
        protected const string CdtTrfTxInfAmt = "CdtTrfTxInfAmt";
        protected const string ChqInstrChqNb = "ChqInstrChqNb";
        protected const string ChqInstrDlvryMtd = "ChqInstrDlvryMtd";
        protected const string ChqInstrDlvryMtdPrtry = "ChqInstrDlvryMtdPrtry";
        protected const string ChqInstrDlvryToNm = "ChqInstrDlvryToNm";
        protected const string ChqInstrDlvryStrtNm = "ChqInstrDlvryStrtNm";
        protected const string ChqInstrDlvryBldgNb = "ChqInstrDlvryBldgNb";
        protected const string ChqInstrDlvryAdrLine1 = "ChqInstrDlvryAdrLine1";
        protected const string ChqInstrDlvryAdrLine2 = "ChqInstrDlvryAdrLine2";
        protected const string ChqInstrDlvryPstCd = "ChqInstrDlvryPstCd";
        protected const string ChqInstrDlvryTwnNm = "ChqInstrDlvryTwnNm"; 
        protected const string ChqInstrDlvryCtrySubDvsn = "ChqInstrDlvryCtrySubDvsn";
        protected const string ChqInstrDlvryCtry = "ChqInstrDlvryCtry";
        protected const string ChqInstrFrmsCd = "ChqInstrFrmsCd";
        protected const string ChqInstrMemoFld = "ChqInstrMemoFld"; 
        protected const string CdtrFinInstnBic = "CdtrFinInstnBic";
        protected const string CdtrFinInstnClrSysMmbId = "CdtrFinInstnClrSysMmbId";
        protected const string CdtrFinInstnStrtNm = "CdtrFinInstnStrtNm";
        protected const string CdtrFinInstnBldgNb = "CdtrFinInstnBldgNb";
        protected const string CdtrFinInstnPstCd = "CdtrFinInstnPstCd";
        protected const string CdtrFinInstnTwnNm = "CdtrFinInstnTwnNm";
        protected const string CdtrFinInstnCtrySubDvsn = "CdtrFinInstnCtrySubDvsn";
        protected const string CdtrFinInstnCtry = "CdtrFinInstnCtry";
        protected const string CdtrNm = "CdtrNm";
        protected const string CdtrStrtNm = "CdtrStrtNm";
        protected const string CdtrBldgNb = "CdtrBldgNb";
        protected const string CdtrAdrLine1 = "CdtrAdrLine1";
        protected const string CdtrAdrLine2 = "CdtrAdrLine2";
        protected const string CdtrPstCd = "CdtrPstCd";
        protected const string CdtrTwnNm = "CdtrTwnNm";
        protected const string CdtrCtrySubDvsn = "CdtrCtrySubDvsn";
        protected const string CdtrCtry = "CdtrCtry";
        protected const string CdtrOrgId = "CdtrOrgId";
        protected const string CdtrOrgSchmeNmCd = "CdtrOrgSchmeNmCd";
        protected const string CdtrAcctId = "CdtrAcctId";
        protected const string CdtrAcctIbanId = "CdtrAcctIbanId";
        protected const string CdtrAcctTp = "CdtrAcctTp";

        protected const string RmtInfEmail = "RmtInfEmail";
        protected const string RmtInfPstlAdrNm = "RmtInfPstlAdrNm";
        protected const string RmtInfPstlAdrCtry = "RmtInfPstlAdrCtry";
        protected const string RmtInfUstrd = "RmtInfUstrd";
        protected const string RmtInfAPRefNbr = "RmtInfAPRefNbr";
        protected const string RmtInfRefInfoPrtry = "RmtInfRefInfoPrtry";
        protected const string RmtInfRefInfoCd = "RmtInfRefInfoCd";


        protected const string NoteID = "NoteID";
        protected const string FileID = "FileName";

        protected List<String> _notes = new List<String>();
        private Dictionary<string, string> _cache;

        public string DefaultFileExtension => ".xml";

        public override string ProviderName => "Payment File Export Provider";

        public override string Extensiton => DefaultFileExtension;

        public override PXFieldState[] GetSchemaFields(string objectName)
        {
            List<PXFieldState> ret = new List<PXFieldState>(base.GetSchemaFields(objectName));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, NoteID, typeof(Int64))));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, FileID)));
            
            //ACH/Wire fields
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, MsgId)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, CreDtTm)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, AuthstnCd)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, InitgPtyId)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, PmtInfId)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, PmtMtd)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, PmtTpInfSvcLvlCd)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, PmtTpInfLclInstrmCd)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, PmtTpInfLclInstrmPrty)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, ReqdExctnDt)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, CtrlSum)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, DbtrNm)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, DbtrStrtNm)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, DbtrBldgNb)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, DbtrPstCd)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, DbtrTwnNm)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, DbtrCtrySubDvsn)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, DbtrCtry)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, DbtrOrgId)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, DbtrOrgIdSchemeNm)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, DbtrAcctId)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, DbtrAcctIbanId)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, DbtrAcctCcy)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, DbtrFinInstnBic)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, DbtrFinInstnClrSysMmbId)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, DbtrFinInstnStrtNm)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, DbtrFinInstnBldgNb)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, DbtrFinInstnPstCd)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, DbtrFinInstnTwnNm)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, DbtrFinInstnCtrySubDvsn)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, DbtrFinInstnCtry)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, ChrgBr)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, CdtrIntrmyAgt1Bic)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, CdtrIntrmyAgt1BicCtry)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, CdtTrfTxInfPmtInstrId)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, CdtTrfTxInfPmtEndToEndId)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, CdtTrfTxInfAmtCcy)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, CdtTrfTxInfAmt)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, ChqInstrChqNb)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, ChqInstrDlvryMtd)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, ChqInstrDlvryMtdPrtry)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, ChqInstrDlvryToNm)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, ChqInstrDlvryStrtNm)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, ChqInstrDlvryBldgNb)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, ChqInstrDlvryAdrLine1)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, ChqInstrDlvryAdrLine2)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, ChqInstrDlvryPstCd)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, ChqInstrDlvryTwnNm)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, ChqInstrDlvryCtrySubDvsn)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, ChqInstrDlvryCtry)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, ChqInstrFrmsCd)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, ChqInstrMemoFld)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, CdtrFinInstnBic)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, CdtrFinInstnClrSysMmbId)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, CdtrFinInstnStrtNm)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, CdtrFinInstnBldgNb)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, CdtrFinInstnPstCd)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, CdtrFinInstnTwnNm)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, CdtrFinInstnCtrySubDvsn)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, CdtrFinInstnCtry)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, CdtrNm)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, CdtrStrtNm)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, CdtrBldgNb)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, CdtrAdrLine1)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, CdtrAdrLine2)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, CdtrPstCd)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, CdtrTwnNm)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, CdtrCtrySubDvsn)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, CdtrCtry)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, CdtrOrgId)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, CdtrOrgSchmeNmCd)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, CdtrAcctId)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, CdtrAcctIbanId)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, CdtrAcctTp)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, RmtInfEmail)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, RmtInfPstlAdrNm)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, RmtInfPstlAdrCtry)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, RmtInfUstrd)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, RmtInfAPRefNbr)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, RmtInfRefInfoCd)));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, RmtInfRefInfoPrtry)));

            return ret.ToArray();
        }

        protected override List<PXStringState> FillParameters()
        {
            List<PXStringState> ret = base.FillParameters();
            ret.Add(CreateParameter(IncomingBatchNumber, Titles.IncomingBatchNumber, String.Empty));
            ret.Add(CreateParameter(DisableXmlSchemaValidation, Messages.DisableXmlSchemaValidation, String.Empty));

            return ret;
        }
        
		protected override void InitialiseFile(int? revision)
		{
			base.InitialiseFile(null);
		}

		protected override void InitialiseFile()
		{
			InitialiseFile(true);
		}

		protected override void InitialiseFile(Boolean create)
		{
			InitialiseFile(GetParameter(FILE_PARAM), true, null);
		}
        
		protected override void InitialiseFile(String fileName, Boolean create, Int32? revision)
		{
			base.InitialiseFile(GetParameter(FILE_PARAM), true, null);
		}

        public override void Export(string objectName, PXSYTable table, bool breakOnError, Action<SyProviderRowResult> callback)
        {
            if (table.Count > 0 || table.Columns.Contains(FileID))
            {
                Int32 fileIndex = table.IndexOfColumn(FileID);
                string file = table[0][fileIndex];

                if (!String.IsNullOrEmpty(file))
                {
                    SetParameters(_Parameters.Where(r => r.Name != FILE_PARAM).Concat(new PXSYParameter[] { new PXSYParameter(FILE_PARAM, file) }).ToArray());
                }
            }

            base.Export(objectName, table, breakOnError, callback);
        }

        protected override byte[] InternalExport(string objectName, PXSYTable table)
        {
            _notes.Clear();
            Int32 noteIndex = table.Columns.IndexOf(NoteID);
            if (noteIndex >= 0) _notes.AddRange(table.Select(r => r[noteIndex]).Distinct());

            Encoding encoding = GetEncoding();

            XmlDocument doc = new XmlDocument();
            var declarationNode = doc.CreateXmlDeclaration("1.0", encoding.WebName.ToUpper(), null);
            doc.AppendChild(declarationNode);

            using (XmlWriter writer = doc.CreateNavigator().AppendChild())
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Document", "urn:iso:std:iso:20022:tech:xsd:pain.001.001.03");
                writer.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
                writer.WriteStartElement("CstmrCdtTrfInitn");
                
                //GrpHdr
                writer.WriteStartElement("GrpHdr");
                    writer.WriteElementString(MsgId, table.Rows[0][table.Columns.IndexOf(MsgId)]);
                    writer.WriteElementString(CreDtTm, table.Rows[0][table.Columns.IndexOf(CreDtTm)]);
                    if(!String.IsNullOrEmpty(table.Rows[0][table.Columns.IndexOf(AuthstnCd)]))
                    { 
                        writer.WriteStartElement("Authstn");
                            writer.WriteElementString("Cd", table.Rows[0][table.Columns.IndexOf(AuthstnCd)]);
                        writer.WriteEndElement(); //Authstn
                    }
                    writer.WriteElementString("NbOfTxs", table.Rows.Count.ToString());
                    writer.WriteElementString(CtrlSum, table.Rows[0][table.Columns.IndexOf(CtrlSum)]);
                    writer.WriteStartElement("InitgPty");
                        writer.WriteStartElement("Id");
                            writer.WriteStartElement("OrgId");
                                writer.WriteStartElement("Othr");
                                    writer.WriteElementString("Id", table.Rows[0][table.Columns.IndexOf(InitgPtyId)]);
                                writer.WriteEndElement(); //Othr
                            writer.WriteEndElement(); //OrgId
                         writer.WriteEndElement(); //Id
                    writer.WriteEndElement(); //InitgPty
                writer.WriteEndElement(); //GrpHdr

                
                writer.WriteStartElement("PmtInf");
                    writer.WriteElementString(PmtInfId, table.Rows[0][table.Columns.IndexOf(PmtInfId)]);
                    writer.WriteElementString(PmtMtd, table.Rows[0][table.Columns.IndexOf(PmtMtd)]);
                    writer.WriteElementString("NbOfTxs", table.Rows.Count.ToString());
                    writer.WriteElementString(CtrlSum, table.Rows[0][table.Columns.IndexOf(CtrlSum)]);

                    //Payment Type Information 
                    if(!String.IsNullOrEmpty(table.Rows[0][table.Columns.IndexOf(PmtTpInfSvcLvlCd)]) || 
                        !String.IsNullOrEmpty(table.Rows[0][table.Columns.IndexOf(PmtTpInfLclInstrmCd)]) || 
                        !String.IsNullOrEmpty(table.Rows[0][table.Columns.IndexOf(PmtTpInfLclInstrmPrty)]))
                    {
                        writer.WriteStartElement("PmtTpInf");
                        if(!String.IsNullOrEmpty(table.Rows[0][table.Columns.IndexOf(PmtTpInfSvcLvlCd)]))
                        {
                            writer.WriteStartElement("SvcLvl");
                                writer.WriteElementString("Cd", table.Rows[0][table.Columns.IndexOf(PmtTpInfSvcLvlCd)]);
                            writer.WriteEndElement(); //SvcLvl
                        }
                        if(!String.IsNullOrEmpty(table.Rows[0][table.Columns.IndexOf(PmtTpInfLclInstrmCd)]))
                        {
                            writer.WriteStartElement("LclInstrm");
                                writer.WriteElementString("Cd", table.Rows[0][table.Columns.IndexOf(PmtTpInfLclInstrmCd)]);
                            writer.WriteEndElement(); //LclInstrm
                        }
                        if(!String.IsNullOrEmpty(table.Rows[0][table.Columns.IndexOf(PmtTpInfLclInstrmPrty)]))
                        {
                            writer.WriteStartElement("LclInstrm");
                                writer.WriteElementString("Prtry", table.Rows[0][table.Columns.IndexOf(PmtTpInfLclInstrmPrty)]);
                            writer.WriteEndElement(); //LclInstrm
                        }
                        writer.WriteEndElement(); //PmtTpInf
                    }
                writer.WriteElementString("ReqdExctnDt", table.Rows[0][table.Columns.IndexOf(ReqdExctnDt)]);

                //Debitor
                writer.WriteStartElement("Dbtr");
                    writer.WriteElementString("Nm", table.Rows[0][table.Columns.IndexOf(DbtrNm)]);
                    writer.WriteStartElement("PstlAdr");
                        writer.WriteElementString("StrtNm", table.Rows[0][table.Columns.IndexOf(DbtrStrtNm)]);
                        writer.WriteElementString("BldgNb", table.Rows[0][table.Columns.IndexOf(DbtrBldgNb)]);
                        writer.WriteElementString("PstCd", table.Rows[0][table.Columns.IndexOf(DbtrPstCd)]);
                        writer.WriteElementString("TwnNm", table.Rows[0][table.Columns.IndexOf(DbtrTwnNm)]);
                        writer.WriteElementString("CtrySubDvsn", table.Rows[0][table.Columns.IndexOf(DbtrCtrySubDvsn)]);
                        writer.WriteElementString("Ctry", table.Rows[0][table.Columns.IndexOf(DbtrCtry)]);
                    writer.WriteEndElement(); //PstlAdr
                    if(!String.IsNullOrEmpty(table.Rows[0][table.Columns.IndexOf(DbtrOrgId)]))
                    { 
                        writer.WriteStartElement("Id");
                            writer.WriteStartElement("OrgId");
                                writer.WriteStartElement("Othr");
                                    writer.WriteElementString("Id", table.Rows[0][table.Columns.IndexOf(DbtrOrgId)]);
                                    if(!String.IsNullOrEmpty(table.Rows[0][table.Columns.IndexOf(DbtrOrgIdSchemeNm)]))
                                    {
                                        writer.WriteStartElement("SchmeNm");
                                            writer.WriteElementString("Prtry", table.Rows[0][table.Columns.IndexOf(DbtrOrgIdSchemeNm)]);
                                        writer.WriteEndElement(); //SchmeNm
                                    }
                                writer.WriteEndElement(); //Othr
                            writer.WriteEndElement(); //OrgId
                        writer.WriteEndElement(); //Id
                    }
                writer.WriteEndElement(); //Dbtr

                //Debitor Account
                writer.WriteStartElement("DbtrAcct");
                    writer.WriteStartElement("Id");
                        writer.WriteElementStringIfNotNull("IBAN", table.Rows[0][table.Columns.IndexOf(DbtrAcctIbanId)]);
                        if (!String.IsNullOrEmpty(table.Rows[0][table.Columns.IndexOf(DbtrAcctId)]))
                        {
                            writer.WriteStartElement("Othr");
                                writer.WriteElementString("Id", table.Rows[0][table.Columns.IndexOf(DbtrAcctId)]);
                            writer.WriteEndElement(); //Othr
                        }
                    writer.WriteEndElement(); //Id
                    writer.WriteElementStringIfNotNull("Ccy", table.Rows[0][table.Columns.IndexOf(DbtrAcctCcy)]);
                writer.WriteEndElement(); //DbtrAcct

                //Debitor Agent
                writer.WriteStartElement("DbtrAgt");
                    writer.WriteStartElement("FinInstnId");
                    writer.WriteElementStringIfNotNull("BIC", table.Rows[0][table.Columns.IndexOf(DbtrFinInstnBic)]);

                    if (!String.IsNullOrEmpty(table.Rows[0][table.Columns.IndexOf(DbtrFinInstnClrSysMmbId)]))
                    { 
                        writer.WriteStartElement("ClrSysMmbId");
                            writer.WriteElementString("MmbId", table.Rows[0][table.Columns.IndexOf(DbtrFinInstnClrSysMmbId)]);
                        writer.WriteEndElement(); //ClrSysMmbId
                    }

                    writer.WriteStartElement("PstlAdr");
                        writer.WriteElementStringIfNotNull("StrtNm", table.Rows[0][table.Columns.IndexOf(DbtrFinInstnStrtNm)]);
                        writer.WriteElementStringIfNotNull("BldgNb", table.Rows[0][table.Columns.IndexOf(DbtrFinInstnBldgNb)]);
                        writer.WriteElementStringIfNotNull("PstCd", table.Rows[0][table.Columns.IndexOf(DbtrFinInstnPstCd)]);
                        writer.WriteElementStringIfNotNull("TwnNm", table.Rows[0][table.Columns.IndexOf(DbtrFinInstnTwnNm)]);
                        writer.WriteElementStringIfNotNull("CtrySubDvsn", table.Rows[0][table.Columns.IndexOf(DbtrFinInstnCtrySubDvsn)]);
                        writer.WriteElementStringIfNotNull("Ctry", table.Rows[0][table.Columns.IndexOf(DbtrFinInstnCtry)]);
                    writer.WriteEndElement(); //PstlAdr
                    writer.WriteEndElement(); //FinInstnId
                writer.WriteEndElement(); //DbtrAgt
                    
                writer.WriteElementStringIfNotNull("ChrgBr", table.Rows[0][table.Columns.IndexOf(ChrgBr)]);

                foreach (PXSYRow row in table)
                {
                    //Credit Transaction Info
                    writer.WriteStartElement("CdtTrfTxInf");
                        writer.WriteStartElement("PmtId");
                            writer.WriteElementStringIfNotNull("InstrId", row[table.Columns.IndexOf(CdtTrfTxInfPmtInstrId)]);
                            writer.WriteElementString("EndToEndId", row[table.Columns.IndexOf(CdtTrfTxInfPmtEndToEndId)]);
                        writer.WriteEndElement(); //PmtId
                        writer.WriteStartElement("Amt");
                            writer.WriteStartElement("InstdAmt");
                                writer.WriteAttributeString("Ccy", row[table.Columns.IndexOf(CdtTrfTxInfAmtCcy)]);
                                writer.WriteValue(row[table.Columns.IndexOf(CdtTrfTxInfAmt)]);
                            writer.WriteEndElement(); //InstdAmt
                        writer.WriteEndElement(); //Amt

                        if(String.IsNullOrEmpty(row[table.Columns.IndexOf(ChqInstrChqNb)]))
                        { 
                            //Regular ACH/WIRE File
                            if (!String.IsNullOrEmpty(row[table.Columns.IndexOf(CdtrIntrmyAgt1Bic)]))
                            {
                                writer.WriteStartElement("IntrmyAgt1");
                                    writer.WriteStartElement("FinInstnId");
                                        writer.WriteElementString("BIC", row[table.Columns.IndexOf(CdtrIntrmyAgt1Bic)]);
                                    if (!String.IsNullOrEmpty(row[table.Columns.IndexOf(CdtrIntrmyAgt1BicCtry)]))
                                    {
                                        writer.WriteStartElement("PstlAdr");
                                        writer.WriteElementString("Ctry", row[table.Columns.IndexOf(CdtrIntrmyAgt1BicCtry)]);
                                        writer.WriteEndElement(); //PstlAdr

                                    }
                                    writer.WriteEndElement(); //FinInstnId
                                writer.WriteEndElement(); //IntrmyAgt1
                            }

                            writer.WriteStartElement("CdtrAgt");
                                writer.WriteStartElement("FinInstnId");
                                    writer.WriteElementStringIfNotNull("BIC", row[table.Columns.IndexOf(CdtrFinInstnBic)]);

                                    if (!String.IsNullOrEmpty(row[table.Columns.IndexOf(CdtrFinInstnClrSysMmbId)]))
                                    {
                                        writer.WriteStartElement("ClrSysMmbId");
                                            writer.WriteElementString("MmbId", row[table.Columns.IndexOf(CdtrFinInstnClrSysMmbId)]);
                                        writer.WriteEndElement(); //ClrSysMmbId
                                    }

                                    writer.WriteStartElement("PstlAdr");
                                        writer.WriteElementStringIfNotNull("StrtNm", row[table.Columns.IndexOf(CdtrFinInstnStrtNm)]);
                                        writer.WriteElementStringIfNotNull("BldgNb", row[table.Columns.IndexOf(CdtrFinInstnBldgNb)]);
                                        writer.WriteElementStringIfNotNull("PstCd", row[table.Columns.IndexOf(CdtrFinInstnPstCd)]);
                                        writer.WriteElementStringIfNotNull("TwnNm", row[table.Columns.IndexOf(CdtrFinInstnTwnNm)]);
                                        writer.WriteElementStringIfNotNull("CtrySubDvsn", row[table.Columns.IndexOf(CdtrFinInstnCtrySubDvsn)]);
                                        writer.WriteElementStringIfNotNull("Ctry", row[table.Columns.IndexOf(CdtrFinInstnCtry)]);
                                    writer.WriteEndElement(); //PstlAdr
                                writer.WriteEndElement(); //FinInstnId
                            writer.WriteEndElement(); //CdtrAgt
                        }
                        else
                        {
                            //Cheque outsourcing
                            writer.WriteStartElement("ChqInstr");
                                writer.WriteElementString("ChqNb", row[table.Columns.IndexOf(ChqInstrChqNb)]);
                                if(!String.IsNullOrEmpty(row[table.Columns.IndexOf(ChqInstrDlvryMtd)]))
                                { 
                                    writer.WriteStartElement("DlvryMtd");
                                        writer.WriteElementString("Cd", row[table.Columns.IndexOf(ChqInstrDlvryMtd)]);
                                    writer.WriteEndElement(); //DlvryMtd
                                }
                                if(!String.IsNullOrEmpty(row[table.Columns.IndexOf(ChqInstrDlvryMtdPrtry)]))
                                { 
                                    writer.WriteStartElement("DlvryMtd");
                                        writer.WriteElementString("Prtry", row[table.Columns.IndexOf(ChqInstrDlvryMtdPrtry)]);
                                    writer.WriteEndElement(); //DlvryMtd
                                }
                                if(!String.IsNullOrEmpty(row[table.Columns.IndexOf(ChqInstrDlvryToNm)]))
                                { 
                                    writer.WriteStartElement("DlvrTo");
                                        writer.WriteElementString("Nm", row[table.Columns.IndexOf(ChqInstrDlvryToNm)]);
                                        writer.WriteStartElement("Adr");
                                            writer.WriteElementStringIfNotNull("StrtNm", row[table.Columns.IndexOf(ChqInstrDlvryStrtNm)]);
                                            writer.WriteElementStringIfNotNull("BldgNb", row[table.Columns.IndexOf(ChqInstrDlvryBldgNb)]);
                                            writer.WriteElementStringIfNotNull("PstCd", row[table.Columns.IndexOf(ChqInstrDlvryPstCd)]);
                                            writer.WriteElementStringIfNotNull("TwnNm", row[table.Columns.IndexOf(ChqInstrDlvryTwnNm)]);
                                            writer.WriteElementStringIfNotNull("CtrySubDvsn", row[table.Columns.IndexOf(ChqInstrDlvryCtrySubDvsn)]);
                                            writer.WriteElementStringIfNotNull("Ctry", row[table.Columns.IndexOf(ChqInstrDlvryCtry)]);
                                            writer.WriteElementStringIfNotNull("AdrLine", row[table.Columns.IndexOf(ChqInstrDlvryAdrLine1)]);
                                            writer.WriteElementStringIfNotNull("AdrLine", row[table.Columns.IndexOf(ChqInstrDlvryAdrLine2)]);
                                        writer.WriteEndElement(); //Adr
                                    writer.WriteEndElement(); //DlvrTo
                                }
                                writer.WriteElementString("FrmsCd", row[table.Columns.IndexOf(ChqInstrFrmsCd)]);
                                writer.WriteElementStringIfNotNull("MemoFld", row[table.Columns.IndexOf(ChqInstrMemoFld)]);
                            writer.WriteEndElement(); //ChqInstr
                        }

                        //Creditor
                        writer.WriteStartElement("Cdtr");
                            writer.WriteElementString("Nm", row[table.Columns.IndexOf(CdtrNm)]);
                            writer.WriteStartElement("PstlAdr");
                                writer.WriteElementStringIfNotNull("StrtNm", row[table.Columns.IndexOf(CdtrStrtNm)]);
                                writer.WriteElementStringIfNotNull("BldgNb", row[table.Columns.IndexOf(CdtrBldgNb)]);
                                writer.WriteElementStringIfNotNull("PstCd", row[table.Columns.IndexOf(CdtrPstCd)]);
                                writer.WriteElementStringIfNotNull("TwnNm", row[table.Columns.IndexOf(CdtrTwnNm)]);
                                writer.WriteElementStringIfNotNull("CtrySubDvsn", row[table.Columns.IndexOf(CdtrCtrySubDvsn)]);
                                writer.WriteElementStringIfNotNull("Ctry", row[table.Columns.IndexOf(CdtrCtry)]);
                                writer.WriteElementStringIfNotNull("AdrLine", row[table.Columns.IndexOf(CdtrAdrLine1)]);
                                writer.WriteElementStringIfNotNull("AdrLine", row[table.Columns.IndexOf(CdtrAdrLine2)]);
                            writer.WriteEndElement(); //PstlAdr
                            if (!String.IsNullOrEmpty(row[table.Columns.IndexOf(CdtrOrgId)]))
                            {
                                writer.WriteStartElement("Id");
                                    writer.WriteStartElement("OrgId");
                                        writer.WriteStartElement("Othr");
                                            writer.WriteElementString("Id", row[table.Columns.IndexOf(CdtrOrgId)]);
                                            writer.WriteStartElement("SchmeNm");
                                                writer.WriteElementString("Cd", row[table.Columns.IndexOf(CdtrOrgSchmeNmCd)]);
                                            writer.WriteEndElement(); //SchmeNm
                                        writer.WriteEndElement(); //Othr
                                    writer.WriteEndElement(); //OrgId
                                writer.WriteEndElement(); //Id
                            }
                        writer.WriteEndElement(); //Cdtr

                        //Creditor Account -- omit node on cheque outsourcing files
                        if (!String.IsNullOrEmpty(row[table.Columns.IndexOf(CdtrAcctId)]) || !String.IsNullOrEmpty(row[table.Columns.IndexOf(CdtrAcctIbanId)]))
                        {
                            writer.WriteStartElement("CdtrAcct");
                                writer.WriteStartElement("Id");
                                    writer.WriteElementStringIfNotNull("IBAN", row[table.Columns.IndexOf(CdtrAcctIbanId)]);
                                    if (!String.IsNullOrEmpty(row[table.Columns.IndexOf(CdtrAcctId)]))
                                    { 
                                        writer.WriteStartElement("Othr");
                                            writer.WriteElementString("Id", row[table.Columns.IndexOf(CdtrAcctId)]);
                                        writer.WriteEndElement(); //Othr
                                    }
                                writer.WriteEndElement(); //Id

                                if (!String.IsNullOrEmpty(row[table.Columns.IndexOf(CdtrAcctTp)]))
                                { 
                                    writer.WriteStartElement("Tp");
                                        writer.WriteElementString("Cd", row[table.Columns.IndexOf(CdtrAcctTp)]);
                                    writer.WriteEndElement(); //Tp
                                }
                            writer.WriteEndElement(); //CdtrAcct
                        }

                        //Email remittance
                        if(!String.IsNullOrEmpty(row[table.Columns.IndexOf(RmtInfEmail)]))
                        { 
                            if (!String.IsNullOrEmpty(row[table.Columns.IndexOf(RmtInfEmail)]))
                            {
                                writer.WriteStartElement("RltdRmtInf");
                                    writer.WriteElementString("RmtId", "Remittance");
                                    writer.WriteElementString("RmtLctnMtd", "EMAL");
                                    writer.WriteElementString("RmtLctnElctrncAdr", row[table.Columns.IndexOf(RmtInfEmail)]);
                                    writer.WriteStartElement("RmtLctnPstlAdr");
                                        writer.WriteElementString("Nm", row[table.Columns.IndexOf(RmtInfPstlAdrNm)]);
                                        writer.WriteStartElement("Adr");
                                            writer.WriteElementString("Ctry", row[table.Columns.IndexOf(RmtInfPstlAdrCtry)]);
                                        writer.WriteEndElement(); //Adr
                                    writer.WriteEndElement(); //RmtLctnPstlAdr
                                writer.WriteEndElement(); //RltdRmtInf
                            }
                        }

                        //Detailed Remittance Information 
                        if (!String.IsNullOrEmpty(row[table.Columns.IndexOf(RmtInfUstrd)]) || !String.IsNullOrEmpty(row[table.Columns.IndexOf(RmtInfAPRefNbr)]))
                        {
                            writer.WriteStartElement("RmtInf");
                                //Unstructured remittance
                                writer.WriteElementStringIfNotNull("Ustrd", row[table.Columns.IndexOf(RmtInfUstrd)], 75);

                                //Structured remittance - we are getting it directly from the AP tables because this info is not available in batch payments
                                if (!String.IsNullOrEmpty(row[table.Columns.IndexOf(RmtInfAPRefNbr)]))
                                {
                                    WriteStructuredAPRemittanceInformation(writer, row[table.Columns.IndexOf(RmtInfAPRefNbr)], row[table.Columns.IndexOf(CdtTrfTxInfAmtCcy)], row[table.Columns.IndexOf(RmtInfRefInfoCd)], row[table.Columns.IndexOf(RmtInfRefInfoPrtry)]);
                                }
                            writer.WriteEndElement(); //RmtInf
                        }

                    writer.WriteEndElement(); //CdtTrfTxInf
                }

                writer.WriteEndElement(); //PmtInf
                writer.WriteEndElement(); //CstmrCdtTrfInitn
                writer.WriteEndElement(); //Document
                writer.WriteEndDocument(); 
                writer.Flush();
            }

            if(this.GetParameter(DisableXmlSchemaValidation) != "True" && this.GetParameter(DisableXmlSchemaValidation) != "true")
            { 
                ValidateXmlDocument(doc);
            }

            return encoding.GetBytes(doc.OuterXml);
        }

        private void WriteStructuredAPRemittanceInformation(XmlWriter writer, string refNbr, string currencyID, string cdtrRefInfCd, string cdtrRefInfPrtry)
        {
            var payments = new PXSelectJoin<APAdjust,
                LeftJoin<APInvoice, On<APAdjust.adjdDocType, Equal<APInvoice.docType>, And<APAdjust.adjdRefNbr, Equal<APInvoice.refNbr>>>>,
                Where<APAdjust.adjgDocType, Equal<APDocType.check>, And<APAdjust.adjgRefNbr, Equal<Required<APAdjust.adjgRefNbr>>>>>(new PXGraph());

            foreach (PXResult<APAdjust, APInvoice> p in payments.Select(refNbr))
            {
                var adj = (APAdjust)p;
                var inv = (APInvoice)p;

                writer.WriteStartElement("Strd");
                    //Referred Document Information
                    writer.WriteStartElement("RfrdDocInf");
                        writer.WriteStartElement("Tp"); 
                            writer.WriteStartElement("CdOrPrtry");
                                writer.WriteElementString("Cd", GetReferredDocumentTypeFromAPDocType(adj.AdjdDocType));
                            writer.WriteEndElement(); //CdOrPrtry
                        writer.WriteEndElement(); //Tp
                        if(inv != null && !String.IsNullOrEmpty(inv.InvoiceNbr)) writer.WriteElementStringIfNotNull("Nb", inv.InvoiceNbr, 35); else writer.WriteElementStringIfNotNull("Nb", adj.AdjdRefNbr, 35);
                        if(inv != null && inv.InvoiceDate != null) writer.WriteElementString("RltdDt", FormatDate(inv.DocDate.Value));
                    writer.WriteEndElement(); //RfrdDocInf

                    //Referred Document Amount
                    writer.WriteStartElement("RfrdDocAmt");
                        if(inv != null)
                        { 
                            writer.WriteStartElement("DuePyblAmt");
                                writer.WriteAttributeString("Ccy", currencyID);
                                writer.WriteValue(FormatAmount(inv.CuryOrigDocAmt.GetValueOrDefault()));
                            writer.WriteEndElement(); //DuePyblAmt
                        }
                        writer.WriteStartElement("DscntApldAmt");
                            writer.WriteAttributeString("Ccy", currencyID);
                            writer.WriteValue(FormatAmount(adj.CuryAdjgDiscAmt.GetValueOrDefault()));
                        writer.WriteEndElement(); //DscntApldAmt
                        writer.WriteStartElement("RmtdAmt");
                            writer.WriteAttributeString("Ccy", currencyID);
                            writer.WriteValue(FormatAmount(adj.CuryAdjgAmt.GetValueOrDefault()));
                        writer.WriteEndElement(); //RmtdAmt
                    writer.WriteEndElement(); //RfrdDocAmt
                    
                    //Creditor Referrence Information
                    writer.WriteStartElement("CdtrRefInf");
                        if(!String.IsNullOrEmpty(cdtrRefInfCd) || !String.IsNullOrEmpty(cdtrRefInfPrtry))
                        {
                            writer.WriteStartElement("Tp");
                                writer.WriteStartElement("CdOrPrtry");
                                    writer.WriteElementStringIfNotNull("Cd", cdtrRefInfCd);
                                    writer.WriteElementStringIfNotNull("Prtry", cdtrRefInfPrtry);
                                writer.WriteEndElement(); //CdOrPrtry
                            writer.WriteEndElement(); //Tp
                        }
                        if(inv != null) writer.WriteElementStringIfNotNull("Ref", inv.InvoiceNbr, 35);
                    writer.WriteEndElement(); //CdtrRefInf
                    //if (inv != null) writer.WriteElementStringIfNotNull("AddtlRmtInf", inv.DocDesc, 140); JP Morgan is asking us to remove this and keep the Ref tag only. Add an option in the mapping if this needs to be enabled for others
                writer.WriteEndElement(); //Strd

            }
        }

        private static string GetReferredDocumentTypeFromAPDocType(string docType)
        {
            switch(docType)
            {
                case APDocType.DebitAdj:
                    return "DEBN";
                case APDocType.CreditAdj:
                    return "CREN";
                case APDocType.Invoice:
                default:
                    return "CINV";
            }

            throw new NotImplementedException();
        }

        private void ValidateXmlDocument(XmlDocument doc)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.CloseInput = true;
            settings.ValidationEventHandler += SchemaValidationEventHandler;
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings |
                XmlSchemaValidationFlags.ProcessIdentityConstraints |
                XmlSchemaValidationFlags.ProcessInlineSchema |
                XmlSchemaValidationFlags.ProcessSchemaLocation;

            using (XmlReader schemaReader = XmlReader.Create(new StringReader(Properties.Resources.pain_001_001_03_xsd)))
            {
                var schema = settings.Schemas.Add(null, schemaReader);
            }

            using (XmlReader validatingReader = XmlReader.Create(new StringReader(doc.OuterXml), settings))
            {
                while (validatingReader.Read()) { /* just loop through document */ }
            }
        }
        private void SchemaValidationEventHandler(object sender, ValidationEventArgs e)
        {
            switch (e.Severity)
            {
                case XmlSeverityType.Error:
                    throw new PXException(PXLocalizer.LocalizeFormat(Messages.XmlFailedValidation, e.Message));
                case XmlSeverityType.Warning:
                    PXTrace.WriteInformation(Messages.XmlValidationWarning, e.Message);
                    break;
            }

        }
        protected override PX.SM.FileInfo SetFile(byte[] bytes)
        {
            PX.SM.FileInfo fi = base.SetFile(bytes);
            if (_notes != null && _notes.Count > 0) SaveNotes((Guid)fi.UID, _notes);
            return fi;
        }

        protected virtual void SaveNotes(Guid fileId, IEnumerable<String> notes)
        {
            foreach (String note in notes)
            {
                if (!String.IsNullOrEmpty(note))
                {
                    using (var record = PXDatabase.SelectSingle<NoteDoc>(
                        new PXDataField("NoteID"),
                        new PXDataFieldValue("NoteID", PXDbType.UniqueIdentifier, Guid.Parse(note)),
                        new PXDataFieldValue("FileID", PXDbType.UniqueIdentifier, fileId)))
                    {

                        if (record == null)
                        {
                            PXDatabase.Insert<NoteDoc>(
                                new PXDataFieldAssign("NoteID", PXDbType.UniqueIdentifier, Guid.Parse(note)),
                                new PXDataFieldAssign("FileID", PXDbType.UniqueIdentifier, fileId));
                        }
                    }
                }
            }
        }

        protected override void InternalImport(PXSYTable table)
        {
            //We do export only with this provider
            throw new NotImplementedException();
        }

        protected Dictionary<string, string> Cache
        {
            get
            {
                if (this._cache == null)
                    this._cache = new Dictionary<string, string>();
                return this._cache;
            }
        }

        #region Public functions called from Export Scenario
        public virtual string GetBatchNbr()
        {
            return this.GetParameter(IncomingBatchNumber);
        }

        public virtual string Coalesce(string aFirst, string aSecond)
        {
            if (string.IsNullOrEmpty(aFirst))
                return aSecond;
            return aFirst;
        }

        public string FormatDateTime(DateTime date)
        {
            return date.ToString("yyyy-MM-dd'T'HH:mm:ss");
        }

        public string FormatDate(DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }
        
        public virtual string FormatAmount(decimal amount)
        {
            return amount.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
        }

        public virtual string CacheValue(string key, string value)
        {
            if (this.Cache.ContainsKey(key))
            {
                if (string.IsNullOrEmpty(value))
                    return this.Cache[key];
            }
            this.Cache[key] = value;
            return value;
        }
        #endregion
    }
}