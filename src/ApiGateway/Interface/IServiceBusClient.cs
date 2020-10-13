using System.Threading.Tasks;

namespace ApiGateway.Interface
{
    public interface IServiceBusClient
    {
        Task<T> Request<T>(string queueName, object payload) where T : class;
    }
}