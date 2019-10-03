namespace Quellatalo.Nin.PropertiesManager
{
    internal class LineEntry
    {
        public PropFileManager Manager { get; set; }
        public string Entry { get; set; }

        public LineEntry(PropFileManager manager, string text = null)
        {
            Manager = manager;
            Entry = text ?? string.Empty;
        }
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
                if (s.EndsWith(Manager.Separator))
                {
                    s = s.Substring(0, s.Length - 1).Trim();
                }
                return s;
            }
        }
    }
}
