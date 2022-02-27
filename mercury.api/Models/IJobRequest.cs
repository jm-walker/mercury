namespace Mercury.Api.Models
{
    public interface IJobRequest
    {
        string? URL { get; set; }
        IEnumerable<string> Services { get; set; }
    }
}