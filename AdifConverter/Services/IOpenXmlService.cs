using AdifConverter.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdifConverter.Services
{
    public interface IOpenXmlService
    {
        void GenerateXlsxFile(ObservableCollection<ADIFRecord> adifRecords, string filePath, string fileName);
    }
}
