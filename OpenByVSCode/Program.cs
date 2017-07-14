using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace OpenByVSCode
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Start(args);
            }
            catch (AppException e)
            {
                App.ShowError(e.Message);
            }
        }

        private static void Start(string[] args)
        {
            var options = new Options(args);

            if (options.Help)
            {
                App.ShowHelp();
                return;
            }

            if (options.Version)
            {
                App.ShowVersion();
                return;
            }

            var paths = options.Paths;
            if (paths.Length == 0)
            {
                App.ShowHelp();
                return;
            }

            // validate 'vscode' option
            var vscode = options.VSCode;
            if (!File.Exists(vscode))
                throw new AppException($"Cannot find VSCode\n{vscode}");

            // if paths[0] is a directory
            // ignore 'project' option
            // if it is included in 'folders' option then find workspace
            // otherwise open it
            var firstItem = paths[0];
            if (App.IsDirectory(firstItem))
            {
                var di = new DirectoryInfo(firstItem);
                var name = di.Name;
                if (options.Folders.Contains(name))
                {
                    var parent = di.Parent;
                    if (parent == null)
                        throw new AppException("Should not open root path");
                    else
                        firstItem = parent.FullName;
                }

                var firstArguments = $"\"{firstItem}\" {options.CodeArgs}";
                System.Diagnostics.Process.Start(vscode, firstArguments);
                return;
            }

            if (paths.Length > options.MaxItems)
                throw new AppException("Open too many files!");

            // set workspace
            string workspace;
            var project = options.Project;
            if (String.IsNullOrEmpty(project))
                workspace = FindUp(options.Folders);
            else
            {
                if (App.IsDirectory(project))
                    workspace = project;
                else
                    throw new AppException($"the 'project' option is not a directory:\nproject={project}");
            }

            // compose code arguments
            var arguments = "\"" + String.Join("\" \"", paths) + "\"";
            arguments += " " + options.CodeArgs;
            if (!String.IsNullOrEmpty(workspace)) arguments = $"\"{workspace}\" {arguments}";

            // start code
            System.Diagnostics.Process.Start(vscode, arguments);
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
                if (directoryInfo == null) return null;
                parent = directoryInfo.FullName;
            }

            return null;
        }
    }
}
