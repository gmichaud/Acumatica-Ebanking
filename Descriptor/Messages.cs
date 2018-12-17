using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Velixo.HsbcEBanking
{
    [PX.Common.PXLocalizable]
    public static class Messages
    {
        public const string XmlFailedValidation = "The XML file failed validation: {0}";
        public const string XmlValidationWarning = "XML Validation Warning: {0}";
        public const string DisableXmlSchemaValidation = "Disable XML Schema Validation (debug mode)";
    }
}
