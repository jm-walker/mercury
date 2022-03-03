using Mercury.Common.Models;
using Mercury.Common.Services;
using Mercury.Plugin;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Mercury.JobWorker
{
    internal class WorkerService : IHostedService
    {
        private readonly ILogger<WorkerService> _logger;
        private readonly IMessageBroker _broker;
        private readonly IRequestCache _requestCache;
        private readonly WorkerConfig _config;
        private readonly Dictionary<string, IPlugin> _plugins = new Dictionary<string, IPlugin>();

        public WorkerService(
            ILogger<WorkerService> logger,
            IMessageBroker broker,
            IRequestCache requestCache,
            IOptions<WorkerConfig> config)
        {
            _logger = logger;
            _broker = broker;
            _requestCache = requestCache;
            _config = config.Value;
        }

        private async Task<IServiceResult> Ping(string host)
        {
            const string SERVICE = "PING";
            try
            {
                using (var pinger = new Ping())
                {
                    PingReply reply = await pinger.SendPingAsync(host);
                    return new ServiceResult()
                    {
                        Service = SERVICE,
                        Result = new
                        {
                            rtt = reply.RoundtripTime,
                            ip = reply.Address.ToString(),
                            status = reply.Status.ToString()
                        },
                        ResultMessage = "Success",
                        Status = ResultStatus.SUCCESS
                    };
                }
            }
            catch (PingException ex)
            {
                return new ServiceResult() { Service = SERVICE, Result = null, ResultMessage = ex.Message, Status = ResultStatus.FAILURE };
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Starting Job Worker");
            string rootPath = Path.Combine(AppContext.BaseDirectory, "");
            _logger.LogDebug($"Looking for plugins in {rootPath}");
            IEnumerable<IPlugin> commands = _config.Plugins.SelectMany(p =>
           {
               _logger.LogDebug($"Attempting to load plugin {p}");
               Assembly pluginAssembly = LoadPlugin(p);
               return CreatePlugin(pluginAssembly);

           });
            foreach( var p in commands )
            {
                _plugins.Add(p.Name, p);
            }

           
            _broker.RegisterServiceRequestListener("PING", ReceiveWork);
            foreach(IPlugin plugin in commands)
            {
                _broker.RegisterServiceRequestListener(plugin.Name, ReceiveWork);
            }




            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (_broker is IDisposable)
            {
                ((IDisposable)_broker).Dispose();
            }
            return Task.CompletedTask;
        }

        private async Task<bool> ReceiveWork(IServiceJobMessage msg)
        {
            _logger.LogDebug($"Received {msg.URL} for {msg.ID}");

            IServiceResult? result = await _requestCache.CheckCache(msg.Service, msg.URL);

            if (result == null)
            {
                result = await DispatchToServices(msg.Service, msg.URL);
                await _requestCache.Add(msg.Service, msg.URL, result);
            }
            else
            {
                result.FromCache = true;
            }


            _broker.EnqueueServiceResponse(result, msg.ID.ToString());


            return await Task.FromResult(true);
        }

        private async Task<IServiceResult> DispatchToServices(string service, string URL)
        {
            if( service == "PING" )
            {
                return await Ping(URL);
            }
            else if( _plugins.Keys.Contains(service) )
            {
                return await _plugins[service].QueryURL(URL);
            }
            else
            { 
                return new ServiceResult()
                {
                    Service = service,
                    Result = null,
                    ResultMessage = "Bad service requested",
                    Status = ResultStatus.FAILURE
                };
            }
        }


        static Assembly LoadPlugin(string relativePath)
        {
            // Navigate up to the solution root
            string pluginLocation = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, relativePath.Replace('\\', Path.DirectorySeparatorChar)));
            Console.WriteLine($"Loading commands from: {pluginLocation}");
            PluginLoadContext loadContext = new PluginLoadContext(pluginLocation);
           
            return loadContext.LoadFromAssemblyPath(pluginLocation);
        }
        IEnumerable<IPlugin> CreatePlugin(Assembly assembly)
        {
            int count = 0;


            foreach (Type type in assembly.GetTypes())
            {

                _logger.LogDebug($"Checking type {type.Name} of {assembly.GetName().Name}");
                _logger.LogDebug($"\tInterfaces: " + String.Join(',', type.GetInterfaces().Select(i => i.Name)));
                _logger.LogDebug($"Found: {type.GetInterfaces().Contains(typeof(IPlugin))}");
                _logger.LogDebug($"Found {type.GetInterfaces().FirstOrDefault()?.Module?.FullyQualifiedName} = {typeof(IPlugin).Module.FullyQualifiedName}: {type.GetInterfaces().FirstOrDefault() == typeof(IPlugin)}");
                _logger.LogDebug($"{typeof(IPlugin).IsAssignableFrom(type)}");
                
                if(typeof(IPlugin).IsAssignableFrom(type)) 
                {
                    _logger.LogDebug($"Found plugin in {type.Name}");
                    IPlugin? result = Activator.CreateInstance(type) as IPlugin;
                    if (result != null)
                    {
                        count++;
                        yield return result;
                    }
                }
            }
            if( count == 0)
            {
               string availableTypes = string.Join(",", assembly.GetTypes().Select(t => t.FullName));
               _logger.LogDebug($"Can't find any type which implements ICommand in {assembly} from {assembly.Location}.\n" +
                    $"Available types: {availableTypes}");
            }
        }
    }
}
