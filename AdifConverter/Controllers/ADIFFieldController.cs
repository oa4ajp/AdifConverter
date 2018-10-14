using AdifConverter.Models;
using System;
using System.IO;
using System.Text;

namespace AdifConverter.ADIF
{
    public class ADIFFieldController
    {
        public ADIFField ParseField(StreamReader sr)
        {
            StringBuilder name = new StringBuilder();
            StringBuilder length = new StringBuilder();
            StringBuilder type = new StringBuilder();
            StringBuilder value = new StringBuilder();

            int state = 0;
            int fieldlen = 0;

            /*
             * FSM parser.
             *  state = 0 : 
             *      look for start-of-record '<'.  Only whitespace allowed
             *      goes to state = 1 when start-of-record found
             *  state = 1 :
             *      get the field name 
             *      a colon goes to state=2
             *      a end-of-record ('>') returns the record
             * state = 2 :
             *      get the field value length.  Only digits allowed
             *      a colon goes to state = 3
             *      a end-of-record goes to state = 4
             * state = 3 :
             *      get the field  type
             *      alnum characters allowed
             *      end-of-record goes to state = 4
             * state = 4 :
             *      get the field value
             */

            while (true)
            {
                int c;

                switch (state)
                {
                    case 0:
                        c = sr.Read();
                        if (c == -1)
                            return null; // null record
                        if (Char.IsWhiteSpace((char)c))
                            break;
                        if (c == '<')
                        {
                            state = 1;
                            break;
                        }
                        return null;

                    case 1:
                        c = sr.Read();
                        if (c == ':')
                        {
                            state = 2;
                        }
                        else if (c == '>')
                        {
                            state = 4;
                        }
                        else if (c == '_' || Char.IsLetterOrDigit((char)c))
                        {
                            name.Append((char)c);
                        }
                        else if (c == -1)
                        {
                            return null; // "Unexpected end-of-file encountered while reading ADIF record.";
                        }
                        else
                        {
                            return null; // "Invalid character '" + (char)c + "'.";
                        }
                        break;
                    case 2:
                        c = sr.Read();
                        if (c == ':')
                        {
                            state = 3;
                        }
                        else if (c == '>')
                        {
                            state = 4;
                        }
                        else if (Char.IsDigit((char)c))
                        {
                            length.Append((char)c);
                        }
                        else if (c == -1)
                        {
                            return null; // "Unexpected end-of-file encountered while reading ADIF record.";
                        }
                        else
                        {
                            return null; //"Invalid character '" + (char)c + "'.";
                        }
                        break;
                    case 3:
                        c = sr.Read();
                        if (c == '>')
                        {
                            state = 4;
                        }
                        else if (Char.IsLetterOrDigit((char)c))
                        {
                            type.Append((char)c);
                        }
                        else
                        {
                            return null; // "Invalid character '" + (char)c + "'.";
                        }
                        break;
                    case 4:
                        state = 5;
                        if (length.Length > 0)
                        {
                            try
                            {
                                fieldlen = Int32.Parse(length.ToString());
                            }
                            catch (Exception e)
                            {
                                return null; // "Can't parse field length '" + length + "'." + e;
                            }
                        }
                        break;
                    case 5:
                        --fieldlen;
                        if (fieldlen >= 0)
                        {
                            c = sr.Read();
                            if (c == -1)
                            {
                                return null; // "Unexpected end-of-file encountered while reading ADIF record.";
                            }
                            value.Append((char)c);
                        }
                        else
                        {                            
                            return new ADIFField()
                            {
                                Name = name.ToString(),
                                Length = length.ToString(),
                                Type = type.ToString(),
                                Value = value.ToString(),
                            };               
                        }
                        break;
                }

            }
        }
    }
}
