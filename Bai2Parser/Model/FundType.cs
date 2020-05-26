using System;
using System.Collections.Generic;
using System.Text;

namespace BaiParser
{
    public class FundType
    {
        public FundType()
        {
            DistributionInfo = new Dictionary<int, decimal>();
        }
        //public string TypeCode { get; set; }
        public string FundsType { get; set; }
        public decimal ImmediateAmount { get; set; }
        public decimal OneDayAmount { get; set; }
        public decimal TwoOrMoreDaysAmount { get; set; }
        public string ValueDate { get; set; }
        public string ValueTime { get; set; }
        public long NumberOfDistributions { get; set; }
        public Dictionary<int, decimal> DistributionInfo { get; set; }
    }

}
