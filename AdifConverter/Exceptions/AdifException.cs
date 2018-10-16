using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdifConverter.Exceptions
{
    public class AdifException : Exception
    {
        public AdifException(String message) : base(message) { }

    }
}
