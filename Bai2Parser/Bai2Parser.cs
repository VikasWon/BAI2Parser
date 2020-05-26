using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BaiParser
{
    public class Bai2Parser
    {
        /// <summary>
        /// Parse the BAI2 flat file to C# classes
        /// </summary>
        /// <param name="fileName">Full File Path</param>
        /// <returns></returns>
        public Bai2Content Parse(string fileName)
        {
            if(!File.Exists(fileName)) throw new Exception("File not found, nothing to parse");
            var data = File.ReadAllLines(fileName);
            var warpedData = CreateWarpedData(data);
            var parsedData = ParseBai(warpedData);
            return parsedData;
        }

        private Bai2Content ParseBai(string[] warpedData)
        {
            Bai2Content baiRecord = new Bai2Content();
            if(warpedData != null && warpedData.Length > 0)
            {
                bool newGroup = true;
                bool newAccount = true;
                Group currentGroup = null;
                AccountRecord currentAccount = null;
                foreach(var line in warpedData)
                {
                    try
                    {
                        if(string.IsNullOrWhiteSpace(line))
                            continue;
                        var lineFields = line.Split(',');
                        #region File Header
                        if(lineFields[0].Equals("01") && lineFields.Length == 9)
                        {
                            FileHeader fileHeader = new FileHeader();
                            fileHeader.SenderId = lineFields[1];
                            fileHeader.ReceiverId = lineFields[2];
                            fileHeader.FileCreationDate = lineFields[3];
                            fileHeader.FileCreationTime = lineFields[4];
                            if(!string.IsNullOrWhiteSpace(lineFields[5]))
                            {
                                if(long.TryParse(lineFields[5], out long output))
                                    fileHeader.FileIdNumber = output;
                                else
                                    fileHeader.FileIdNumber = 1;
                            }
                            if(!string.IsNullOrWhiteSpace(lineFields[6]))
                            {
                                if(long.TryParse(lineFields[6], out long output))
                                    fileHeader.RecordLength = output;
                                else
                                    fileHeader.RecordLength = 0;
                            }
                            if(!string.IsNullOrWhiteSpace(lineFields[7]))
                            {
                                if(long.TryParse(lineFields[7], out long output))
                                    fileHeader.BlockSize = output;
                                else
                                    fileHeader.BlockSize = null;
                            }
                            if(!string.IsNullOrWhiteSpace(lineFields[8]))
                            {
                                if(long.TryParse(lineFields[8], out long output))
                                    fileHeader.VersionNumber = output;
                                else
                                    fileHeader.VersionNumber = 2;
                            }
                            baiRecord.FileHeader = fileHeader;
                        }
                        #endregion File Header
                        #region Group Header
                        if(lineFields[0].Equals("02") && lineFields.Length == 8 && newGroup)
                        {
                            newGroup = false;
                            Group groupHeader = new Group();
                            groupHeader.ReceiverId = lineFields[1];
                            groupHeader.SenderId = lineFields[2];
                            groupHeader.GroupStatus = lineFields[3];
                            groupHeader.AsOfDate = lineFields[4];
                            groupHeader.AsOfTime = lineFields[5];
                            groupHeader.CurrencyCode = lineFields[6];
                            if(!string.IsNullOrWhiteSpace(lineFields[7]))
                            {
                                if(int.TryParse(lineFields[7], out int output))
                                {
                                    groupHeader.AsOfDateModifier = output;
                                }
                            }
                            currentGroup = groupHeader;
                        }
                        #endregion Group Header
                        #region Account Record
                        if(lineFields[0].Equals("03") && lineFields.Length >= 7 && newAccount)
                        {
                            newAccount = false;
                            AccountRecord accountRecord = new AccountRecord();

                            accountRecord.AccountNumber = lineFields[1];
                            accountRecord.CurrencyCode = lineFields[2];
                            for(int i = 3; i < lineFields.Length; i++)
                            {
                                int count = 1;
                                AccountDetails accountDetail = new AccountDetails();
                                accountDetail.TypeCode = lineFields[i];
                                if(!string.IsNullOrWhiteSpace(lineFields[i + count]))
                                {
                                    if(decimal.TryParse(lineFields[i + count], out decimal output))
                                    {
                                        accountDetail.Amount = output;
                                    }
                                    else
                                    {
                                        accountDetail.Amount = 0;
                                    }
                                }
                                count++;
                                if(!string.IsNullOrWhiteSpace(lineFields[i + count]))
                                {
                                    if(long.TryParse(lineFields[i + count], out long output))
                                    {
                                        accountDetail.ItemCount = output;
                                    }
                                    else
                                    {
                                        accountDetail.ItemCount = 0;
                                    }
                                }
                                count++;
                                if(!string.IsNullOrWhiteSpace(lineFields[i + count]))
                                {
                                    FundType fundType = CreateFundType(lineFields, i, ref count);
                                    accountDetail.FundsType = fundType;
                                }
                                i += count;
                                accountRecord.Details.Add(accountDetail);
                            }
                            currentAccount = accountRecord;
                        }
                        #endregion Account Record
                        #region Transaction Record
                        if(lineFields[0].Equals("16") && lineFields.Length >= 7)
                        {
                            TransactionRecord transaction = new TransactionRecord();
                            transaction.TypeCode = lineFields[1];
                            if(!string.IsNullOrWhiteSpace(lineFields[2]))
                            {
                                if(decimal.TryParse(lineFields[2], out decimal output))
                                {
                                    transaction.Amount = output;
                                }
                                else
                                {
                                    transaction.Amount = 0;
                                }
                            }
                            for(int i = 3; i < lineFields.Length; i++)
                            {
                                int count = 0;
                                FundType fundType = CreateFundType(lineFields, i, ref count);
                                transaction.FundsType = fundType;
                                count++;
                                transaction.BankRefeneceNumber = lineFields[i + count];
                                count++;
                                transaction.CustomerReferenceNumber = lineFields[i + count];
                                count++;
                                transaction.Text = lineFields[i + count];
                                i += count;
                            }
                            currentAccount.TransactionRecords.Add(transaction);
                        }
                        #endregion Transaction Record
                        #region Account Totals
                        if(lineFields[0].Equals("49") && lineFields.Length == 3 && !newAccount)
                        {
                            newAccount = true;
                            AccountTotals accountTotals = new AccountTotals();
                            if(!string.IsNullOrWhiteSpace(lineFields[1]))
                            {
                                if(decimal.TryParse(lineFields[1], out decimal output))
                                {
                                    accountTotals.AccountControlTotal = output;
                                }
                            }
                            if(!string.IsNullOrWhiteSpace(lineFields[2]))
                            {
                                if(long.TryParse(lineFields[2], out long output))
                                {
                                    accountTotals.NumberOfRecords = output;
                                }
                            }
                            currentAccount.AccountTotals = accountTotals;
                            currentGroup.Accounts.Add(currentAccount);
                            currentAccount = null;
                        }
                        #endregion Account Totals
                        #region Group Totals
                        if(lineFields[0].Equals("98") && lineFields.Length == 4 && !newGroup)
                        {
                            newGroup = true;
                            GroupTotals groupTotals = new GroupTotals();
                            if(!string.IsNullOrWhiteSpace(lineFields[1]))
                            {
                                if(decimal.TryParse(lineFields[1], out decimal output))
                                {
                                    groupTotals.GroupControlTotal = output;
                                }
                            }
                            if(!string.IsNullOrWhiteSpace(lineFields[2]))
                            {
                                if(long.TryParse(lineFields[2], out long output))
                                {
                                    groupTotals.NumberOfAccounts = output;
                                }
                            }
                            if(!string.IsNullOrWhiteSpace(lineFields[3]))
                            {
                                if(long.TryParse(lineFields[3], out long output))
                                {
                                    groupTotals.NumberOfRecords = output;
                                }
                            }
                            if(currentGroup != null)
                            {
                                currentGroup.GroupTotals = groupTotals;
                                baiRecord.Groups.Add(currentGroup);
                                currentGroup = null;
                            }
                        }
                        #endregion Group Totals
                        #region File Trailer
                        if(lineFields[0].Equals("99") && lineFields.Length == 4)
                        {
                            FileTrailer fileTrailer = new FileTrailer();
                            if(!string.IsNullOrWhiteSpace(lineFields[1]))
                            {
                                if(decimal.TryParse(lineFields[1], out decimal output))
                                {
                                    fileTrailer.FileControlTotal = output;
                                }
                            }
                            if(!string.IsNullOrWhiteSpace(lineFields[2]))
                            {
                                if(long.TryParse(lineFields[2], out long output))
                                {
                                    fileTrailer.NumberOfGroups = output;
                                }
                            }
                            if(!string.IsNullOrWhiteSpace(lineFields[3]))
                            {
                                if(long.TryParse(lineFields[3], out long output))
                                {
                                    fileTrailer.NumberOfRecords = output;
                                }
                            }
                            baiRecord.FileTrailer = fileTrailer;
                        }
                        #endregion File Trailer
                    }
                    catch(IndexOutOfRangeException iex)
                    {
                        var ex = new Exception($"Incorrect data in file for warped record : {line}", iex);
                        throw ex;
                    }
                }

            }
            return baiRecord;
        }

        private static FundType CreateFundType(string[] lineFields, int i, ref int count)
        {
            FundType fundType = new FundType();
            fundType.FundsType = lineFields[i + count];
            switch(lineFields[i + count])
            {
                case "S":
                    count++;
                    if(!string.IsNullOrWhiteSpace(lineFields[i + count]))
                    {
                        if(decimal.TryParse(lineFields[i + count], out decimal output))
                            fundType.ImmediateAmount = output;
                    }
                    count++;
                    if(!string.IsNullOrWhiteSpace(lineFields[i + count]))
                    {
                        if(decimal.TryParse(lineFields[i + count], out decimal output))
                            fundType.OneDayAmount = output;
                    }
                    count++;
                    if(!string.IsNullOrWhiteSpace(lineFields[i + count]))
                    {
                        if(decimal.TryParse(lineFields[i + count], out decimal output))
                            fundType.TwoOrMoreDaysAmount = output;
                    }
                    break;
                case "V":
                    count++;
                    fundType.ValueDate = lineFields[i + count];
                    count++;
                    fundType.ValueTime = lineFields[i + count];
                    break;
                case "D":
                    count++;
                    long noOfDistributions = 0;
                    if(!string.IsNullOrWhiteSpace(lineFields[i + count]))
                    {
                        if(long.TryParse(lineFields[i + count], out long output))
                        {
                            fundType.NumberOfDistributions = output;
                            noOfDistributions = output;
                        }
                    }
                    for(int j = 0; j < noOfDistributions; j++)
                    {
                        count++;
                        int days = 0;
                        decimal availableAmount = 0;
                        if(!string.IsNullOrWhiteSpace(lineFields[i + count]))
                        {
                            int.TryParse(lineFields[i + count], out days);
                        }
                        count++;
                        if(!string.IsNullOrWhiteSpace(lineFields[i + count]))
                        {
                            decimal.TryParse(lineFields[i + count], out availableAmount);
                        }
                        fundType.DistributionInfo.Add(days, availableAmount);
                    }
                    break;
            }

            return fundType;
        }

        private string[] CreateWarpedData(string[] data)
        {
            string[] singleLineRecords = null;
            if(data != null && data.Length > 0)
            {
                StringBuilder sb = new StringBuilder();
                string curLine;
                string finalRow;
                bool isLastFieldText = false;
                string lastField = null;
                string previousRecType = "01";
                bool isText = false;
                foreach(var line in data)
                {
                    if(string.IsNullOrWhiteSpace(line))
                        continue;

                    if(line.TrimEnd().EndsWith("/"))
                    {
                        curLine = line.Remove(line.LastIndexOf('/'));
                    }
                    else
                    {
                        curLine = line;
                    }
                    curLine = curLine.Trim();
                    var lineFields = curLine.Split(',');
                    if(lineFields != null && lineFields.Length > 0)
                    {
                        //lastField = lineFields.Last();
                        var isLong = long.TryParse(lastField, out long longVal);
                        var isChar = char.TryParse(lastField, out char charVal);
                        if(!isLong && !isChar && !string.IsNullOrEmpty(lastField))
                            isLastFieldText = true;
                        else
                            isLastFieldText = false;
                        if(isLastFieldText && !previousRecType.Equals("16"))
                        {
                            isLastFieldText = false;
                        }
                    }
                    if(!line.StartsWith("88"))
                    {
                        finalRow = $"|{curLine}";
                        previousRecType = line.Substring(0, 2);
                        isText = false;
                    }
                    else
                    {
                        if(isLastFieldText)
                        {
                            if(!previousRecType.Equals("16"))
                            {
                                finalRow = $" {curLine.Substring(3)}";
                            }
                            else
                            {
                                finalRow = $" {curLine.Substring(3).Replace(",", string.Empty)}";
                                isText = true;
                            }
                        }
                        else
                        {
                            if(!previousRecType.Equals("16"))
                            {
                                if(previousRecType.Equals("03"))
                                {
                                    finalRow = curLine.Substring(2);
                                }
                                else
                                    finalRow = curLine.Substring(3);
                            }
                            else
                            {
                                finalRow = curLine.Substring(2);
                            }
                        }
                    }
                    if(isText && previousRecType.Equals("16"))
                    {
                        finalRow = finalRow.Replace(",", string.Empty);
                    }
                    if(finalRow.LastIndexOf(',') >= 0)
                    {
                        lastField = finalRow.Substring(finalRow.LastIndexOf(',') + 1);
                    }
                    else
                    {
                        lastField = finalRow.Trim('|');
                    }
                    sb.Append(finalRow);
                }
                singleLineRecords = sb.ToString().Trim('|').Split('|');
            }
            return singleLineRecords;
        }
    }
}
