using System;
using System.Collections.Generic;
using System.Text;

namespace BaiParser
{
    public class FileHeader
    {
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string FileCreationDate { get; set; }
        public string FileCreationTime { get; set; }
        public long FileIdNumber { get; set; } = 1;
        public long RecordLength { get; set; }
        public long? BlockSize { get; set; }
        public decimal VersionNumber { get; set; }
    }
}
