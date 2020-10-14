using System.Collections.Generic;
using System.Threading.Tasks;
using ApiGateway.Interface;
using ApiGateway.Models;
using Microsoft.AspNetCore.SignalR;

namespace ApiGateway.Hubs
{
    public class FileBrowserHub : Hub
    {
        private readonly IServiceBusClient _serviceBusClient;

        public FileBrowserHub(IServiceBusClient serviceBusClient)
        {
            _serviceBusClient = serviceBusClient;
        }

        public void GetPaths(string root, string responseAt)
        {
            Task.Run(async () =>
            {
                var payload = new {Path = root};
                var response = await _serviceBusClient.Request<List<FileBrowserResponse>>("FileBrowser", payload);
                await Clients.Client(Context.ConnectionId).SendAsync(responseAt, response);
            }).Start();
        }
    }
}