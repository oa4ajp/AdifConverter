using AdifConverter.Models;

using System.Collections.ObjectModel;namespace AdifConverter.Services.Interfaces
{
    public interface IOpenXmlService
    {
        void GenerateXlsxFile(ObservableCollection<ADIFRecord> adifRecords, string filePath, string fileName);
    }
}
