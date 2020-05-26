using BaiParser;
using System;

namespace BAI2Parser.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            string filePath = @"C:\REL Docs\Bank Integration - Transactions\BMO\sampleBAI.txt";
            Bai2Parser parser = new Bai2Parser();
            var parsedContent = parser.Parse(filePath);
        }
    }
}
