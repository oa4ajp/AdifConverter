using AdifConverter.Models;
using DocumentFormat.OpenXml.Spreadsheet;
using static AdifConverter.Enums.Enums;

namespace AdifConverter.Strategy
{
    public class OpenXmlRowHeaderBuilder : OpenXmlRowBuilder, IOpenXmlRowBuilderStrategy
    {
        public void BuildRow(ADIFRecord adifRecord, Row row, RowType rowtype)
        {
            int fieldCounter = 1;

            foreach (var adifField in adifRecord.Fields)
            {
                if (fieldCounter > 1)
                {
                    row.Append(ConstructCell(adifField.Name, CellValues.String));
                }

                fieldCounter++;
            }
        }
    }
}
