using AdifConverter.Models;
using System.Collections.ObjectModel;

namespace AdifConverter.Services.Interfaces
{
    public interface IFileServiceStrategy
    {
        void Save(ObservableCollection<ADIFRecord> adifRecords, string filePath);
    }
}
