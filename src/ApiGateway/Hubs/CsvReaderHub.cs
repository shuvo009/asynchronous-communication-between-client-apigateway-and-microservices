using System.Threading.Tasks;
using ApiGateway.Interface;
using ApiGateway.Models;
using Microsoft.AspNetCore.SignalR;

namespace ApiGateway.Hubs
{
    public class CsvReaderHub : Hub
    {
        private readonly IServiceBusClient _serviceBusClient;
        private readonly IHubContext<CsvReaderHub> _hubContext;

        public CsvReaderHub(IServiceBusClient serviceBusClient, IHubContext<CsvReaderHub> hubContext)
        {
            _serviceBusClient = serviceBusClient;
            _hubContext = hubContext;
        }

        public void Read(string path, int page, string responseAt)
        {
            var cid = Context.ConnectionId;
            Task.Run(async () =>
            {
                var localCid = cid;
                var payload = new { Path = path, Page = page };
                var data = await _serviceBusClient.Request<CsvFileContentResponse>("ReadCsvFile", payload);
                await _hubContext.Clients.Client(localCid).SendAsync(responseAt, data);
            });
        }
    }
}