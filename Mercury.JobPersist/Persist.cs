using Mercury.Common.Models;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace Mercury.Common.Services
{
    /// <summary>
    /// Redis Persistance Class
    /// </summary>
    public class JobPersist : IJobPersist
    {
        private readonly ILogger<JobPersist> _logger;
        private readonly IRedisClient _client;
        private const string JOBTAG = "job";
        private const int EXPIRY = 600;
        /// <summary>
        /// Main ctor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="client"></param>
        public JobPersist(ILogger<JobPersist> logger, IRedisClient client)
        {
            _logger = logger;
            _client = client;
        }

        /// <summary>
        /// Get job by Guid
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public async Task<IJob?> GetJob(Guid guid)
        {
            var result = await _client.GetDefaultDatabase().GetAsync<Job>("job:" + guid.ToString());
            return result;
        }

        /// <summary>
        /// Get all entries tagged as jobs
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<IJob>> GetJobs()
        {
            var result = await _client.GetDefaultDatabase().GetByTagAsync<Job>(JOBTAG);
            return result.Where(x => x != null) as IEnumerable<Job>;

        }

        /// <summary>
        /// Save to the store
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
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