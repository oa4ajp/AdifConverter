using AdifConverter.Models;
using DocumentFormat.OpenXml.Spreadsheet;

namespace AdifConverter.Strategy
{
    public class OpenXmlRowDataBuilder : OpenXmlRowBuilderBase, IOpenXmlRowBuilderStrategy
    {
        public void AppendRow(Row row, ADIFField field)
        {
            row.Append(ConstructCell(field.Value, CellValues.String));
        }
    }
}