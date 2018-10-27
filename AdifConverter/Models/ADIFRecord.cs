using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdifConverter.Models
{
    public class ADIFRecord
    {
        public ADIFRecord()
        {
            Fields = new ObservableCollection<ADIFField>();
        }

        public ObservableCollection<ADIFField> Fields { get; set; }

        public ADIFRecord ShallowCopy()
        {
            return (ADIFRecord)this.MemberwiseClone();
        }

        public ADIFRecord DeepCopy()
        {
            var other = (ADIFRecord) this.MemberwiseClone();
            other.Fields = Fields;
            return other;
        }
    }
}
