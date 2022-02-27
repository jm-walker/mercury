namespace Mercury.Api.Models
{
    public class JobRequest : IJobRequest
    {
        private List<string> _services = new List<string>();
        public string? URL { get; set; }
        public IEnumerable<string> Services { get => _services; set => _services = value.ToList(); }
    }
}