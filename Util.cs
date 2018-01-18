using System;
using System.Collections.Generic;
using System.Text;

namespace Quellatalo.Nin.PropertiesManager
{
    internal class Util
    {
        internal static int IndexOfNonWhitespace(string source, int startIndex = 0)
        {
            if (startIndex < 0) throw new IndexOutOfRangeException("Starting index should not be negative.");
            if (source != null)
                for (int i = startIndex; i < source.Length; i++)
                    if (!char.IsWhiteSpace(source[i])) return i;
            return -1;
        }
    }
}
