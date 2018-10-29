using AdifConverter.Models;

using System.Collections.ObjectModel;
using System.Windows.Controls;
using AdifConverter.Commands;
using System.Windows.Input;
using AdifConverter.Services.Interfaces;
using static AdifConverter.Enums.Enums;

namespace AdifConverter.ViewModels
{
    public class ADIFRecordViewModel
    {    
        private IDataGridService _dataGridService;        
        private IADIFRecordService _adifRecordService;

        private IFileServiceStrategy _fileCSVService;
        private IFileServiceStrategy _fileCSVPlanillaService;
        private IFileServiceStrategy _fileOpenXmlService;

        private IFileServiceStrategy fileServiceStrategy;

        private readonly DelegateCommand _saveCsvCommand;
        public ICommand SaveCsvCommand => _saveCsvCommand;

        private readonly DelegateCommand _saveXlsxCommand;
        public ICommand SaveXlsxCommand => _saveXlsxCommand;

        private readonly DelegateCommand _savePlanillaCsvCommand;
        public ICommand SavePlanillaCsvCommand => _savePlanillaCsvCommand;

        public ObservableCollection<ADIFRecord> Records { get; set; } = new ObservableCollection<ADIFRecord>();

        public string CurrentOpenedFileName { get; set; } = string.Empty;

        public Grid MainGrid { get; set; }

        public ADIFRecordViewModel
        (
            IDataGridService dataGridService, 
            IADIFRecordService adifRecordService,
            IFileServiceStrategy fileCSVService,
            IFileServiceStrategy fileCSVPlanillaService,
            IFileServiceStrategy fileOpenXmlService
        )
        {
            _dataGridService = dataGridService;
            _adifRecordService = adifRecordService;      
            _fileCSVService = fileCSVService;
            _fileCSVPlanillaService = fileCSVPlanillaService;
            _fileOpenXmlService = fileOpenXmlService;

            _saveCsvCommand = new DelegateCommand(OnSaveCsv);
            _saveXlsxCommand = new DelegateCommand(OnSaveXlsx);
            _savePlanillaCsvCommand = new DelegateCommand(OnSavePlanillaCsv);             
        }

        private IFileServiceStrategy GetFileServiceOption(FileType fileType)
        {
            IFileServiceStrategy fileServiceStrategy = null;

            switch (fileType)
            {
                case FileType.Csv:
                    fileServiceStrategy = _fileCSVService;
                    break;
                case FileType.PlanillaCsv:
                    fileServiceStrategy = _fileCSVPlanillaService;
                    break;
                case FileType.Xlsx:
                    fileServiceStrategy = _fileOpenXmlService;
                    break;
                default:
                    break;
            }
            return fileServiceStrategy;
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
            fileServiceStrategy = GetFileServiceOption(FileType.Csv);
            fileServiceStrategy.Save(Records, CurrentOpenedFileName);
          
            MainGrid.InvalidateVisual(); //Refresh Grid container to clean pop up sticky for overwire confirmation.
        }

        private void OnSavePlanillaCsv(object commandParameter)
        {
            fileServiceStrategy = GetFileServiceOption(FileType.PlanillaCsv);
            fileServiceStrategy.Save(Records, CurrentOpenedFileName);

            MainGrid.InvalidateVisual();
        }

        private void OnSaveXlsx(object commandParameter)
        {
            fileServiceStrategy = GetFileServiceOption(FileType.Xlsx);
            fileServiceStrategy.Save(Records, CurrentOpenedFileName);

            MainGrid.InvalidateVisual();
        }

    }
}
