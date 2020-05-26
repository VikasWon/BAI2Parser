using System;
using System.Collections.Generic;
using System.Text;

namespace BaiParser
{
    public class TransactionRecord
    {
        public string TypeCode { get; set; }
        public decimal Amount { get; set; }
        public FundType FundsType { get; set; }
        public string BankRefeneceNumber { get; set; }
        public string CustomerReferenceNumber { get; set; }
        public string Text { get; set; }
    }
}
