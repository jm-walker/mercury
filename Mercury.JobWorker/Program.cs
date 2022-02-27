using Mercury.Common.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Mercury.JobWorker
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, builder) =>
                {
                    builder.AddJsonFile("appsettings.local.json", optional: false);
                })
                .ConfigureServices((hostContext, services) =>
                {
                // Load DLLs dynamically into dictionary lookup

                /* services
                    .Configure<options>(hostContext.Configuration.GetSection(nameof(options)))
                */

                    services.AddLogging(cfg => cfg.AddSimpleConsole(opts =>
                    {
                        opts.IncludeScopes = true;
                        opts.SingleLine = true;
                        opts.TimestampFormat = "hh:mm:ss | ";
                    }));

                    services
                        .AddScoped<IRequestCache, Mercury.RequestCache.RequestCache>()
                        .AddSingleton<IMessageBroker, MessageBroker>()
                        .AddHostedService<WorkerService>();

                });
    }
}
