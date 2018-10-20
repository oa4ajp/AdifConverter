using System.ComponentModel;

namespace AdifConverter.Models
{
    public class ADIFField : INotifyPropertyChanged
    {
        /**
         * Representation of an ADIF field.
         *
         * An ADIF field is a single part of a logbook entry, like a
         * call sign, date, time, etc.
         *
         * A field consists of a mandatory field name, the length of the
         * field value, and optionally a field data typer identifier,
         * separated by a colon character, and enclosed in \'<' and '>'.
         * It is followed by the (optional) field value.
         *   Example: "<call:6:s>LA1BFA "
         *
         * Note that the data type is usually omitted for "well-known"
         * fields (i.e those specified in the ADIF specification).
         */

        private string _name;
        private string _length;
        private string _Type;
        private string _value;

        public ADIFField()
        {
            Value = string.Empty;
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged("Name");
            }
        } 
        public string Length
        {
            get { return _length; }
            set
            {
                _length = value;
                RaisePropertyChanged("Length");
            }
        }
        public string Type
        {
            get { return _Type; }
            set
            {
                _Type = value;
                RaisePropertyChanged("Type");
            }
        }

        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
                RaisePropertyChanged("Value");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
