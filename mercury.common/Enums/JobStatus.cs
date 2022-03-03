using System.ComponentModel.DataAnnotations;
namespace Mercury.Common.Enums
{
    public enum JobStatus
    {

        [Display(Name = "Success")]
        Success,

        [Display(Name = "Failure")]
        Failure,

        [Display(Name = "Processing")]
        Processing,

        [Display(Name = "Partial")]
        Partial
    }
}
