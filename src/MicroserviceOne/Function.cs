using System;
using System.Linq;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using MicroserviceOne.Interfaces;
using MicroserviceOne.Models;
using Microsoft.Extensions.Configuration;

namespace MicroserviceOne
{
    public class Function
    {
        private readonly IConfiguration _configuration;
        private readonly IFileBrowser _fileBrowser;

        public Function(IConfiguration configuration, IFileBrowser fileBrowser)
        {
            _configuration = configuration;
            _fileBrowser = fileBrowser;
        }

        public async void FileBrowser([ServiceBusTrigger("FileBrowser")] Message message, ILogger log)
        {
            log.LogInformation("Messages Received");
            var messageJson = Encoding.UTF8.GetString(message.Body);
            log.LogInformation($"Messages Body : { messageJson}");

            var fileBrowserRequest = JsonConvert.DeserializeObject<FileBrowserRequest>(messageJson);
            var paths = _fileBrowser.GetFileAndFolders(fileBrowserRequest.Path);

            var reply = JsonConvert.SerializeObject(paths);
            log.LogInformation($"Reply : ${reply}");
            await SendReply(message, reply, log);
        }

        #region Supported Method

        private async Task SendReply(Message senderMessage, string reply, ILogger log)
        {

            var replyQueue = senderMessage.ReplyTo;
            var responseMessage = new Message(Encoding.UTF8.GetBytes(reply))
            {
                ReplyToSessionId = senderMessage.SessionId
            };
            var client = new QueueClient(_configuration["serviceBusConnection"], replyQueue);
            await client.SendAsync(responseMessage);
            log.LogInformation("Reply Sended");
        }

        #endregion
    }
}