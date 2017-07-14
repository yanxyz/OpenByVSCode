using System;
using System.Collections.Generic;

namespace OpenByVSCode
{
    class Options
    {
        public string VSCode = "code";
        public string[] Folders = new[] { ".vscode", ".git" };
        public string[] Paths;
        public bool Help = false;
        public bool Version = false;
        public bool Editing = false;
        public string CodeArgs = "";

        public Options(string[] args)
        {
            if (args.Length == 0)
            {
                Help = true;
                return;
            }

            var data = Merge(
                new INIFile(Utils.iniFileName).Read(),
                ParseArgs(args)
                );

            // Add a "" for joing string easy
            var codeArgs = new List<string> { "" };

            foreach (var kvp in data)
            {
                var key = kvp.Key;
                var value = kvp.Value;

                switch (key)
                {
                    case "vscode":
                        if (value != String.Empty)
                            VSCode = value;
                        break;
                    case "folders":
                        if (value != String.Empty)
                        {
                            var parts = value.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            if (parts != null) Folders = parts;
                        }
                        break;
                    case "help":
                        Help = true;
                        break;
                    case "version":
                        Version = true;
                        break;
                    case "edit":
                        Editing = true;
                        break;
                    default:
                        var str = value == String.Empty ? $"--{key}" : $"--{key}=\"{value}\"";
                        codeArgs.Add(str);
                        break;
                }
            }

            CodeArgs = String.Join(" ", codeArgs);
        }

        private Dictionary<string, string> ParseArgs(string[] args)
        {
            if (args.Length == 0) return null;
            var d = new Dictionary<string, string>();
            var list = new List<string>();

            foreach (var item in args)
            {
                if (item.StartsWith("--"))
                {
                    if (item.Length == 2) continue;
                    // long option value must use '='
                    var parts = item.Substring(2).Split(new[] { '=' });
                    d[parts[0]] = parts.Length > 1 ? parts[1] : "";
                }
                else if (item.StartsWith("-"))
                {
                    if (item.Length != 2) continue;
                    var c = item[1];
                    if (c == 'h')
                        d["help"] = "";
                    else if (c == 'v')
                        d["version"] = "";
                    else if (c == 'e')
                        d["edit"] = "";
                    // ignore other short option
                    continue;
                }
                else
                {
                    // Add quotes back
                    list.Add(Utils.Quote(item));
                }
            }

            Paths = list.ToArray();

            return d;
        }

        private static Dictionary<string, string> Merge(params Dictionary<string, string>[] list)
        {
            var d = new Dictionary<string, string>();

            foreach (var dict in list)
            {
                if (dict == null) continue;
                foreach (var kvp in dict)
                {
                    var key = kvp.Key;
                    var value = kvp.Value;
                    d[key] = value;
                }
            }

            return d;
        }
    }
}
