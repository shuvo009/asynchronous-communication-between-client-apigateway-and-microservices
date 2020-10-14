using System.Collections.Generic;
using System.Threading.Tasks;
using ApiGateway.Interface;
using ApiGateway.Models;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileBrowserController : ControllerBase
    {
        private readonly IServiceBusClient _serviceBusClient;

        public FileBrowserController(IServiceBusClient serviceBusClient)
        {
            _serviceBusClient = serviceBusClient;
        }

        [HttpGet("GetPaths")]
        public async Task<IActionResult> GetPaths(string root)
        {
            var payload = new {Path = root};
            var paths = await _serviceBusClient.Request<List<FileBrowserResponse>>("FileBrowser", payload);
            return Ok(paths);
        }
    }
}