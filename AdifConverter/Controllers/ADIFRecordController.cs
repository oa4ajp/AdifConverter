using AdifConverter.ADIF;
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

namespace AdifConverter.Controllers
{
    public class ADIFRecordController
    {
        public ObservableCollection<ADIFRecord> ReadRecords(string fileName)
        {
            var adifRecords = new ObservableCollection<ADIFRecord>();

            var reader = new StreamReader(fileName, Encoding.UTF8);

            string[] separatingChar = { "<EOH>", "<eoh>" };

            //Remove LineBrake and Tabs characters
            string fileContent = reader.ReadToEnd().Replace("\n", "").Replace("\r", "").Replace("\t", "").Trim(); ;

            string[] contentArray = fileContent.Split(separatingChar, System.StringSplitOptions.RemoveEmptyEntries);

            MemoryStream mStrm = new MemoryStream(Encoding.UTF8.GetBytes(contentArray[1]));

            StreamReader streamReader = new StreamReader(mStrm);

            var fieldController = new ADIFFieldController();

            var record = new ADIFRecord() { Fields = new ObservableCollection<ADIFField>() { new ADIFField() { Name = "Line", Value = $"{adifRecords.Count + 1}" } } };

            try
            {
                for (; ; )
                {
                    var field = fieldController.ParseField(streamReader);

                    if (field == null)
                        break;

                    if ("EOR".Equals(field.Name.ToUpper()))
                    {
                        var tempRecord = record.Clone() as ADIFRecord;

                        adifRecords.Add(tempRecord);

                        record = new ADIFRecord() { Fields = new ObservableCollection<ADIFField>() { new ADIFField() { Name = "Line", Value= $"{adifRecords.Count + 1 }" } } };
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
            
            var processedRecords = ProcessRecords(adifRecords);

            return processedRecords;
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
