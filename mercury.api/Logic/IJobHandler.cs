using Mercury.Common.Models;
using Mercury.Api.Models;

namespace Mercury.Api.Logic
{
    public interface IJobHandler
    {
        Task<Guid> EnqueueRequest(IJobRequest jobRequest);
        Task<IJob> ProcessRequest(IJobRequest jobRequest);
        Task<IJob?> GetJob(Guid jobId);
        Task<IEnumerable<IJob>> GetJobs();
    }
}
