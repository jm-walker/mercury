using Mercury.Common.Enums;

namespace Mercury.Api.Models
{
    public interface IJob
    {
        public Guid ID { get; set; }
        public JobStatus MyProperty { get; set; }
        // TODO: Represent disperate results
    }
}