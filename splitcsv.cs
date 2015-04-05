using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace PRTools
{
    public class SplitCsv
    {
        public string[] SplitCSVLine(string lineData)
        {
            ArrayList columnsValue;


            if (lineData.IndexOf(',') != -1)
            {
                columnsValue = SplitLine(lineData);
            }
            else
            {
                columnsValue = new ArrayList();
                columnsValue.Add(lineData);
            }

            return (string[])columnsValue.ToArray(typeof(string));
        }

        private ArrayList SplitLine(string originalValue)
        {
            ArrayList columnsValue;
            string notSplit;
            int preCommaPos;
            int lastCommaPos;
            char splitBy;
            string temp;


            notSplit = originalValue;
            columnsValue = new ArrayList();
            splitBy = ',';

            columnsValue.Add("");

            preCommaPos = 0;
            while (notSplit != string.Empty)
            {
                lastCommaPos = notSplit.IndexOf(splitBy, preCommaPos);
                if (lastCommaPos == -1)
                {
                    if (IsEvenQuotation(notSplit))
                    {
                        columnsValue[columnsValue.Count - 1] = FormatStr(notSplit);
                    }
                    else
                    {
                        columnsValue[columnsValue.Count - 1] = notSplit;
                    }
                }
                if (lastCommaPos > -1)
                {
                    temp = notSplit.Substring(0, lastCommaPos);
                    if (IsEvenQuotation(temp))
                    {
                        columnsValue[columnsValue.Count - 1] = FormatStr(temp);
                        columnsValue.Add("");
                        preCommaPos = 0;
                        notSplit = notSplit.Substring(lastCommaPos + 1);
                    }
                    else
                    {
                        preCommaPos = lastCommaPos + 1;
                    }
                }
                else
                {
                    notSplit = string.Empty;
                }
            }

            return columnsValue;
        }

        private bool IsEvenQuotation(string tempString)
        {
            int quotationCount;
            int start;

            quotationCount = 0;
            start = -1;

            while (tempString.IndexOf('"', start + 1) != -1)
            {
                start = tempString.IndexOf('"', start + 1);
                quotationCount = quotationCount + 1;
            }
            if (quotationCount % 2 != 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private string FormatStr(string tempString)
        {
            string rtnString = tempString;
            if (tempString.StartsWith("\"") == true)
            {
                rtnString = rtnString.Substring(1);
                rtnString = rtnString.Substring(0, rtnString.Length - 1);
            }
            return rtnString.Replace("\"\"", "\"");
        }
    }
}
