using Mercury.Common.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.System.Text.Json;

namespace Mercury.CorrelationWorker
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
                    builder.AddJsonFile("appsettings.json", optional: false);
                    builder.AddEnvironmentVariables();
                })
                .ConfigureServices((hostContext, services) =>
                {

                    // Logging
                    services.AddLogging(cfg => cfg.AddSimpleConsole(opts =>
                    {
                        opts.IncludeScopes = true;
                        opts.SingleLine = true;
                        opts.TimestampFormat = "hh:mm:ss | ";
                    }));


                    // Redis Services
                    services.Configure<RedisConfiguration>(
                            hostContext.Configuration.GetRequiredSection("Redis")
                    );

                    services
                            .AddSingleton((ctx) =>
                            {
                                return ctx.GetRequiredService<IOptions<RedisConfiguration>>().Value;
                            });

                    services.AddStackExchangeRedisExtensions<SystemTextJsonSerializer>((ctx) => new List<RedisConfiguration> { ctx.GetRequiredService<IOptions<RedisConfiguration>>().Value });


                    // MQ Services
                    services.Configure<MessageBrokerConfig>(
                            hostContext.Configuration.GetRequiredSection("MQ")
                    );
                    services
                            .AddSingleton<IConnectionFactory, ConnectionFactory>((ctx) =>
                            {
                                var config = ctx.GetRequiredService<IOptions<MessageBrokerConfig>>().Value;
                                return new ConnectionFactory() { HostName = config.Hostname, UserName = config.Username, Password = config.Password, Port = config.Port };
                            });

                    // Rest of DI
                    services
                        .AddScoped<IJobPersist, JobPersist>()
                        .AddScoped<IMessageBroker, Broker>()
                        .AddHostedService<CorrelationService>();

                });
    }
}
