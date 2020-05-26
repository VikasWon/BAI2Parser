using System;
using System.Collections.Generic;

namespace BaiParser
{
    public class Bai2Content
    {
        public Bai2Content()
        {
            Groups = new List<Group>();
        }
        public FileHeader FileHeader { get; set; }
        public List<Group> Groups { get; set; }
        public FileTrailer FileTrailer { get; set; }
    }
}
