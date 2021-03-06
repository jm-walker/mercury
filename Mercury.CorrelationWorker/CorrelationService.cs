using Mercury.Common.Enums;
using Mercury.Common.Models;
using Mercury.Common.Services;
using Mercury.Plugin;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Mercury.CorrelationWorker
{
    internal class CorrelationService : IHostedService
    {
        private readonly ILogger<CorrelationService> _logger;
        private readonly IMessageBroker _broker;
        private readonly IJobPersist _jobPersist;

        public CorrelationService(
            ILogger<CorrelationService> logger,
            IMessageBroker broker,
            IJobPersist jobPersist)
        {
            _logger = logger;
            _broker = broker;
            _jobPersist = jobPersist;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Starting Job Worker");
            _broker.RegisterServiceResponseListener(ReceiveWork);
            return Task.CompletedTask;
        }

        

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (_broker is IDisposable b)
            {
                b.Dispose();
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Main Work Loop
        /// </summary>
        /// <param name="arg">Service Result</param>
        /// <param name="JobID">ID for job</param>
        /// <returns></returns>
        private async Task<bool> ReceiveWork(IServiceResult arg, string JobID)
        {
            _logger.LogDebug($"Received {arg.Service} for {JobID}");

            // Find the job, otherwise re-store a blank one for failure processing
            IJob? job = await _jobPersist.GetJob(Guid.Parse(JobID));
            if (job == null) // Lost trick of the job?
            {
                _logger.LogWarning($"JobID wasn't found in the persistent store. {JobID}");
                var newJob = new Job()
                {
                    ID = Guid.Parse(JobID),
                    Results = new List<ServiceResult>(),
                    Status = Common.Enums.JobStatus.Failure
                };
                await _jobPersist.SaveJob(newJob);
                _broker.EnqueueJobCompletion(newJob);
                return true;
            }

            // Remove old status and add current one
            job.Results.Remove(job.Results.First(j => j.Service == arg.Service));
            job.Results.Add(new ServiceResult(arg));


            // If it's done
            if (job.Results.All(j => j.Status != ResultStatus.PROCESSING))
            {
                job.Status = determineJobStatus(job.Results);
                await _jobPersist.SaveJob(job);
                _broker.EnqueueJobCompletion(job);
            }
            else
            {
                await _jobPersist.SaveJob(job);
            }

            return true;

        }

        private static JobStatus determineJobStatus( IEnumerable<IServiceResult> results )
        {
            bool sawSuccess = false, sawFailure = false;
            foreach (var r in results)
            {
                sawSuccess = sawSuccess || r.Status == ResultStatus.SUCCESS;
                sawFailure = sawFailure || r.Status == ResultStatus.FAILURE;
                if( sawSuccess && sawFailure )
                {
                    break;
                }
            }

            if (sawSuccess && !sawFailure)
            {
                return JobStatus.Success;
            }
            else if (sawFailure && !sawSuccess)
            {
                return JobStatus.Failure;
            }
            
            return JobStatus.Partial;
        }
       
    }
}
