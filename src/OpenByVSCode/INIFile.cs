using System;
using System.Collections.Generic;
using System.IO;

namespace OpenByVSCode
{
    public class INIFile
    {
        private string _fileName;

        public INIFile(string fileName) => _fileName = fileName;

        public Dictionary<string, string> Read()
        {
            if (!File.Exists(_fileName)) return null;

            var d = new Dictionary<string, string>();
            using (var sr = new StreamReader(_fileName))
            {
                while (sr.Peek() >= 0)
                {
                    var line = sr.ReadLine().Trim();
                    if (line == string.Empty) continue;
                    var c = line[0];
                    // There is only one section, so it could be ignored
                    if (c == '#' || c == ';' || c == '[') continue;

                    var parts = line.Split(new[] { '=' });
                    if (parts.Length != 2) continue;
                    var key = parts[0].Trim().Replace('_', '-');
                    var value = parts[1].Trim();
                    // remove quotes
                    var len = value.Length;
                    if (len > 1)
                    {
                        if (value[0] == '"' && value[len - 1] == '"')
                            value = value.Substring(1, len - 2);
                    }

                    d[key] = value;
                }
            }

            return d;
        }
    }
}
