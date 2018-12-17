using System;
using System.Xml;

namespace Velixo.HsbcEBanking
{
    internal static class XmlWriterExtensions
    {
        public static void WriteElementStringIfNotNull(this XmlWriter writer, string localName, string value, int maxLength = 0)
        {
            if (!String.IsNullOrEmpty(value))
            {
                if(maxLength > 0 && value.Length > maxLength)
                {
                    value = value.Substring(0, maxLength);
                }

                writer.WriteElementString(localName, value);
            }
        }
    }
}
