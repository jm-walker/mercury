using Mercury.Common.Enums;
using Mercury.Plugin;

namespace Mercury.Common.Models
{
    public interface IJob
    {
        public Guid ID { get; set; }
        public string URL { get; set; }
        public JobStatus Status { get; set; }
        public IList<ServiceResult> Results { get; set; }
    }


}

