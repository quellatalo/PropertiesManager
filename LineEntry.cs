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
        public LineEntry(string text = "")
        {
            Entry = text;
        }
    }
}
