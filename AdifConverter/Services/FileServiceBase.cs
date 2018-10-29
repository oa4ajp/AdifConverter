using AdifConverter.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AdifConverter.Services
{
    public abstract class FileServiceBase
    {
        private readonly string _delimiter = ";";

        public void BuildHeader(ADIFRecord adifRecord, StringBuilder csvRows)
        {
            int totalfields = adifRecord.Fields.Count;
            int fieldCounter = 1;

            foreach (var adifField in adifRecord.Fields)
            {
                if (fieldCounter > 1)
                    AppendRows(csvRows, adifField.Name, fieldCounter, totalfields);
                fieldCounter++;
            }
        }

        public void BuildRow(ADIFRecord adifRecord, StringBuilder csvRows)
        {
            int totalfields = adifRecord.Fields.Count;
            int fieldCounter = 1;

            foreach (var adifField in adifRecord.Fields)
            {
                if (fieldCounter > 1)
                    AppendRows(csvRows, adifField.Value, fieldCounter, totalfields);

                fieldCounter++;
            }
        }

        public void AppendRows(StringBuilder csvRows, string fieldNameOrValue, int fieldCounter, int totalFields)
        {
            var adifFieldHeader = $"\"{fieldNameOrValue}\"" + (fieldCounter < totalFields ? _delimiter : "");
            csvRows.Append(adifFieldHeader);
        }

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

        public bool ValidateRecords(ObservableCollection<ADIFRecord> adifRecords)
        {
            var result = true;
            if (!adifRecords.Any())
            {
                MessageBox.Show("No records found", Properties.Resources.ApplicationName);
                result = false;
            }

            return result;
        }

    }
}
