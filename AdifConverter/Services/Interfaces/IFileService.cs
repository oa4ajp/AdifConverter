using AdifConverter.Models;
using System.Collections.ObjectModel;
using static AdifConverter.Enums.Enums;

namespace AdifConverter.Services.Interfaces
{
    public interface IFileService
    {
        void SaveFile(ObservableCollection<ADIFRecord> records, string fullFileName, FileType fileType);
    }
}
