using AdifConverter.Models;
using AdifConverter.Services.Interfaces;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdifConverter.Services
{
    class FileCSVService : FileServiceBase, IFileServiceStrategy
    {
        public void Save(ObservableCollection<ADIFRecord> adifRecords, string filePath)
        {
            if (!ValidateRecords(adifRecords)) return;

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Csv file (*.csv)|*.csv",
                FileName = GetFileName(filePath)
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                SaveFile(adifRecords, saveFileDialog.FileName);
                ShowSaveConfirmation(saveFileDialog.SafeFileName);
            }
            saveFileDialog = null;           
        }


        public void SaveFile(ObservableCollection<ADIFRecord> adifRecords, string filePath)
        {
            if (!adifRecords.Any()) return;

            var stringBuilderList = new List<StringBuilder>();

            var csvHeader = new StringBuilder();

            var firstRecord = adifRecords[0];

            BuildHeader(firstRecord, csvHeader);
            stringBuilderList.Add(csvHeader);

            foreach (var adifRecord in adifRecords)
            {
                var csvRow = new StringBuilder();
                BuildRow(adifRecord, csvRow);
                stringBuilderList.Add(csvRow);
            }

            using (var w = new StreamWriter(filePath))
            {

                foreach (var sb in stringBuilderList)
                {
                    w.WriteLine(sb.ToString());
                }
                w.Flush();
            }
        }

    }
}
