using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Threading.Tasks;
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
                    /* services
                         .Configure<OnBaseOptions>(hostContext.Configuration.GetSection(nameof(OnBaseOptions)))
                         .Configure<KeywordChangeServiceOptions>(hostContext.Configuration.GetSection(nameof(KeywordChangeServiceOptions)))
                         .Configure<KeywordChangePersistOptions>(hostContext.Configuration.GetSection(nameof(KeywordChangePersistOptions)))
                         .Configure<QueueOptions>(hostContext.Configuration.GetSection(nameof(QueueOptions)));*/

                services.AddLogging(cfg => cfg.AddSimpleConsole(opts =>
                {
                    opts.IncludeScopes = true;
                    opts.SingleLine = true;
                    opts.TimestampFormat = "hh:mm:ss | ";
                }));

                    /* services
                         .AddSingleton<IProducerConsumerCollection<long>, ConcurrentQueue<long>>()
                         .AddSingleton<IKeywordChangePersist, KeywordChangePersist>()
                         .AddScoped<IDatabaseAccess, DatabaseAccess>()
                         .AddScoped<IQueuer, Queuer>()
                         .AddScoped<IUpdateDocument, UpdateDocument>()
                         .AddHostedService<KeywordChangeService>()
                         .AddHostedService<DocumentUpdateService>();*/
            });
}

