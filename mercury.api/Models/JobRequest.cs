using System.ComponentModel.DataAnnotations;

namespace Mercury.Api.Models
{
    /// <summary>
    /// A request to run services
    /// </summary>
    public class JobRequest : IJobRequest
    {
        /// <summary>
        /// Host or IP to query
        /// </summary>
        [Required]
        public string Hostname { get; set; } = String.Empty;

        /// <summary>
        /// List of services to retrieve. Default: [[DEFAULT_SERVICES]]. All: [[ALL_SERVICES]]
        /// </summary>
        public IEnumerable<string>? Services { get; set; } = new List<string>();
    }
}