using BaiParser;
using System;
using System.IO;

namespace BAI2Parser.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            string relativePath = @".\bai2samplefile.txt";
            string filePath = Path.GetFullPath(relativePath);
            Bai2Parser parser = new Bai2Parser();
            var parsedContent = parser.Parse(filePath);
        }
    }
}
