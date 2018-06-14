using System;
using System.Xml;

namespace NexVue.HsbcEBanking
{
    internal static class XmlWriterExtensions
    {
        public static void WriteElementStringIfNotNull(this XmlWriter writer, string localName, string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                writer.WriteElementString(localName, value);
            }
        }
    }
}
