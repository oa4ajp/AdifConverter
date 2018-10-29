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
        public ObservableCollection<ADIFField> Fields { get; set; }

        public int FieldsCounter { get; set; }

        public ADIFRecord()
        {
            Fields = new ObservableCollection<ADIFField>();
        }

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
