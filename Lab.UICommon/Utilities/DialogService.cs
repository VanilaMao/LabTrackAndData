using System.IO;
using Microsoft.Win32;

namespace Lab.UICommon.Utilities
{
    public class DialogService :IDialogService
    {
        public static string Folder = "C:\\Documents";
        public string OpenDialogFile(string filter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Folder;
            openFileDialog.Filter = $"{filter} files(*.{filter})|*.{filter}|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                var filename  = openFileDialog.FileName;
                if (!string.IsNullOrEmpty(filename))
                {
                    Folder = Path.GetDirectoryName(filename);
                }              
                return filename;
            }
            
            return string.Empty;
        }

        public string SaveDialogFile(string filter)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Folder;
            saveFileDialog.Filter = $"{filter} files(*.{filter})|*.{filter}";
            if (saveFileDialog.ShowDialog() == true)
            {
                var filename = saveFileDialog.FileName;
                if (!string.IsNullOrEmpty(filename))
                {
                    Folder = Path.GetDirectoryName(filename);
                }
                return filename;
            }
            return string.Empty;
        }
    }
}
