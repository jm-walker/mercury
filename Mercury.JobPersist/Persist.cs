using Mercury.Common.Models;
using Mercury.Common.Services;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace Mercury.JobPersist
{
    public class JobPersist : IJobPersist
    {
        private readonly ILogger<JobPersist> _logger;
        private readonly IRedisClient _client;
        private const string JOBTAG = "job";
        private const int EXPIRY = 600;
        public JobPersist(ILogger<JobPersist> logger, IRedisClient client)
        {
            _logger = logger;
            _client = client;
        }

        public async Task<IJob?> GetJob(Guid guid)
        {
            var result = await _client.GetDefaultDatabase().GetAsync<Job>("job:" + guid.ToString());
            return result;
        }

        public async Task<IEnumerable<IJob>> GetJobs()
        {
            var result = await _client.GetDefaultDatabase().GetByTagAsync<Job>(JOBTAG);
            return result.Where(x => x != null) as IEnumerable<Job>;

        }

        public async Task SaveJob(IJob job)
        {
            // TODO: Updating with this could cause race condition with multiple correlation agents
            await _client.GetDefaultDatabase().AddAsync(
                "job:" + job.ID.ToString(),
                job,
                DateTimeOffset.Now.AddSeconds(EXPIRY), //Only keep job data around for a bit  TODO: Make Configurable
                When.Always,
                CommandFlags.None,
                new HashSet<string>() { JOBTAG }
            );
        }
    }
}