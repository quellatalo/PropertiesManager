namespace Quellatalo.Nin.PropertiesManager
{
    internal class LineEntry
    {
        public string Entry { get; set; }
        public bool IsProperty
        {
            get
            {
                return PropFileManager.IsPropertyLine(Entry);
            }
        }

        public string Key
        {
            get
            {
                string s = Entry.Trim();
                if (s.EndsWith("="))
                {
                    s = s.Substring(0, s.Length - 1).Trim();
                }
                return s;
            }
        }

        public LineEntry(string text = "")
        {
            Entry = text;
        }
    }
}
