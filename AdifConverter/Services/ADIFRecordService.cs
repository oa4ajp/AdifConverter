using AdifConverter.Exceptions;
using AdifConverter.Models;
using AdifConverter.Services.Interfaces;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace AdifConverter.Services
{
    public class ADIFRecordService : IADIFRecordService
    {
        private IADIFFieldService _adifFieldService;

        public ADIFRecordService(IADIFFieldService adifFieldService)
        {
            _adifFieldService = adifFieldService;
        }

        public ObservableCollection<ADIFRecord> ReadRecords(string fileName)
        {
            var records = new ObservableCollection<ADIFRecord>();
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
                        record.FieldsCounter = record.Fields.Count;
                        var tempRecord = record.DeepCopy();

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
                return new ObservableCollection<ADIFRecord>();
            }
            finally
            {
                streamReader.Dispose();
                mStrm.Dispose();
                reader.Dispose();
            }

            var listHasDifferentFieldsCounterValues = adifRecords.Select(x => x.FieldsCounter).Distinct().Skip(1).Any();
            return listHasDifferentFieldsCounterValues ? ProcessRecords(adifRecords) : adifRecords;
         
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
    }
}
