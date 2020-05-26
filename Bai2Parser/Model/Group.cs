using System;
using System.Collections.Generic;
using System.Text;

namespace BaiParser
{
    public class Group
    {
        public Group()
        {
            Accounts = new List<AccountRecord>();
        }
        public string ReceiverId { get; set; }
        public string SenderId { get; set; }
        public string GroupStatus { get; set; }
        public string AsOfDate { get; set; }
        public string AsOfTime { get; set; }
        public string CurrencyCode { get; set; }
        public int AsOfDateModifier { get; set; }
        public List<AccountRecord> Accounts { get; set; }
        public GroupTotals GroupTotals { get; set; }
    }
}
