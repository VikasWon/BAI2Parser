using System;
using System.Collections.Generic;
using System.Text;

namespace BaiParser
{
    public class AccountDetails
    {
        public string TypeCode { get; set; }
        public decimal Amount { get; set; }
        public long ItemCount { get; set; }
        public FundType FundsType { get; set; }
    }
}
