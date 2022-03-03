using System.ComponentModel.DataAnnotations;

namespace Mercury.Plugin
{
    /// <summary>
    /// Result of service call
    /// </summary>
    public enum ResultStatus
    {
        /// <summary>
        /// Service call is still processing
        /// </summary>
        [Display(Name = "Processing")]
        PROCESSING,

        /// <summary>
        /// Service call successful
        /// </summary>
        [Display(Name = "Success")]
        SUCCESS,

        /// <summary>
        /// Service call failed. Check message
        /// </summary>
        [Display(Name = "Failure")]
        FAILURE,

        /// <summary>
        /// Partial results for call available
        /// </summary>
        [Display(Name = "Partial")]
        PARTIAL
    }
}
