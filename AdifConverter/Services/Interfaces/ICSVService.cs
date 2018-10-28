using AdifConverter.Models;
using System.Collections.ObjectModel;

namespace AdifConverter.Services.Interfaces
{
    public interface ICSVService
    {
        void SaveCsv(ObservableCollection<ADIFRecord> adifRecords, string filePath);
        void SavePlanillaCsv(ObservableCollection<ADIFRecord> adifRecords, string filePath);
    }
}
