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
        private IFileBuilderStrategy fileBuilderStrategy = null;

        private IFileBuilderStrategy _fileCsvBuilder;
        private IFileBuilderStrategy _filePlanillaCsvBuilder;
        private IFileBuilderStrategy _fileXlsxBuilder;

        public FileService(IFileBuilderStrategy fileCsvBuilder, IFileBuilderStrategy filePlanillaCsvBuilder, IFileBuilderStrategy fileXlsxBuilder)
        {
            _fileCsvBuilder = fileCsvBuilder;
            _filePlanillaCsvBuilder = filePlanillaCsvBuilder;
            _fileXlsxBuilder = fileXlsxBuilder;
        }

        private IFileBuilderStrategy GetFileBuilderOption(FileType fileType)
        {
            IFileBuilderStrategy fileBuilderStrategy = null;

            switch (fileType)
            {
                case FileType.Csv:
                    fileBuilderStrategy = _fileCsvBuilder;
                    break;
                case FileType.PlanillaCsv:
                    fileBuilderStrategy = _filePlanillaCsvBuilder;
                    break;
                case FileType.Xlsx:
                    fileBuilderStrategy = _fileXlsxBuilder;
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
