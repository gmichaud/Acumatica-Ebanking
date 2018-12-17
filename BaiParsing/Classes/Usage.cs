using System;

namespace Velixo.HsbcEBanking.BaiParsing
{
    [AttributeUsage(AttributeTargets.Property)]
    public class Usage : Attribute
    {
        public UsageType Type { get; private set; }
        public Usage(UsageType usageType)
        {
            Type = usageType;
        }
    }
}