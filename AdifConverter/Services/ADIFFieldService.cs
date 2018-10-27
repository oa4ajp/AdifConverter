using AdifConverter.Exceptions;
using AdifConverter.Models;
using System;
using System.IO;
using System.Text;

namespace AdifConverter.Services
{
    public class ADIFFieldService : IADIFFieldService
    {
        public ADIFField ParseField(StreamReader sr)
        {
            StringBuilder name = new StringBuilder();
            StringBuilder length = new StringBuilder();
            StringBuilder type = new StringBuilder();
            StringBuilder value = new StringBuilder();

            byte state = 0;
            short fieldlen = 0; //It could be negative due to the --fieldlen

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
             *      
             *      Read() -1 if no more characters are available.
             */

            while (true)
            {
                int c;

                switch (state)
                {
                    case 0:
                        c = sr.Read();
                        if (c == -1)
                            return null; // null field
                        if (Char.IsWhiteSpace((char)c))
                            break;
                        if (c == '<')
                        {
                            state = 1;
                            break;
                        }                        
                        throw new AdifException($"Invalid character '{(char)c}', expected '<'");

                    case 1:
                        //Name Section
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
                            throw new AdifException("Unexpected end-of-file encountered while reading ADIF record.");
                        }
                        else
                        {                            
                            throw new AdifException($"Invalid character '{(char)c}' on the tag name");
                        }
                        break;                    
                    case 2:
                        //Length Section
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
                            throw new AdifException("Unexpected end-of-file encountered while reading ADIF record.");
                        }
                        else
                        {                            
                            throw new AdifException($"Invalid character '{(char)c}' on the tag length.");
                        }
                        break;
                    case 3:
                        //Type Sections
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
                            throw new AdifException($"Invalid character '{(char)c}' on the tag type.");
                        }
                        break;
                    case 4:
                        state = 5;
                        if (length.Length > 0)
                        {
                            try
                            {
                                fieldlen = Int16.Parse(length.ToString());
                            }
                            catch (Exception)
                            {                                
                                throw new AdifException($"Can't parse field length {length} .");
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
                                throw new AdifException("Unexpected end-of-file encountered while reading ADIF record.");
                            }
                            value.Append((char)c);
                        }
                        else
                        {
                            var tempLength = length.ToString();

                            return new ADIFField()
                            {
                                Name = name.ToString(),
                                Length = Convert.ToUInt16( (string.IsNullOrEmpty(tempLength) ? "0" : tempLength) ),
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
