using System.Linq;
using System.Windows;

namespace AdifConverter.Strategy
{
    public abstract class FileBuilder
    {
        public string GetFileName(string fullFileName)
        {
            string[] separatingChar = { "." };
            string fileName = string.Empty;

            var fullFileNameArray = fullFileName.Split(separatingChar, System.StringSplitOptions.RemoveEmptyEntries);

            if (fullFileNameArray.Any())
                fileName = fullFileNameArray[0];

            return fileName;
        }

        public void ShowSaveConfirmation(string fileName)
        {
            MessageBox.Show($"{fileName} saved.", Properties.Resources.ApplicationName);
        }
    }
}
