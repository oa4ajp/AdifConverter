using AdifConverter.Services;
using AdifConverter.Exceptions;
using AdifConverter.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using AdifConverter.Commands;
using System.Windows.Input;
using Microsoft.Win32;
using AdifConverter.Services.Interfaces;
using static AdifConverter.Enums.Enums;

namespace AdifConverter.ViewModels
{
    public class ADIFRecordViewModel
    {    
        private IDataGridService _dataGridService;        
        private IADIFRecordService _adifRecordService;
        private IFileService _fileService;

        private readonly DelegateCommand _saveCsvCommand;
        public ICommand SaveCsvCommand => _saveCsvCommand;

        private readonly DelegateCommand _saveXlsxCommand;
        public ICommand SaveXlsxCommand => _saveXlsxCommand;

        private readonly DelegateCommand _savePlanillaCsvCommand;
        public ICommand SavePlanillaCsvCommand => _savePlanillaCsvCommand;

        public ObservableCollection<ADIFRecord> Records { get; set; } = new ObservableCollection<ADIFRecord>();

        public string CurrentOpenedFileName { get; set; } = string.Empty;

        public Grid MainGrid { get; set; }

        public ADIFRecordViewModel(IDataGridService dataGridService, IADIFRecordService adifRecordService, IFileService fileService)
        {
            _dataGridService = dataGridService;
            _adifRecordService = adifRecordService;
            _fileService = fileService;

            _saveCsvCommand = new DelegateCommand(OnSaveCsv);
            _saveXlsxCommand = new DelegateCommand(OnSaveXlsx);
            _savePlanillaCsvCommand = new DelegateCommand(OnSavePlanillaCsv);             
        }

        public void ReadRecords(string fileName)
        {
            Records = _adifRecordService.ReadRecords(fileName);
        }

        public void SetupGrid(DataGrid dataGrid)
        {
            _dataGridService.SetupGrid(dataGrid, Records);
        }

        public void OnSaveCsv(object commandParameter)
        {
            _fileService.SaveFile(Records, CurrentOpenedFileName, FileType.Csv);
            MainGrid.InvalidateVisual(); //Refresh Grid container to clean pop up sticky for overwire confirmation.
        }

        private void OnSaveXlsx(object commandParameter)
        {
            _fileService.SaveFile(Records, CurrentOpenedFileName, FileType.Xlsx);
            MainGrid.InvalidateVisual();
        }

        private void OnSavePlanillaCsv(object commandParameter)
        {
            _fileService.SaveFile(Records, CurrentOpenedFileName, FileType.PlanillaCsv);
            MainGrid.InvalidateVisual();
        }

    }
}
