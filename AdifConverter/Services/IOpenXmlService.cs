using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdifConverter.Services
{
    public interface IOpenXmlService
    {
        void GenerateXlsxFile(string filePath);
    }
}
