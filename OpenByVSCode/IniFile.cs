using System;
using System.Collections.Generic;
using System.IO;

namespace OpenByVSCode
{
    class IniFile
    {
        public string FilePath { get; private set; }

        public IniFile(string IniFilePath = null)
        {
            if (IniFilePath == null)
                FilePath = Path.Combine(App.Root, App.Name + ".ini");
            else
                FilePath = new FileInfo(IniFilePath).FullName.ToString();
        }

        public Dictionary<string, string> Read()
        {
            if (!File.Exists(FilePath)) return null;

            using (FileStream fs = File.Open(FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    return Parse(sr.ReadToEnd());
                }
            }
        }

        public Dictionary<string, string> Parse(string dataString)
        {
            var data = new Dictionary<string, string>();
            var lines = dataString.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            for (int lineNumber = 0; lineNumber < lines.Length; lineNumber++)
            {
                var line = lines[lineNumber].Trim();

                if (line == String.Empty || line.StartsWith(";") || !line.Contains("="))
                    continue;

                var kvp = GetKeyValuePair(line);
                data.Add(kvp.Key, kvp.Value);
            }
            return data;
        }

        public KeyValuePair<string, string> GetKeyValuePair(string dataString)
        {
            var arr = dataString.Split(new[] { '=' }, 2);
            var key = TransformName(arr[0].TrimEnd());
            var value = arr[1].TrimStart();
            var kvp = new KeyValuePair<string, string>(key, value);
            return kvp;
        }

        // PascalCase -> kebab-case
        public string TransformName(string name)
        {
            var sb = new System.Text.StringBuilder();

            var firstChar = name[0];
            sb.Append(char.IsUpper(firstChar) ? char.ToLower(firstChar) : firstChar);

            for (byte i = 1; i < name.Length; i++)
            {
                var c = name[i];

                if (char.IsUpper(c))
                {
                    sb.Append('-');
                    sb.Append(char.ToLower(c));
                }
                else
                    sb.Append(c);
            }
            return sb.ToString();
        }
    }
}
