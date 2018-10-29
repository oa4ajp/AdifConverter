using AdifConverter.Models;
using DocumentFormat.OpenXml.Spreadsheet;
using static AdifConverter.Enums.Enums;

namespace AdifConverter.Strategy
{
    public interface IOpenXmlRowBuilderStrategy
    {
        void AppendRow(Row row, ADIFField field);
    }
}
