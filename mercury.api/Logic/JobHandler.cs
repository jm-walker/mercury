﻿using Mercury.Api.Models;
using Mercury.Common.Services;
using Mercury.Common.Models;
using Mercury.MessageBroker.Exceptions;
using System.Linq;
using Mercury.Plugin;

namespace Mercury.Api.Logic
{
    public class JobHandler : IJobHandler
    {
        private readonly IJobPersist _persistance;
        private readonly IMessageBroker _broker;
        private readonly ILogger _logger;

        public JobHandler(
            ILogger<JobHandler> logger,
            IJobPersist persistance,
            IMessageBroker broker
            )
        {
            _logger = logger;
            _persistance = persistance;
            _broker = broker;
        }

        /// <summary>
        /// Async submit job request
        /// </summary>
        /// <param name="jobRequest">Request containing all services to serve</param>
        /// <returns>ID of request</returns>
        public async Task<Guid> EnqueueRequest(IJobRequest jobRequest)
        {

            Guid ID = Guid.NewGuid();
            await SaveAndProcess(ID, jobRequest);
            return ID;
        }

        /// <summary>
        /// Synchronous processing of request - waits for finish
        /// </summary>
        /// <param name="jobRequest">Request containing all services to serve</param>
        /// <returns>Job results</returns>
        public async Task<IJob> ProcessRequest(IJobRequest jobRequest)
        {
            Guid ID = Guid.NewGuid();
            TaskCompletionSource<IJob> taskCompletionSource = new TaskCompletionSource<IJob>();
            
            //  Listen to correlation response queue
            _broker.RegisterJobCompleteListener(ID.ToString(), async (job) =>
            {
                // Set the completion
                return taskCompletionSource.TrySetResult(job);
            });

            // Now that listener is in place, submit the job
            await SaveAndProcess(ID, jobRequest);

            // Wait for response to return
            return await taskCompletionSource.Task;

        }

        private async Task SaveAndProcess(Guid ID, IJobRequest jobRequest)
        {
            
            if (jobRequest.Hostname == null)
            {
                throw new ArgumentNullException(nameof(jobRequest.Hostname));
            }
            // Add to persistance 
            await _persistance.SaveJob(new Job()
            {
                ID = ID,
                Status = Common.Enums.JobStatus.Processing,
                URL = jobRequest.Hostname,
                Results = jobRequest.Services.Select(s =>( new ServiceResult() { Service = s, Status = ResultStatus.PROCESSING })).ToList()


            }); 

            foreach (var svc in jobRequest.Services)
            {
                try
                {
                    _logger.LogDebug($"Sending {ID} to broker for {svc}");
                    // Enqueue Message
                    _broker.EnqueueServiceRequest(new ServiceJobMessage(ID, svc, jobRequest.Hostname));
                }
                catch( InvalidQueueException ex )
                {
                    //TODO: Set failure message
                    _logger.LogInformation($"Queue was not available to proces {svc}");
                }
            }
        }

        public async Task<IJob?> GetJob(Guid jobId)
        {
            return await _persistance.GetJob(jobId);
        }

        public async Task<IEnumerable<IJob>> GetJobs()
        {
            return await _persistance.GetJobs();
        }
    }
}
