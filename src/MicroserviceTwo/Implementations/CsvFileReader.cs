using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using MicroserviceTwo.Interfaces;
using MicroserviceTwo.Models;

namespace MicroserviceTwo.Implementations
{
    public class CsvFileReader : ICsvFileReader
    {
        public CsvFileContentResponse Read(string path, int page)
        {
            var csvFileContentResponse = new CsvFileContentResponse {Rows = new List<List<string>>()};
            using (var fileReader = new StreamReader(path))
            using (var csvResult = new CsvReader(fileReader, CultureInfo.InvariantCulture))
            {
                csvResult.Configuration.BadDataFound = null;
                csvResult.Configuration.MissingFieldFound = null;
                csvResult.Configuration.DetectColumnCountChanges = true;
                csvResult.Configuration.Delimiter = ",";
                csvResult.Configuration.HasHeaderRecord = true;

                csvResult.Read();

                var recordCount = 0;
                var skip = page - 1 * 10;
                var maxCount = page * 10;

                while (csvResult.Read() && recordCount <= maxCount)
                {
                    recordCount++;
                    if (recordCount <= skip)
                        continue;

                    var headerRow = csvResult.Context.ColumnCount;
                    var row = new List<string>();
                    foreach (var i in Enumerable.Range(0, headerRow))
                    {
                        var stringField = csvResult.GetField(i);
                        row.Add(stringField);
                    }

                    csvFileContentResponse.Rows.Add(row);
                }
            }

            return csvFileContentResponse;
        }
    }
}