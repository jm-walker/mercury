using Mercury.Api.Models;

namespace Mercury.Common.Services
{
    public class Persist : IJobPersist
    {

        public IJob? GetJob(Guid guid)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IJob> GetJobs()
        {
            throw new NotImplementedException();
        }

        public void SaveJob(IJob job)
        {
            throw new NotImplementedException();
        }
    }
}