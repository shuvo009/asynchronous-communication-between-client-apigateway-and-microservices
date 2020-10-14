using System.Threading.Tasks;
using ApiGateway.Interface;
using ApiGateway.Models;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CsvReaderController : ControllerBase
    {
        private readonly IServiceBusClient _serviceBusClient;

        public CsvReaderController(IServiceBusClient serviceBusClient)
        {
            _serviceBusClient = serviceBusClient;
        }

        [HttpGet("Read/{page}")]
        public async Task<IActionResult> Read(string path, int page)
        {
            var payload = new {Path = path, Page = page};
            var paths = await _serviceBusClient.Request<CsvFileContentResponse>("ReadCsvFile", payload);
            return Ok(paths);
        }
    }
}