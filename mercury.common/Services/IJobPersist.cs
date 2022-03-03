using Mercury.Common.Models;

namespace Mercury.Common.Services
{
    public interface IJobPersist
    {
        Task SaveJob(IJob job);
        Task<IJob?> GetJob(Guid guid);
        Task<IEnumerable<IJob>> GetJobs();
    }
}
