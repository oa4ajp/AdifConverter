using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdifConverter.Models
{
    public class ADIFRecord : ICloneable
    {
        public ADIFRecord()
        {
            Fields = new List<ADIFField>();
        }

        public List<ADIFField> Fields { get; set; }

        public virtual object Clone()
        {
            return new ADIFRecord()
            {
                Fields = Fields
            };
        }

    }
}
