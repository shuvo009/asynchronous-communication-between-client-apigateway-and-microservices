using System.Text;
using System.Threading.Tasks;
using MicroserviceTwo.Interfaces;
using MicroserviceTwo.Models;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MicroserviceTwo
{
    public class Function
    {
        private readonly IConfiguration _configuration;
        private readonly ICsvFileReader _csvFileReader;

        public Function(IConfiguration configuration, ICsvFileReader csvFileReader)
        {
            _configuration = configuration;
            _csvFileReader = csvFileReader;
        }

        public async void FileBrowser([ServiceBusTrigger("ReadCsvFile")] Message message, ILogger log)
        {
            log.LogInformation("Messages Received");
            var messageJson = Encoding.UTF8.GetString(message.Body);
            log.LogInformation($"Messages Body : {messageJson}");

            var fileBrowserRequest = JsonConvert.DeserializeObject<CsvFileReadRequest>(messageJson);
            var paths = _csvFileReader.Read(fileBrowserRequest.Path, fileBrowserRequest.Page);

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