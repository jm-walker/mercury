using Mercury.Api.Models;

namespace Mercury.Api.Logic
{
    public interface IJobHandler
    {
        void EnqueueRequest(IJobRequest jobRequest);
        IJob? GetJob(Guid jobId);
        IEnumerable<IJob> GetJobs();
    }
}
