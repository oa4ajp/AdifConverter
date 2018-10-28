using AdifConverter.Models;
using System;
using System.IO;

namespace AdifConverter.Services.Interfaces
{
    public interface IADIFFieldService
    {
        ADIFField ParseField(StreamReader sr);
    }
}
