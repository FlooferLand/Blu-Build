#if UNITY_STANDALONE_WIN

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Ookii.Dialogs;

namespace SFB {
    // For fullscreen support
    // - WindowWrapper class and GetActiveWindow() are required for modal file dialog.
    // - "PlayerSettings/Visible In Background" should be enabled, otherwise when file dialog opened app window minimizes automatically.

    public class WindowWrapper : IWin32Window {
        public WindowWrapper(IntPtr handle) {
            Handle = handle;
        }

        public IntPtr Handle { get; }
    }

    public class StandaloneFileBrowserWindows : IStandaloneFileBrowser {
        public string[] OpenFilePanel(string title, string directory, ExtensionFilter[] extensions, bool multiselect) {
            var fd = new VistaOpenFileDialog();
            fd.Title = title;
            if (extensions != null) {
                fd.Filter = GetFilterFromFileExtensionList(extensions);
                fd.FilterIndex = 1;
            }
            else {
                fd.Filter = string.Empty;
            }

            fd.Multiselect = multiselect;
            if (!string.IsNullOrEmpty(directory)) fd.FileName = GetDirectoryPath(directory);
            var res = fd.ShowDialog(new WindowWrapper(GetActiveWindow()));
            string[] filenames = res == DialogResult.OK ? fd.FileNames : new string[0];
            fd.Dispose();
            return filenames;
        }

        public void OpenFilePanelAsync(string title, string directory, ExtensionFilter[] extensions, bool multiselect,
            Action<string[]> cb) {
            cb.Invoke(OpenFilePanel(title, directory, extensions, multiselect));
        }

        public string[] OpenFolderPanel(string title, string directory, bool multiselect) {
            var fd = new VistaFolderBrowserDialog();
            fd.Description = title;
            if (!string.IsNullOrEmpty(directory)) fd.SelectedPath = GetDirectoryPath(directory);
            var res = fd.ShowDialog(new WindowWrapper(GetActiveWindow()));
            string[] filenames = res == DialogResult.OK ? new[] { fd.SelectedPath } : new string[0];
            fd.Dispose();
            return filenames;
        }

        public void OpenFolderPanelAsync(string title, string directory, bool multiselect, Action<string[]> cb) {
            cb.Invoke(OpenFolderPanel(title, directory, multiselect));
        }

        public string SaveFilePanel(string title, string directory, string defaultName, ExtensionFilter[] extensions) {
            var fd = new VistaSaveFileDialog();
            fd.Title = title;

            string finalFilename = "";

            if (!string.IsNullOrEmpty(directory)) finalFilename = GetDirectoryPath(directory);

            if (!string.IsNullOrEmpty(defaultName)) finalFilename += defaultName;

            fd.FileName = finalFilename;
            if (extensions != null) {
                fd.Filter = GetFilterFromFileExtensionList(extensions);
                fd.FilterIndex = 1;
                fd.DefaultExt = extensions[0].Extensions[0];
                fd.AddExtension = true;
            }
            else {
                fd.DefaultExt = string.Empty;
                fd.Filter = string.Empty;
                fd.AddExtension = false;
            }

            var res = fd.ShowDialog(new WindowWrapper(GetActiveWindow()));
            string filename = res == DialogResult.OK ? fd.FileName : "";
            fd.Dispose();
            return filename;
        }

        public void SaveFilePanelAsync(string title, string directory, string defaultName, ExtensionFilter[] extensions,
            Action<string> cb) {
            cb.Invoke(SaveFilePanel(title, directory, defaultName, extensions));
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();

        // .NET Framework FileDialog Filter format
        // https://msdn.microsoft.com/en-us/library/microsoft.win32.filedialog.filter
        private static string GetFilterFromFileExtensionList(ExtensionFilter[] extensions) {
            string filterString = "";
            foreach (var filter in extensions) {
                filterString += filter.Name + "(";

                foreach (string ext in filter.Extensions) filterString += "*." + ext + ",";

                filterString = filterString.Remove(filterString.Length - 1);
                filterString += ") |";

                foreach (string ext in filter.Extensions) filterString += "*." + ext + "; ";

                filterString += "|";
            }

            filterString = filterString.Remove(filterString.Length - 1);
            return filterString;
        }

        private static string GetDirectoryPath(string directory) {
            string directoryPath = Path.GetFullPath(directory);
            if (!directoryPath.EndsWith("\\")) directoryPath += "\\";
            if (Path.GetPathRoot(directoryPath) == directoryPath) return directory;
            return Path.GetDirectoryName(directoryPath) + Path.DirectorySeparatorChar;
        }
    }
}

#endif