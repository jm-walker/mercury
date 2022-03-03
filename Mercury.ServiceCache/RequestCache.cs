using Mercury.Common.Services;
using Mercury.Plugin;
using Microsoft.Extensions.Logging;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace Mercury.RequestCache
{
    public class RequestCache : IRequestCache
    {

        private readonly ILogger<IRequestCache> _logger;
        private readonly IRedisClient _client;
        private const string TAG = "SVC_CACHE";
        private const int EXPIRY = 3600;
        public RequestCache(ILogger<RequestCache> logger, IRedisClient client)
        {
            _logger = logger;
            _client = client;
        }


        public async Task<IServiceResult?> CheckCache(string service, string url)
        {
            return await _client.GetDefaultDatabase().GetAsync<ServiceResult?>(service + ":" + url);
        }

        public async Task Add(string service, string url, IServiceResult result)
        {
            await _client.GetDefaultDatabase().AddAsync(
                     service + ":" + url,
                     result,
                     DateTimeOffset.Now.AddSeconds(EXPIRY), //Only keep job data around for a bit  TODO: Make Configurable
                     StackExchange.Redis.When.Always,
                     StackExchange.Redis.CommandFlags.None,
                     new HashSet<string> { TAG }
                 );
        }
    }
}