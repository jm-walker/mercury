using Mercury.Common.Enums;
using Mercury.Plugin;

namespace Mercury.Common.Models
{

    /// <summary>
    /// A job and its results
    /// </summary>
    public class Job : IJob
    {
        /// <summary>
        /// Unique ID of the Job
        /// </summary>
        public Guid ID { get; set; }
        /// <summary>
        /// URL/Hostname requested
        /// </summary>
        public string URL { get; set; } = string.Empty;

        /// <summary>
        /// Overall status of the job
        /// </summary>
        public JobStatus Status { get; set; } = JobStatus.Processing;

        /// <summary>
        /// The results/status of each requested service
        /// </summary>
        public IList<ServiceResult> Results { get; set; } = new List<ServiceResult>();
        
    }
}