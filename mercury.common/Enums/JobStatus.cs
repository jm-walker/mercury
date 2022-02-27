using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercury.Common.Enums
{
    public enum JobStatus
    {
        [Display(Name = "Success")]
        Success,

        [Display(Name = "Failure")]
        Failure,

        [Display(Name = "Processing")]
        Processing
    }
}
