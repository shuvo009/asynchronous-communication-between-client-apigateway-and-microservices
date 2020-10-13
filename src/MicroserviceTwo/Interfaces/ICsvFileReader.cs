using MicroserviceTwo.Models;

namespace MicroserviceTwo.Interfaces
{
    public interface ICsvFileReader
    {
        CsvFileContentResponse Read(string path, int page);
    }
}