using System;
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
        private readonly IHubContext<FileBrowserHub> _hubContext;

        public FileBrowserHub(IServiceBusClient serviceBusClient, IHubContext<FileBrowserHub> hubContext)
        {
            _serviceBusClient = serviceBusClient;
            _hubContext = hubContext;
        }

        public void GetPaths(string root, string responseAt)
        {
            var cid = Context.ConnectionId;

            Task.Run(async () =>
            {
                var localCid = cid;
                var payload = new { Path = root };
                var response = await _serviceBusClient.Request<List<FileBrowserResponse>>("FileBrowser", payload);
                await _hubContext.Clients.Client(localCid).SendAsync(responseAt, response);
            });
        }
    }
}