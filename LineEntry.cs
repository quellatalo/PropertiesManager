namespace Quellatalo.Nin.PropertiesManager
{
    internal class LineEntry
    {
        public string Entry { get; set; }
        public bool IsProperty
        {
            get
            {
                string trimStart = Entry.TrimStart();
                return trimStart.Length > 0 && !trimStart.StartsWith("#");
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
