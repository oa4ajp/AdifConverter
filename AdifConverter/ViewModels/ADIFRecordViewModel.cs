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

namespace AdifConverter.ViewModels
{
    public class ADIFRecordViewModel
    {
        private ICSVService _csvService;
        private IOpenXmlService _openXmlService;
        private IDataGridService _dataGridService;
        private IADIFFieldService _adifFieldService;

        private readonly DelegateCommand _saveCsvCommand;
        public ICommand SaveCsvCommand => _saveCsvCommand;

        private readonly DelegateCommand _saveXlsxCommand;
        public ICommand SaveXlsxCommand => _saveXlsxCommand;

        private readonly DelegateCommand _savePlanillaCsvCommand;
        public ICommand SavePlanillaCsvCommand => _savePlanillaCsvCommand;

        public ObservableCollection<ADIFRecord> Records { get; set; } = new ObservableCollection<ADIFRecord>();

        public ADIFRecordViewModel(ICSVService csvService, IOpenXmlService openXmlService, IDataGridService dataGridService, IADIFFieldService adifFieldService)
        {
            _csvService = csvService;
            _openXmlService = openXmlService;
            _dataGridService = dataGridService;
            _adifFieldService = adifFieldService;

            _saveCsvCommand = new DelegateCommand(OnSaveCsv);
            _saveXlsxCommand = new DelegateCommand(OnSaveXlsx);
            _savePlanillaCsvCommand = new DelegateCommand(OnSavePlanillaCsv);             
        }

        public void ReadRecords(string fileName)
        {
            var adifRecords = new ObservableCollection<ADIFRecord>();

            var reader = new StreamReader(fileName, Encoding.UTF8);

            string[] separatingChar = { "<EOH>", "<eoh>" };

            //Remove LineBrake and Tabs characters
            string fileContent = reader.ReadToEnd().Replace("\n", "").Replace("\r", "").Replace("\t", "").Trim(); ;

            string[] contentArray = fileContent.Split(separatingChar, System.StringSplitOptions.RemoveEmptyEntries);

            MemoryStream mStrm = new MemoryStream(Encoding.UTF8.GetBytes(contentArray[1]));

            StreamReader streamReader = new StreamReader(mStrm);

            var record = new ADIFRecord() { Fields = new ObservableCollection<ADIFField>() { new ADIFField() { Name = "Line", Value = $"{adifRecords.Count + 1}" } } };

            try
            {
                for (; ; )
                {
                    var field = _adifFieldService.ParseField(streamReader);

                    if (field == null)
                        break;

                    if ("EOR".Equals(field.Name.ToUpper()))
                    {
                        var tempRecord = record.Clone() as ADIFRecord;

                        adifRecords.Add(tempRecord);

                        record = new ADIFRecord() { Fields = new ObservableCollection<ADIFField>() { new ADIFField() { Name = "Line", Value = $"{adifRecords.Count + 1 }" } } };
                    }
                    else
                    {
                        record.Fields.Add(field);
                    }
                }

            }
            catch (AdifException ae)
            {
                var message = ae.Message;
                MessageBox.Show($"{message} on row {adifRecords.Count + 1}.", Properties.Resources.ApplicationName, MessageBoxButton.OK, MessageBoxImage.Error);
                Records = new ObservableCollection<ADIFRecord>();
                return;
            }
            finally
            {
                streamReader.Dispose();
                mStrm.Dispose();
                reader.Dispose();
            }

            var processedRecords = ProcessRecords(adifRecords);

            Records = processedRecords;
        }

        public ObservableCollection<ADIFRecord> ProcessRecords(ObservableCollection<ADIFRecord> records)
        {
            //Sort the records by Field count descending 
            var sortedRecords = records.OrderByDescending(r => r.Fields.Count).ToList();
            var columns = sortedRecords.FirstOrDefault();

            var processedRecords = new ObservableCollection<ADIFRecord>();
            var processedFields = new ObservableCollection<ADIFField>();

            if (columns == null) return new ObservableCollection<ADIFRecord>();

            foreach (var record in records)
            {
                processedFields = new ObservableCollection<ADIFField>();

                foreach (var column in columns.Fields)
                {
                    var adifField = new ADIFField() { Name = column.Name };

                    foreach (var field in record.Fields)
                    {
                        if (column.Name == field.Name)
                        {
                            adifField.Length = field.Length;
                            adifField.Type = field.Type;
                            adifField.Value = field.Value;
                            break;
                        }
                    }

                    processedFields.Add(adifField);
                }

                processedRecords.Add(
                    new ADIFRecord()
                    {
                        Fields = processedFields
                    }
                );
            }

            return processedRecords;

        }

        public void SetupGrid(DataGrid dataGrid)
        {
            _dataGridService.SetupGrid(dataGrid, Records);
        }

        public void ApplyStyles(DataGrid dataGrid)
        {
            _dataGridService.ApplyStyles(dataGrid);
        }

        public ADIFField ParseField(StreamReader sr)
        {
            return _adifFieldService.ParseField(sr);
        }

        private void OnSaveCsv(object commandParameter)
        {
            if (!Records.Any())
            {
                MessageBox.Show("No records found", Properties.Resources.ApplicationName);
                return;
            }

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Csv file (*.csv)|*.csv"
            };

            if (saveFileDialog.ShowDialog() == true)
            {                
                _csvService.SaveCsv(Records, saveFileDialog.FileName);
                MessageBox.Show($"{saveFileDialog.SafeFileName} saved.", Properties.Resources.ApplicationName);
            }
            saveFileDialog = null;
        }

        private void OnSaveXlsx(object commandParameter)
        {
            if (!Records.Any())
            {
                MessageBox.Show("No records found", Properties.Resources.ApplicationName);
                return;
            }

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Xlsx file (*.xlsx)|*.xlsx"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                _openXmlService.GenerateXlsxFile(saveFileDialog.FileName);
                MessageBox.Show($"{saveFileDialog.SafeFileName} saved.", Properties.Resources.ApplicationName);
            }
            saveFileDialog = null;
        }

        private void OnSavePlanillaCsv(object commandParameter)
        {
            if (!Records.Any())
            {
                MessageBox.Show("No records found", Properties.Resources.ApplicationName);
                return;
            }

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Csv file (*.csv)|*.csv"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                _csvService.SavePlanillaCsv(Records, saveFileDialog.FileName);
                MessageBox.Show($"{saveFileDialog.SafeFileName} saved.", Properties.Resources.ApplicationName);
            }
            saveFileDialog = null;
        }

    }
}
