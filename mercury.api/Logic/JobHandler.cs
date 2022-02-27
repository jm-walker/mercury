using Mercury.Api.Models;
using Mercury.Common.Services;

namespace Mercury.Api.Logic
{
    public class JobHandler : IJobHandler
    {
        private readonly IJobPersist _persistance;
        private IMessageBroker _broker;

        public JobHandler(
            IJobPersist persistance,
            IMessageBroker broker
            )
        {
            _persistance = persistance;
            _broker = broker;
        }
        public void EnqueueRequest(IJobRequest jobRequest)
        {
            throw new NotImplementedException();
        }

        public IJob? GetJob(Guid jobId)
        {
            return _persistance.GetJob(jobId);
        }

        public IEnumerable<IJob> GetJobs()
        {
            return _persistance.GetJobs();
        }
    }
}
