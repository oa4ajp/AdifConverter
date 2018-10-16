﻿using AdifConverter.ADIF;
using AdifConverter.Exceptions;
using AdifConverter.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AdifConverter.Controllers
{
    public class ADIFRecordController
    {
        public List<ADIFRecord> ReadRecords(string fileName)
        {
            var adifRecords = new List<ADIFRecord>();

            var reader = new StreamReader(fileName, Encoding.UTF8);

            string[] separatingChar = { "<EOH>", "<eoh>" };

            //Remove LineBrake and Tabs characters
            string fileContent = reader.ReadToEnd().Replace("\n", "").Replace("\r", "").Replace("\t", "").Trim(); ;

            string[] contentArray = fileContent.Split(separatingChar, System.StringSplitOptions.RemoveEmptyEntries);

            MemoryStream mStrm = new MemoryStream(Encoding.UTF8.GetBytes(contentArray[1]));

            StreamReader streamReader = new StreamReader(mStrm);

            var fieldController = new ADIFFieldController();

            var record = new ADIFRecord();

            try
            {
                for (; ; )
                {
                    var field = fieldController.ParseField(streamReader);

                    if (field == null)
                        break;

                    if ("EOR".Equals(field.Name.ToUpper()))
                    {
                        var tempRecord = record.Clone() as ADIFRecord;

                        adifRecords.Add(tempRecord);
                        record = new ADIFRecord();
                    }
                    else
                    {
                        record.Fields.Add(field);
                    }
                }

            }
            catch (AdifException ae)
            {
                var message = ae.Message;
                MessageBox.Show($"{message} on row {adifRecords.Count + 1}.", Properties.Resources.ApplicationName, MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<ADIFRecord>();
            }

            return adifRecords;
        }
    }
}
