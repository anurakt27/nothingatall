using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace basicapp.Services
{
    public partial class FileService : IFileService
    {
        static private readonly string[] supportedFileTypes = new string[] { "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "application/vnd.ms-excel" };
        public bool IsValidFileType(IFormFile file)
        {
            return supportedFileTypes.Contains(file.ContentType);
        }

        // gets list of emails that are in "Email" column of Excel sheet.
        public IEnumerable<string> ProcessFile(IFormFile file)
        {
            List<string> email = new List<string>();
            DataSet dataSet = null;

            // Register an EncodingProvider to be able to access excel encodings.
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var stream = new MemoryStream())
            {
                // copy file contents into Memory Stream
                file.CopyTo(stream);
                stream.Position = 0;
                IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream);

                // configure the excel reader to specify which columns to read
                var config = new ExcelDataSetConfiguration
                {
                    ConfigureDataTable = _ => new ExcelDataTableConfiguration
                    {
                        // iterate through each column and filter the ones you need
                        FilterColumn = (colReader, colIndex) => 
                        {
                            string header = colReader.GetString(colIndex);
                            return (header == "Email");
                        },
                        UseHeaderRow = true
                    }
                };

                dataSet = reader.AsDataSet(config);
            }

            // iterate through all sheets and rows and fetch all email addresses
            foreach(DataTable table in dataSet.Tables)
            {
                foreach(DataRow row in table.Rows)
                {
                    email.Add(row[0].ToString());
                }
            }

            return email;
        }
    }

    public interface IFileService
    {
        bool IsValidFileType(IFormFile file);
        IEnumerable<string> ProcessFile(IFormFile file);
    }
}
