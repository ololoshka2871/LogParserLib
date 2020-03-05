using System;

namespace LogParser.Import
{
    // http://msdn.microsoft.com/en-us/library/ms229064(v=vs.100).aspx
    [Serializable]
    public class RawCSVStringParseException : Exception
    {
        public RawCSVStringParseException(string row, int rowNumber)
            : base(row)
        {
            RowNumber = rowNumber;
        }

        public int RowNumber { get; private set; }
    }
}
