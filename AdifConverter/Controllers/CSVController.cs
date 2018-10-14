using AdifConverter.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdifConverter.Controllers
{
    public class CSVController
    {
        private readonly string _delimiter = ";";

        private string FilePath { get; set; }

        public CSVController(string filePath)
        {
            FilePath = filePath;
        }

        public void SaveCsv(List<ADIFRecord> adifRecords)
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
            
            using (var w = new StreamWriter(FilePath)) {

                foreach (var sb in stringBuilderList)
                {
                    w.WriteLine(sb.ToString());
                }                
                w.Flush();
            }

        }

        public void SavePlanillaCsv(List<ADIFRecord> adifRecords)
        {            
            if (!adifRecords.Any()) return;

            var fieldsToInclude = new List<string>(){ "QTR OA", "INDICATIVO", "REP. ENTREGADO", "REP. RECIBIDO"};

            List<ADIFRecord> planillaRecords = new List<ADIFRecord>();

            int recordCounter = 1;

            //Remove Unused Fields
            foreach (var adifRecord in adifRecords)
            {
                var record = new ADIFRecord();
                //Custom Fields
                record.Fields.Add(new ADIFField() { Name = "QSO", Value = recordCounter.ToString() });

                foreach (var field in fieldsToInclude)
                {
                    if (field.Equals("QTR OA"))
                    {
                        var qsoDate = adifRecord.Fields.Where(x => x.Name.Equals("QSO_DATE")).FirstOrDefault() ?? new ADIFField();
                        var qsoTimeOn = adifRecord.Fields.Where(x => x.Name.Equals("TIME_ON")).FirstOrDefault() ?? new ADIFField();
                        
                        string qsoLocalTime = string.Empty;

                        //Refactor on Function to get the LocalHour
                        if (qsoDate.Value.Length == 8 && qsoTimeOn.Value.Length == 6)
                        {
                            string year = qsoDate.Value.Substring(0, 4);
                            string month = qsoDate.Value.Substring(4, 2);
                            string day = qsoDate.Value.Substring(6, 2);

                            string hours = qsoTimeOn.Value.Substring(0, 2);
                            string minutes = qsoTimeOn.Value.Substring(2, 2);
                            string seconds = qsoTimeOn.Value.Substring(4, 2);

                            var formattedDate = $"{year}-{month}-{day}T{hours}:{minutes}:{seconds}Z";

                            DateTime utcDate = DateTime.Parse(formattedDate, null, System.Globalization.DateTimeStyles.RoundtripKind);

                            //Make configurable depending the time zone, or use a offset
                            DateTime localDate = utcDate.AddHours(-5);

                            qsoLocalTime = $"{localDate.Hour:00}:{localDate.Minute:00}";
                        }

                        record.Fields.Add(new ADIFField() { Name = field, Value = qsoLocalTime });
                        continue;
                    }

                    if (field.Equals("INDICATIVO"))
                    {
                        var item = adifRecord.Fields.Where(x => x.Name.Equals("CALL")).FirstOrDefault() ?? new ADIFField();
                        record.Fields.Add(new ADIFField() { Name = field, Value = item.Value });
                        continue;
                    }

                    if (field.Equals("REP. ENTREGADO"))
                    {
                        var item = adifRecord.Fields.Where(x => x.Name.Equals("STX")).FirstOrDefault() ?? new ADIFField();
                        Int32.TryParse(item.Value, out int stx);

                        var rstRcvd = adifRecord.Fields.Where(x => x.Name.Equals("RST_RCVD")).FirstOrDefault() ?? new ADIFField();
                        
                        record.Fields.Add(new ADIFField() { Name = field, Value = $"{rstRcvd.Value} {stx:000}" });
                        continue;
                    }

                    if (field.Equals("REP. RECIBIDO"))
                    {
                        var item = adifRecord.Fields.Where(x => x.Name.Equals("SRX")).FirstOrDefault() ?? new ADIFField();
                        Int32.TryParse(item.Value, out int srx);

                        var rstSent = adifRecord.Fields.Where(x => x.Name.Equals("RST_SENT")).FirstOrDefault() ?? new ADIFField();

                        record.Fields.Add(new ADIFField() { Name = field, Value = $"{rstSent.Value} {srx:000}" });
                        continue;
                    }
                }                

                planillaRecords.Add(record);
                recordCounter++;
            }

            var stringBuilderList = new List<StringBuilder>();

            var csvHeader = new StringBuilder();

            var firstRecord = planillaRecords[0];

            BuildHeader(firstRecord, csvHeader);
            stringBuilderList.Add(csvHeader);

            foreach (var adifRecord in planillaRecords)
            {
                var csvRow = new StringBuilder();
                BuildRow(adifRecord, csvRow);
                stringBuilderList.Add(csvRow);
            }

            using (var w = new StreamWriter(FilePath))
            {

                foreach (var sb in stringBuilderList)
                {
                    w.WriteLine(sb.ToString());
                }
                w.Flush();
            }
        }

        private void BuildHeader(ADIFRecord adifRecord, StringBuilder csvRows)
        {
            int totalfields = adifRecord.Fields.Count;
            int fieldCounter = 1;

            foreach (var adifField in adifRecord.Fields)
            {
                AppendRows(csvRows, adifField.Name, fieldCounter, totalfields);
                fieldCounter++;
            }
        }

        private void BuildRow(ADIFRecord adifRecord, StringBuilder csvRows)
        {
            int totalfields = adifRecord.Fields.Count;
            int fieldCounter = 1;

            foreach (var adifField in adifRecord.Fields)
            {
                AppendRows(csvRows, adifField.Value, fieldCounter, totalfields);
                fieldCounter++;
            }
        }

        private void AppendRows(StringBuilder csvRows, string fieldNameOrValue, int fieldCounter, int totalFields)
        {
            var adifFieldHeader = $"\"{fieldNameOrValue}\"" + (fieldCounter < totalFields ? _delimiter : "");
            csvRows.Append(adifFieldHeader);            
        }

    }
}
