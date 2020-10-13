using System;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.DependencyInjection;

namespace MicroserviceOne.Implementations
{
    public class WebJobActivator : IJobActivator
    {
        private readonly IServiceProvider _service;

        public WebJobActivator(IServiceProvider service)
        {
            _service = service;
        }

        public T CreateInstance<T>()
        {
            try
            {
                var service = _service.GetService<T>();
                return service;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}