using AdifConverter.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdifConverter.Services
{
    public interface ICSVService
    {
        void SaveCsv(ObservableCollection<ADIFRecord> adifRecords, string filePath);
        void SavePlanillaCsv(ObservableCollection<ADIFRecord> adifRecords, string filePath);
    }
}
