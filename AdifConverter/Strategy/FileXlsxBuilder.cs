﻿using AdifConverter.Models;
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
    public class FileXlsxBuilder : FileBuilder, IFileBuilderStrategy
    {
        private IOpenXmlService _openXmlService;

        public FileXlsxBuilder(IOpenXmlService openXmlService)
        {
            _openXmlService = openXmlService;
        }

        public void SaveFile(ObservableCollection<ADIFRecord> records, string fullFileName)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Xlsx file (*.xlsx)|*.xlsx",
                FileName = GetFileName(fullFileName)
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                _openXmlService.GenerateXlsxFile(records, saveFileDialog.FileName, GetFileName(saveFileDialog.SafeFileName));                
                ShowSaveConfirmation(saveFileDialog.SafeFileName);
            }
            saveFileDialog = null;
        }

    }
}
