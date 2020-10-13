using System;
using System.Text;
using System.Threading.Tasks;
using ApiGateway.Interface;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace ApiGateway.Implementations
{
    public class ServiceBusClient : IServiceBusClient
    {
        private readonly IConfiguration _configuration;

        public ServiceBusClient(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<T> Request<T>(string queueName, object payload) where T : class
        {
            var queueClient = new QueueClient(_configuration["serviceBusConnection"], queueName);
            var replyQueue = await CreateReplyQueue();

            var message = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload)))
            {
                ReplyTo = replyQueue
            };

            var replyReceiver = new MessageReceiver(_configuration["serviceBusConnection"], replyQueue, ReceiveMode.ReceiveAndDelete);
            await queueClient.SendAsync(message);

            var reply = await replyReceiver.ReceiveAsync(TimeSpan.FromMinutes(5));
            var messageJson = Encoding.UTF8.GetString(reply.Body);
            return JsonConvert.DeserializeObject<T>(messageJson);
        }

        #region Supported Methods

        public async Task<string> CreateReplyQueue()
        {
            var replyQueueName = Guid.NewGuid().ToString();
            var temporaryQueueDescription = new QueueDescription(replyQueueName)
            {
                AutoDeleteOnIdle = TimeSpan.FromMinutes(5),
            };

            var _managementClient = new ManagementClient(_configuration["serviceBusConnection"]);
            await _managementClient.CreateQueueAsync(temporaryQueueDescription);
            return replyQueueName;
        }


        #endregion
    }
}
