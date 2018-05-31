using PX.Api;
using PX.Data;
using PX.DataSync;
using PX.SM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace NexVue.HsbcEBanking
{
    public class HsbcAchProvider : PXSYBaseEncodedFileProvider, IPXSYProvider
    {
        private const String INCOMING_BATCH_NUMBER = "BatchNbr";

        protected const string NoteID = "NoteID";
        protected const string FileID = "FileName";
        protected List<String> _notes = new List<String>();
        private Dictionary<string, string> _cache;

        public string DefaultFileExtension => ".xml";

        public override string ProviderName => "HSBC ACH Export Provider";

        public override PXFieldState[] GetSchemaFields(string objectName)
        {
            List<PXFieldState> ret = new List<PXFieldState>(base.GetSchemaFields(objectName));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, NoteID, typeof(Int64))));
            ret.Add(CreateFieldState(new SchemaFieldInfo(-1, FileID)));

            var encoding = GetEncoding();
            var columns = new HashSet<string>();
            using (var streamReader = new StreamReader(new MemoryStream(_file.BinData, false), encoding))
            using (var reader = XmlReader.Create(streamReader))
            {
                reader.MoveToContent();
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        columns.Add(reader.Name);
                    }
                }
            }
            
            if (columns == null)
            {
                throw new PXException(PX.DataSync.Messages.XmlMalformed);
            }

            foreach(var col in columns)
            { 
                ret.Add(CreateFieldState(new SchemaFieldInfo(-1, col)));
            }

            return ret.ToArray();
        }

        protected override List<PXStringState> FillParameters()
        {
            List<PXStringState> ret = base.FillParameters();
            ret.Add(CreateParameter(INCOMING_BATCH_NUMBER, Titles.IncomingBatchNumber, String.Empty));
            return ret;
        }

        protected override byte[] InternalExport(string objectName, PXSYTable table)
        {
            _notes.Clear();
            Int32 noteIndex = table.Columns.IndexOf(NoteID);
            if (noteIndex >= 0) _notes.AddRange(table.Select(r => r[noteIndex]).Distinct());

            Encoding encoding = GetEncoding();
        
            var settings = new XmlWriterSettings();
            settings.Encoding = encoding;
            settings.Indent = true;
            settings.ConformanceLevel = ConformanceLevel.Auto;

            var memoryStream = new MemoryStream();
            using (var writer = XmlWriter.Create(memoryStream, settings))
            {
                //TODO: Header
                writer.WriteStartDocument();
                writer.WriteStartElement("Document");

                writer.WriteStartElement("CstmrCdtTrfInitn");
                writer.WriteEndElement();

                writer.WriteStartElement("GrpHdr");
                writer.WriteEndElement();

                foreach (PXSYRow row in table)
                {
                    writer.WriteStartElement("PmtInf");
                    foreach(var col in table.Columns)
                    {
                        writer.WriteStartElement(col);
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();
                memoryStream.Position = 0;
                return memoryStream.ToArray();
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
            if (notes.Count() <= 0) return;

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
            return this.GetParameter(INCOMING_BATCH_NUMBER);
        }

        public string FormatDateTime(DateTime date)
        {
            return date.ToString("yyyy-MM-dd'T'HH:mm:ss");
        }

        public string FormatDate(DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
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