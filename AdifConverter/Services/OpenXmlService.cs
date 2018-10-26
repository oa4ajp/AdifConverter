using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdifConverter.Models;
using AdifConverter.Strategy;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using static AdifConverter.Enums.Enums;

namespace AdifConverter.Services
{
    public class OpenXmlService : IOpenXmlService
    {
        private IOpenXmlRowBuilderStrategy openXmlRowBuilderStrategy = null;

        private IOpenXmlRowBuilderStrategy GetOpenXmlRowBuilderOption(RowType rowType)
        {
            IOpenXmlRowBuilderStrategy openXmlRowBuilderStrategy = null;

            switch (rowType)
            {
                case RowType.Header:
                    openXmlRowBuilderStrategy = new OpenXmlRowHeaderBuilder();
                    break;
                case RowType.Data:
                    openXmlRowBuilderStrategy = new OpenXmlRowDataBuilder();
                    break;
                default:
                    break;
            }
            return openXmlRowBuilderStrategy;
        }

        public void GenerateXlsxFile(ObservableCollection<ADIFRecord> adifRecords, string filePath, string fileName)
        {
            string[] separatingChar = { "." };
            string sheetName = string.Empty;

            if (!adifRecords.Any()) return;

            var firstRecord = adifRecords[0];
            var sheetNameArray = fileName.Split(separatingChar, System.StringSplitOptions.RemoveEmptyEntries);

            if (sheetNameArray.Any())
                sheetName = sheetNameArray[0];

            using (SpreadsheetDocument document = SpreadsheetDocument.Create(filePath, SpreadsheetDocumentType.Workbook))
            {
                // Add a WorkbookPart to the document.
                var workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                // Add a WorksheetPart to the WorkbookPart.
                var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new Worksheet();

                var sheets = workbookPart.Workbook.AppendChild(new Sheets());

                var sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = sheetName };
                sheets.Append(sheet);

                workbookPart.Workbook.Save();

                var sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                // Constructing header
                Row row = new Row();
                BuildRow(firstRecord, row, RowType.Header);

                // Insert the header row to the Sheet Data
                sheetData.AppendChild(row);           

                foreach (var adifRecord in adifRecords)
                {
                    row = new Row();
                    BuildRow(adifRecord, row, RowType.Data);
                    sheetData.AppendChild(row);
                }
                
                //Insert List Data
                worksheetPart.Worksheet.Save();
            }
        }

        private void BuildRow(ADIFRecord adifRecord, Row row, RowType rowtype)
        {
            openXmlRowBuilderStrategy = GetOpenXmlRowBuilderOption(rowtype);
            openXmlRowBuilderStrategy.BuildRow(adifRecord, row, rowtype);          
        }

    }
}
