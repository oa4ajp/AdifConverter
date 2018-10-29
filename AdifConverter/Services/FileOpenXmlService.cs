using AdifConverter.Models;
using AdifConverter.Services.Interfaces;
using AdifConverter.Strategy;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AdifConverter.Enums.Enums;

namespace AdifConverter.Services
{
    public class FileOpenXmlService : FileServiceBase, IFileServiceStrategy
    {
        private IOpenXmlRowBuilderStrategy openXmlRowBuilderStrategy;

        private IOpenXmlRowBuilderStrategy _openXmlRowHeaderBuilder;
        private IOpenXmlRowBuilderStrategy _openXmlRowDataBuilder;

        public FileOpenXmlService(IOpenXmlRowBuilderStrategy openXmlRowHeaderBuilder, IOpenXmlRowBuilderStrategy openXmlRowDataBuilder)
        {
            _openXmlRowHeaderBuilder = openXmlRowHeaderBuilder;
            _openXmlRowDataBuilder = openXmlRowDataBuilder;
        }

        private IOpenXmlRowBuilderStrategy GetOpenXmlRowBuilderOption(RowType rowType)
        {
            IOpenXmlRowBuilderStrategy openXmlRowBuilderStrategy = null;

            switch (rowType)
            {
                case RowType.Header:
                    openXmlRowBuilderStrategy = _openXmlRowHeaderBuilder;
                    break;
                case RowType.Data:
                    openXmlRowBuilderStrategy = _openXmlRowDataBuilder;
                    break;
                default:
                    break;
            }
            return openXmlRowBuilderStrategy;
        }

        public void Save(ObservableCollection<ADIFRecord> adifRecords, string filePath)
        {
            if (!ValidateRecords(adifRecords)) return;

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Xlsx file (*.xlsx)|*.xlsx",
                FileName = GetFileName(filePath)
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                SaveFile(adifRecords, saveFileDialog.FileName);
                ShowSaveConfirmation(saveFileDialog.SafeFileName);
            }
        }

        public void SaveFile(ObservableCollection<ADIFRecord> adifRecords, string filePath)
        {
            if (!adifRecords.Any()) return;

            var firstRecord = adifRecords[0];

            var sheetName = Path.GetFileNameWithoutExtension(filePath); 

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
