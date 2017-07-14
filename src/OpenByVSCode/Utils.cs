using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace OpenByVSCode
{
    static class Utils
    {
        public static readonly Assembly App = typeof(Utils).Assembly;
        public static readonly string Name = App.GetName().Name;
        public static string Version
        {
            get
            {
                var version = App.GetName().Version;
                return version.Major + "." + version.Minor;
            }
        }
        public static readonly string ExecutablePath = App.Location;
        public static readonly string iniFileName = Path.ChangeExtension(ExecutablePath, ".ini");

        public static void Start(string[] args)
        {
            var options = new Options(args);

            if (options.Help)
            {
                ShowHelp();
                return;
            }

            if (options.Version)
            {
                ShowVersion();
                return;
            }

            if (options.Editing)
            {
                Edit(options);
                return;
            }

            var paths = options.Paths;
            if (paths.Length == 0)
            {
                ShowHelp();
                return;
            }

            // If paths[0] is a directory
            // if it is included in the 'folders' option then open its parent
            // otherwise open it
            var firstItem = paths[0];
            if (IsDirectory(firstItem))
            {
                var di = new DirectoryInfo(firstItem);
                var name = di.Name;
                if (options.Folders.Contains(name))
                {
                    var parent = di.Parent;
                    if (parent == null)
                        throw new Exception("Should not open root");
                    else
                        firstItem = parent.FullName;
                }

                StartCode(options.VSCode, Quote(firstItem), options.CodeArgs);
                return;
            }

            var arguments = String.Join(" ", paths);
            string workspace = FindUp(options.Folders);
            if (!String.IsNullOrEmpty(workspace))
                arguments = Quote(workspace) + " " + arguments;
            StartCode(options.VSCode, arguments, options.CodeArgs);
        }

        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        private static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);

        public static int ShowError(string msg, string caption = null)
        {
            return MessageBox(new IntPtr(0), msg, caption ?? Name, 0x0 | 0x10);
        }

        public static void ShowHelp()
        {
            Console.WriteLine(Name + " " + Version);
            Console.WriteLine();

            var resourceName = $"{Name}.Usage.txt";

            using (Stream stream = App.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                string result = reader.ReadToEnd();
                Console.Write(result);
            }
        }

        public static void ShowVersion()
        {
            Console.Write(Version);
        }

        private static bool IsDirectory(string path)
        {
            return Directory.Exists(path);
        }

        public static string Quote(string value)
        {
            if (value.IndexOf(' ') > -1)
                return "\"" + value + "\"";
            else
                return value;
        }

        // find folders from current directory
        private static string FindUp(string[] folders)
        {
            var parent = Directory.GetCurrentDirectory();
            while (!String.IsNullOrEmpty(parent))
            {
                var dirs = Directory.EnumerateDirectories(parent);
                foreach (var dir in dirs)
                {
                    var dirname = dir.Substring(dir.LastIndexOf("\\") + 1);
                    foreach (var name in folders)
                    {
                        if (dirname == name) return parent;
                    }
                }

                var directoryInfo = Directory.GetParent(parent);
                return directoryInfo?.FullName;
            }

            return null;
        }

        private static void StartCode(string code, string arguments, string codeArgs)
        {
            if (code.EndsWith(".exe"))
            {
                Process.Start(code, arguments + codeArgs);
                return;
            }

            var startInfo = new ProcessStartInfo()
            {
                FileName = "cmd.exe",
                Arguments = $"/c {Quote(code)} {arguments} {codeArgs}",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true
            };

            var proc = Process.Start(startInfo);
            var error = proc.StandardError.ReadToEnd();
            proc.WaitForExit();
            if (proc.ExitCode > 0)
            {
                var sb = new StringBuilder();
                sb.Append("cmd.exe ");
                sb.AppendLine(startInfo.Arguments);
                sb.AppendLine(error);
                throw new Exception(sb.ToString());
            }
        }

        private static void Edit(Options options)
        {
            if (!File.Exists(iniFileName))
            {
                var sb = new StringBuilder();
                sb.AppendLine($"[{Name}]");
                sb.AppendLine($"vscode={options.VSCode}");
                sb.AppendLine($"folders={String.Join(", ", options.Folders)}");

                using (var sw = new StreamWriter(iniFileName))
                {
                    sw.Write(sb.ToString());
                }
            }

            Process.Start(iniFileName);
        }
    }
}
