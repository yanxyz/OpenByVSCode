using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenByVSCode
{
    public class Options
    {
        public string VSCode = @"C:\Program Files (x86)\Microsoft VS Code\Code.exe";
        public string[] Folders = { ".vscode", ".git" };
        public string[] Paths;
        public string CodeArgs = "";
        public bool Help = false;
        public bool Version = false;
        public byte MaxItems = 5;
        public string Project;

        public Options(string[] args)
        {
            if (args.Length == 0)
            {
                Help = true;
                return;
            }

            // step 1
            var myIni = new IniFile();
            var iniData = myIni.Read();
            // step 2
            var opts = ParseArgs(args);
            // step 3
            var merged = iniData == null ? opts : Merge(new[] { iniData, opts });
            // step 4
            var codeArgs = new List<string>();
            
            foreach (var kvp in merged)
            {
                var key = kvp.Key;
                var value = kvp.Value;

                switch (key)
                {
                    case "vscode":
                        if (!String.IsNullOrEmpty(value))
                            VSCode = value.Trim('"'); // remove quotes
                        break;
                    case "folders":
                        if (!String.IsNullOrEmpty(value))
                            Folders = ParseList(value.Trim('"'));
                        break;
                    case "max-items":
                        byte min = 2;
                        byte byteValue;
                        if (Byte.TryParse(value, out byteValue))
                            MaxItems = byteValue < min ? min : byteValue;
                        break;
                    case "project":
                        if (!String.IsNullOrEmpty(value))
                            Project = value;
                        break;
                    case "help":
                        Help = true;
                        break;
                    case "version":
                        Version = true;
                        break;
                    default:
                        var str = value == "true" ? $"--{key}" : $"--{key}={value}";
                        codeArgs.Add(str);
                        break;
                }
            }

            if (codeArgs.Count > 0)
                CodeArgs = String.Join(" ", codeArgs);
        }

        public static Dictionary<TKey, TValue> Merge<TKey, TValue>(IEnumerable<Dictionary<TKey, TValue>> dictionaries)
        {
            var result = new Dictionary<TKey, TValue>();
            foreach (var dict in dictionaries)
                foreach (var kvp in dict)
                    result[kvp.Key] = kvp.Value;
            return result;
        }

        private Dictionary<string, string> ParseArgs(string[] args)
        {
            var opts = new Dictionary<string, string>();
            var paths = new List<string>();

            for (int i = 0; i < args.Length; i++)
            {
                var item = args[i];

                if (item == "-h")
                {
                    opts.Add("help", "true");
                    continue;
                }

                if (item == "-v")
                {
                    opts.Add("version", "true");
                    continue;
                }

                if (item.Length > 2 && item.StartsWith("--"))
                {
                    var kvp = GetKeyValuePair(item);
                    opts.Add(kvp.Key, kvp.Value);
                    continue;
                }

                // ignore short options
                if (item.StartsWith("-") || item == "--")
                {
                    continue;
                }

                paths.Add(item);
            }

            Paths = paths.ToArray();
            return opts;
        }

        private KeyValuePair<string, string> GetKeyValuePair(string arg)
        {
            string key, value;
            var i = arg.IndexOf('=');
            if (i == -1)
            {
                key = arg.Substring(2);
                value = "true";
            }
            else
            {
                key = arg.Substring(2, i - 3);
                value = arg.Substring(i + 1);
            }

            var kvp = new KeyValuePair<string, string>(key, value);
            return kvp;
        }

        private string[] ParseList(string arg)
        {
            var chars = new[] { ',', ' ' };
            var list = arg.Split(chars, StringSplitOptions.RemoveEmptyEntries);
            return list;
        }
    }
}
