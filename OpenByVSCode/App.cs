using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace OpenByVSCode
{
    static class App
    {
        public static readonly Assembly Reference = typeof(App).Assembly;
        public static readonly string Location = Reference.Location;
        public static readonly string Root = Path.GetDirectoryName(Location);
        public static readonly string Name = Reference.GetName().Name;
        public static readonly Version Version = Reference.GetName().Version;
        
        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        private static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);

        public static int ShowError(string msg, string caption = null)
        {
            return MessageBox(new IntPtr(0), msg, caption ?? Name, 0x0|0x10);
        }

        public static void ShowHelp()
        {
            //var file = Path.Combine(Root, "Usage.txt");
            //using (StreamReader sr = new StreamReader(file))
            //{
            //    String line = sr.ReadToEnd();
            //    Console.WriteLine(line);
            //}

            var resourceName = "OpenByVSCode.Usage.txt";

            using (Stream stream = Reference.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                string result = reader.ReadToEnd();
                Console.Write(result);
            }
        }

        public static void ShowVersion()
        {
            var ver = Version;
            Console.WriteLine(ver);
        }

        public static bool IsDirectory(string path)
        {
            try
            {
                var attr = File.GetAttributes(path);
                return attr.HasFlag(FileAttributes.Directory);
            }
            catch
            {
                return false;
            }
        }
    }
}
