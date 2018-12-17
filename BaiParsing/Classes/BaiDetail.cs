using System.Collections.Generic;

namespace Velixo.HsbcEBanking.BaiParsing
{
    public class BaiDetail
    {
        public string TransactionDetail { get; private set; }
        public List<string> DetailContinuation { get; internal set; }
        public BaiDetail(string line)
        {
            TransactionDetail = line;
            DetailContinuation = new List<string>();
        }
    }
}