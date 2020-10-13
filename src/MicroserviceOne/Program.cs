using System.Threading.Tasks;
using MicroserviceOne.Implementations;
using MicroserviceOne.Interfaces;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MicroserviceOne
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var program = new Program();
            await program.RunHost();
        }

        #region Supported Methods

        public async Task RunHost()
        {
            var builder = new HostBuilder();
            builder.ConfigureAppConfiguration((context, configurationBuilder) =>
                {
                    configurationBuilder.AddJsonFile("appsettings.json", true);
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddTransient<Function, Function>();
                    services.AddTransient<IFileBrowser, FileBrowser>();
                    services.AddSingleton<IJobActivator>(new WebJobActivator(services.BuildServiceProvider()));
                })
                .ConfigureWebJobs((c, b) =>
                {
                    b.AddAzureStorageCoreServices();
                    b.AddServiceBus(sbOptions =>
                    {
                        sbOptions.MessageHandlerOptions.AutoComplete = true;
                        sbOptions.MessageHandlerOptions.MaxConcurrentCalls = 16;
                        sbOptions.ConnectionString = c.Configuration["serviceBusConnection"];
                    });
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConsole();
                    logging.AddDebug();
                });
            var host = builder.Build();
            using (host)
            {
                var logger = host.Services.GetService<ILogger<Program>>();
                logger.LogInformation("MicroService One Is Started :)");
                await host.RunAsync();
            }
        }

        #endregion
    }
}
