using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Quellatalo.Nin.PropertiesManager
{
    /// <summary>
    /// Manages a properties file.
    /// </summary>
    public class PropFileManager
    {
        private static readonly string INVALID_KEY_EXCEPTION_DESCRIPTION = "A key should not have '=' nor NewLine character in it, and should not start with '#'.";
        private static readonly string LINE_ENTRY_EXCEPTION_DESCRIPTION = "An entry should not contain a NewLine character.";
        private IDictionary<string, string> properties = new Dictionary<string, string>();
        private char[] separator = { '=' };
        private List<LineEntry> entries = new List<LineEntry>();
        /// <summary>
        /// Key-value separator character.
        /// </summary>
        public char Separator { get { return separator[0]; } set { separator[0] = value; } }
        /// <summary>
        /// File encoding.
        /// </summary>
        public Encoding Encoding { get; set; }
        /// <summary>
        /// Path to file.
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// NewLine character in this properties file.
        /// </summary>
        public string NewLine { get; set; }
        /// <summary>
        /// Constructs a PropertiesFileManager and load data from the target file..
        /// </summary>
        /// <param name="filePath">Target file.</param>
        /// <param name="encoding">Encoding. (null means default)</param>
        /// <param name="newLine">NewLine character in this properties file. (null means environment's NewLine)</param>
        /// <param name="separator">Key-value separator character.</param>
        public PropFileManager(string filePath, Encoding encoding = null, string newLine = null, char separator = '=')
        {
            FilePath = filePath;
            NewLine = newLine ?? Environment.NewLine;
            Encoding = encoding ?? Encoding.Default;
            Separator = separator;
            if (!File.Exists(filePath))
            {
                using (File.Create(filePath)) { }
            }
        }
        /// <summary>
        /// Gets or Sets a line entry.
        /// </summary>
        /// <param name="entryIndex">Zero-based line index.</param>
        /// <returns>Returns the entry at the specified line.</returns>
        public string this[int entryIndex]
        {
            get { return GetLineEntry(entryIndex); }
            set
            {
                SetLineEntry(entryIndex, value);
            }
        }

        /// <summary>
        /// Gets a line entry.
        /// </summary>
        /// <param name="entryIndex">Zero-based line index.</param>
        /// <returns>Returns the entry at the specified line.</returns>
        public string GetLineEntry(int entryIndex)
        {
            string rs;
            if (entries[entryIndex].IsProperty)
            {
                string key = entries[entryIndex].Key;
                rs = entries[entryIndex].Entry + (properties.ContainsKey(key) ? properties[key] : "");
            }
            else
            {
                rs = entries[entryIndex].Entry;
            }
            return rs;
        }

        /// <summary>
        /// Modifies a line entry.
        /// </summary>
        /// <param name="entryIndex">Zero-based line index.</param>
        /// <param name="text">The new text to be replaced over.</param>
        public void SetLineEntry(int entryIndex, string text = "")
        {
            checkLineEntryValid(text);
            string trimstart = text.TrimStart();
            if (trimstart.Length == 0 || trimstart.StartsWith("#"))
            {
                entries[entryIndex].Entry = text;
            }
            else
            {
                string v = "";
                string[] prop = text.Split(separator, 2);
                if (prop.Length > 1)
                {
                    v = prop[1].Trim();
                    int valueIndex = Util.IndexOfNonWhitespace(prop[1]);
                    entries[entryIndex].Entry = prop[0] + "=" + prop[1].Substring(0, valueIndex == -1 ? 0 : valueIndex);
                }
                else
                {
                    entries[entryIndex].Entry = prop[0];
                }
                properties[prop[0].Trim()] = v;
            }
        }

        /// <summary>
        /// Gets the number of lines.
        /// </summary>
        public int LineCount { get { return entries.Count; } }

        /// <summary>
        /// Adds a new line of text to the file.
        /// </summary>
        /// <param name="text">The text to be added.</param>
        public void AddLineEntry(string text = "")
        {
            checkLineEntryValid(text);
            string trimstart = text.TrimStart();
            if (trimstart.Length == 0 || trimstart.StartsWith("#"))
            {
                entries.Add(new LineEntry(text));
            }
            else
            {
                string value = "";
                string[] prop = text.Split(separator, 2);
                if (prop.Length > 1)
                {
                    value = prop[1].Trim();
                    int valueIndex = Util.IndexOfNonWhitespace(prop[1]);
                    entries.Add(new LineEntry(prop[0] + "=" + prop[1].Substring(0, valueIndex == -1 ? 0 : valueIndex)));
                }
                else
                {
                    entries.Add(new LineEntry(prop[0]));
                }
                properties[prop[0].Trim()] = value;
            }
        }

        /// <summary>
        /// Inserts a new line of text into the specified zero-based index.
        /// </summary>
        /// <param name="index">The zero-based index to be inserted.</param>
        /// <param name="text">The text to be inserted.</param>
        public void InsertLineEntry(int index = 0, string text = "")
        {
            checkLineEntryValid(text);
            string trimstart = text.TrimStart();
            if (trimstart.Length == 0 || trimstart.StartsWith("#"))
            {
                entries.Insert(index, new LineEntry(text));
            }
            else
            {
                string value = "";
                string[] prop = text.Split('=');
                if (prop.Length > 1)
                {
                    value = prop[1].Trim();
                    int valueIndex = Util.IndexOfNonWhitespace(prop[1]);
                    entries.Insert(index, new LineEntry(prop[0] + "=" + prop[1].Substring(0, valueIndex == -1 ? 0 : valueIndex)));
                }
                else
                {
                    entries.Insert(index, new LineEntry(prop[0]));
                }
                properties[prop[0].Trim()] = value;
            }
        }

        /// <summary>
        /// Removes the line at the specified zero-based index.
        /// </summary>
        /// <param name="index">The zero-based index of the line to be removed.</param>
        public void RemoveLineEntryAt(int index)
        {
            entries.RemoveAt(index);
        }
        /// <summary>
        /// Gets or Sets a property.
        /// If a new property is set, a new line entry would also be added.
        /// </summary>
        /// <param name="key">Property key.</param>
        /// <returns>The value of the property.</returns>
        public string this[string key]
        {
            get
            {
                checkKeyValid(key);
                key = key.Trim();
                string rs = null;
                if (properties.ContainsKey(key))
                    rs = properties[key];
                return rs;
            }
            set { SetProperty(key, value); }
        }
        /// <summary>
        /// Gets the line zero-based index of a specified key.
        /// (Only return the first occurrence.)
        /// Not found return value is -1.
        /// </summary>
        /// <param name="key">Property key.</param>
        /// <returns>Only return the first occurrence. Not found return value is -1.</returns>
        public int GetPropertyEntryIndex(string key)
        {
            checkKeyValid(key);
            key = key.Trim();
            bool found = false;
            int i;
            for (i = 0; i < entries.Count; i++)
            {
                if (entries[i].IsProperty)
                {
                    if (entries[i].Key.Equals(key))
                    {
                        found = true;
                        break;
                    }
                }
            }
            if (!found) i = -1;
            return i;
        }

        /// <summary>
        /// Sets a property.
        /// If a new property is set, a new line entry would also be added.
        /// </summary>
        /// <param name="key">Property key.</param>
        /// <param name="value">Property value.</param>
        public void SetProperty(string key, object value)
        {
            SetProperty(key, value.ToString());
        }
        /// <summary>
        /// Sets a property.
        /// If a new property is set, a new line entry would also be added.
        /// </summary>
        /// <param name="key">Property key.</param>
        /// <param name="value">Property value.</param>
        public void SetProperty(string key, string value)
        {
            checkKeyValid(key);
            checkLineEntryValid(value);
            properties[key.Trim()] = value.Trim();
            if (GetPropertyEntryIndex(key) == -1)
            {
                entries.Add(new LineEntry(key + "="));
            }
        }

        /// <summary>
        /// Check whether a property exists.
        /// </summary>
        /// <param name="key">Property key.</param>
        /// <returns>Is exist.</returns>
        public bool HasProperty(string key)
        {
            return properties.ContainsKey(key);
        }
        /// <summary>
        /// Clears all entries and properties.
        /// </summary>
        public void ClearAll()
        {
            entries.Clear();
            properties.Clear();
        }
        /// <summary>
        /// Loads data from the properties file.
        /// </summary>
        public void Load()
        {
            ClearAll();
            string[] lines = File.ReadAllLines(FilePath, Encoding);
            foreach (string line in lines)
            {
                AddLineEntry(line);
            }
        }

        /// <summary>
        /// Gets full text output.
        /// </summary>
        [Obsolete("FullText will be removed in further releases. Please use GetFullText method instead. ")]
        public string FullText
        {
            get
            {
                return GetFullText();
            }
        }

        /// <summary>
        /// Gets full text output.
        /// </summary>
        /// <returns>The full text output of the config file.</returns>
        public string GetFullText()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (LineEntry entry in entries)
            {
                if (entry.IsProperty)
                {
                    string key = entry.Key;
                    stringBuilder.Append(entry.Entry).Append(properties.ContainsKey(key) ? properties[key] : "").Append(NewLine);
                }
                else
                {
                    stringBuilder.Append(entry.Entry).Append(NewLine);
                }
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Gets all lines as List.
        /// </summary>
        /// <returns>All lines in order.</returns>
        public List<string> GetAllLines()
        {
            List<string> lines = new List<string>();
            foreach (LineEntry entry in entries)
            {
                if (entry.IsProperty)
                {
                    string key = entry.Key;
                    lines.Add(entry.Entry + (properties.ContainsKey(key) ? properties[key] : ""));
                }
                else
                {
                    lines.Add(entry.Entry);
                }
            }
            return lines;
        }

        /// <summary>
        /// Saves full text to file.
        /// </summary>
        public void Save()
        {
            using (FileStream fs = File.Create(FilePath))
            {
                string data = GetFullText();
                fs.Write(Encoding.GetBytes(data), 0, data.Length);
                fs.Flush();
            }
        }
        /// <summary>
        /// Get a property value as short.
        /// </summary>
        /// <param name="key">Property key.</param>
        /// <returns>The value of the property as short.</returns>
        public short GetShort(string key)
        {
            short.TryParse(this[key], out short rs);
            return rs;
        }
        /// <summary>
        /// Gets a property value as ushort.
        /// </summary>
        /// <param name="key">Property key.</param>
        /// <returns>The value of the property as ushort.</returns>
        public ushort GetUnsignedShort(string key)
        {
            ushort.TryParse(this[key], out ushort rs);
            return rs;
        }
        /// <summary>
        /// Get a property value as int.
        /// </summary>
        /// <param name="key">Property key.</param>
        /// <returns>The value of the property as int.</returns>
        public int GetInt(string key)
        {
            int.TryParse(this[key], out int rs);
            return rs;
        }
        /// <summary>
        /// Get a property value as uint.
        /// </summary>
        /// <param name="key">Property key.</param>
        /// <returns>The value of the property as uint.</returns>
        public uint GetUnsignedInt(string key)
        {
            uint.TryParse(this[key], out uint rs);
            return rs;
        }
        /// <summary>
        /// Gets a property value as long.
        /// </summary>
        /// <param name="key">Property key.</param>
        /// <returns>The value of the property as long.</returns>
        public long GetLong(string key)
        {
            long.TryParse(this[key], out long rs);
            return rs;
        }
        /// <summary>
        /// Gets a property value as long.
        /// </summary>
        /// <param name="key">Property key.</param>
        /// <returns>The value of the property as ulong.</returns>
        public ulong GetUnsignedLong(string key)
        {
            ulong.TryParse(this[key], out ulong rs);
            return rs;
        }
        /// <summary>
        /// Gets a property value as float.
        /// </summary>
        /// <param name="key">Property key.</param>
        /// <returns>The value of the property as float.</returns>
        public float GetFloat(string key)
        {
            if (!float.TryParse(this[key], out float rs))
            {
                rs = float.NaN;
            }
            return rs;
        }
        /// <summary>
        /// Gets a property value as double.
        /// </summary>
        /// <param name="key">Property key.</param>
        /// <returns>The value of the property as double.</returns>
        public double GetDouble(string key)
        {
            if (!double.TryParse(this[key], out double rs))
            {
                rs = double.NaN;
            }
            return rs;
        }
        /// <summary>
        /// Gets a property value as bool.
        /// </summary>
        /// <param name="key">Property key.</param>
        /// <returns>The value of the property as bool.</returns>
        public bool GetBool(string key)
        {
            bool.TryParse(this[key], out bool rs);
            return rs;
        }
        /// <summary>
        /// Gets a property value as string.
        /// </summary>
        /// <param name="key">Property key.</param>
        /// <returns>The value of the property as string.</returns>
        public string GetString(string key)
        {
            return this[key];
        }

        /// <summary>
        /// Gets a property value as char.
        /// </summary>
        /// <param name="key">Property key.</param>
        /// <returns>The value of the property as char.</returns>
        public char GetChar(string key)
        {
            char.TryParse(this[key], out char rs);
            return rs;
        }

        /// <summary>
        /// Gets a property value as byte.
        /// </summary>
        /// <param name="key">Property key.</param>
        /// <returns>The value of the property as byte.</returns>
        public byte GetByte(string key)
        {
            byte.TryParse(this[key], out byte rs);
            return rs;
        }

        /// <summary>
        /// Gets a property value as DateTime.
        /// </summary>
        /// <param name="key">Property Key.</param>
        /// <returns>The value of the property as DateTime.</returns>
        public DateTime GetDateTime(string key)
        {
            DateTime.TryParse(this[key], out DateTime rs);
            return rs;
        }

        private void checkKeyValid(string key)
        {
            if (key.Contains("=") || key.TrimStart().StartsWith("#") || key.Contains(NewLine)) throw newInvalidKeyException();
        }

        private void checkLineEntryValid(string text)
        {
            if (text.Contains(NewLine)) throw newLineEntryException();
        }

        private static ArgumentOutOfRangeException newLineEntryException() { return new ArgumentOutOfRangeException(LINE_ENTRY_EXCEPTION_DESCRIPTION); }

        private static ArgumentException newInvalidKeyException() { return new ArgumentException(INVALID_KEY_EXCEPTION_DESCRIPTION); }

        /// <summary>
        /// Check whether a line of text is a valid property.
        /// </summary>
        /// <param name="line">The line to be checked.</param>
        /// <returns>Whether the line is a valid property.</returns>
        public static bool IsPropertyLine(String line)
        {
            string trimStart = line.TrimStart();
            return trimStart.Length > 0 && !trimStart.StartsWith("#");
        }
    }
}
