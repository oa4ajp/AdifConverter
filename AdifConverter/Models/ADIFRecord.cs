using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdifConverter.Models
{
    public class ADIFRecord : ICloneable
    {
        public ADIFRecord()
        {
            Fields = new ObservableCollection<ADIFField>();
        }

        public ObservableCollection<ADIFField> Fields { get; set; }

        public virtual object Clone()
        {
            return new ADIFRecord()
            {
                Fields = Fields
            };
        }

    }
}
