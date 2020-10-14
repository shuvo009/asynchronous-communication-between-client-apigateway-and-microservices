using System.Collections.Generic;

namespace ApiGateway.Models
{
    public class CsvFileContentResponse
    {
        public List<List<string>> Rows { get; set; }
    }
}