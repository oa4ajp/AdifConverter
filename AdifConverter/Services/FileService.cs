using AdifConverter.Models;
using AdifConverter.Services.Interfaces;
using AdifConverter.Strategy;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static AdifConverter.Enums.Enums;

namespace AdifConverter.Services
{
    public class FileService : IFileService
    {        
        private ICSVService _csvService;
        private IOpenXmlService _openXmlService;

        private IFileBuilderStrategy fileBuilderStrategy = null;

        public FileService(ICSVService csvService, IOpenXmlService openXmlService)
        {
            _csvService = csvService;
            _openXmlService = openXmlService;
        }

        private IFileBuilderStrategy GetFileBuilderOption(FileType fileType)
        {
            IFileBuilderStrategy fileBuilderStrategy = null;

            switch (fileType)
            {
                case FileType.Csv:
                    fileBuilderStrategy = new FileCsvBuilder(_csvService);
                    break;
                case FileType.PlanillaCsv:
                    fileBuilderStrategy = new FilePlanillaCsvBuilder(_csvService);
                    break;
                case FileType.Xlsx:
                    fileBuilderStrategy = new FileXlsxBuilder(_openXmlService);
                    break;
                default:
                    break;
            }
            return fileBuilderStrategy;
        }

        public void SaveFile(ObservableCollection<ADIFRecord> records, string fullFileName, FileType fileType)
        {
            if (!records.Any())
            {
                MessageBox.Show("No records found", Properties.Resources.ApplicationName);
                return;
            }

            fileBuilderStrategy = GetFileBuilderOption(fileType);
            fileBuilderStrategy.SaveFile(records, fullFileName);                
        }
    }
}
