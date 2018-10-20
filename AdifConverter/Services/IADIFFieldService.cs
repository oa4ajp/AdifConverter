using AdifConverter.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdifConverter.Services
{
    public interface IADIFFieldService
    {
        ADIFField ParseField(StreamReader sr);
    }
}
