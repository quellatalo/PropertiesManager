using System;
using System.Collections.Generic;
using System.Text;

namespace Quellatalo.Nin.PropertiesManager
{
    internal class Util
    {
        private static readonly string INVALID_INDEX_EXCEPTION = "Starting index should not be negative.";
        internal static int IndexOfNonWhitespace(string source, int startIndex = 0)
        {
            if (startIndex < 0) throw new IndexOutOfRangeException(INVALID_INDEX_EXCEPTION);
            if (source != null)
                for (int i = startIndex; i < source.Length; i++)
                    if (!char.IsWhiteSpace(source[i])) return i;
            return -1;
        }
    }
}
