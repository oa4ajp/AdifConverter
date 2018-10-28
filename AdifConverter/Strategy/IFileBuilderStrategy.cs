using AdifConverter.Models;
using System.Collections.ObjectModel;

namespace AdifConverter.Strategy
{
    public interface IFileBuilderStrategy
    {
        void SaveFile(ObservableCollection<ADIFRecord> records, string fullFileName);
    }
}
