using AdifConverter.Models;
using DocumentFormat.OpenXml.Spreadsheet;
using static AdifConverter.Enums.Enums;

namespace AdifConverter.Strategy
{
    public interface IOpenXmlRowBuilderStrategy
    {
        void BuildRow(ADIFRecord adifRecord, Row row, RowType rowtype);
    }
}
