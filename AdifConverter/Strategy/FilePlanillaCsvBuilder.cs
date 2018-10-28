using AdifConverter.Models;
using AdifConverter.Services.Interfaces;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AdifConverter.Strategy
{
    public class FilePlanillaCsvBuilder : FileBuilder, IFileBuilderStrategy
    {
        private ICSVService _csvService;

        public FilePlanillaCsvBuilder(ICSVService csvService)
        {
            _csvService = csvService;
        }

        public void SaveFile(ObservableCollection<ADIFRecord> records, string fullFileName)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Csv file (*.csv)|*.csv",
                FileName = GetFileName(fullFileName)
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                _csvService.SavePlanillaCsv(records, saveFileDialog.FileName);                
                ShowSaveConfirmation(saveFileDialog.SafeFileName);
            }
            saveFileDialog = null;
        }

    }
}
