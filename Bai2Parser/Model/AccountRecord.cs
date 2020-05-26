using System;
using System.Collections.Generic;
using System.Text;

namespace BaiParser
{

    public class AccountRecord
    {
        public AccountRecord()
        {
            Details = new List<AccountDetails>();
            TransactionRecords = new List<TransactionRecord>();
        }
        public string AccountNumber { get; set; }
        public string CurrencyCode { get; set; }
        public List<AccountDetails> Details { get; set; }
        public List<TransactionRecord> TransactionRecords { get; set; }
        public AccountTotals AccountTotals { get; set; }

    }
}
