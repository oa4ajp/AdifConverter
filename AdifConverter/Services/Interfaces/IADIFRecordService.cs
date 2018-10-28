using AdifConverter.Models;
using System.Collections.ObjectModel;

namespace AdifConverter.Services.Interfaces
{
    public interface IADIFRecordService
    {
        ObservableCollection<ADIFRecord> ReadRecords(string fileName);
        ObservableCollection<ADIFRecord> ProcessRecords(ObservableCollection<ADIFRecord> records);
    }
}
