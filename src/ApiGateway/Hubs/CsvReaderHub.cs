using System.Threading.Tasks;
using ApiGateway.Interface;
using ApiGateway.Models;
using Microsoft.AspNetCore.SignalR;

namespace ApiGateway.Hubs
{
    public class CsvReaderHub : Hub
    {
        private readonly IServiceBusClient _serviceBusClient;

        public CsvReaderHub(IServiceBusClient serviceBusClient)
        {
            _serviceBusClient = serviceBusClient;
        }

        public void Read(string path, int page, string responseAt)
        {
            Task.Run(async () =>
            {
                var payload = new {Path = path, Page = page};
                var paths = await _serviceBusClient.Request<CsvFileContentResponse>("ReadCsvFile", payload);
                await Clients.Client(Context.ConnectionId).SendAsync(responseAt, page);
            }).Start();
        }
    }
}