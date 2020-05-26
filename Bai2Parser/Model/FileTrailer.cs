using System;
using System.Collections.Generic;
using System.Text;

namespace BaiParser
{
    public class FileTrailer
    {
        public decimal FileControlTotal { get; set; }
        public long NumberOfGroups { get; set; }
        public long NumberOfRecords { get; set; }
    }
}
