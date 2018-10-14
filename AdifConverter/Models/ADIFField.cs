using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdifConverter.Models
{
    public class ADIFField
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

        public string Name { get; set; }
        public string Length { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
    }
}
