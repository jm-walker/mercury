using System.ComponentModel.DataAnnotations;
namespace Mercury.Common.Enums
{
    /// <summary>
    /// Overall status of a job
    /// </summary>
    public enum JobStatus
    {
        /// <summary>
        /// Successful
        /// </summary>
        [Display(Name = "Success")]
        Success,

        /// <summary>
        /// Job Failed
        /// </summary>
        [Display(Name = "Failure")]
        Failure,

        /// <summary>
        /// Job currently running
        /// </summary>
        [Display(Name = "Processing")]
        Processing,

        /// <summary>
        /// Complete, but only partially successful
        /// </summary>
        [Display(Name = "Partial")]
        Partial
    }
}
