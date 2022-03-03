namespace Mercury.Api.Models
{
    /// <summary>
    /// A Request to run services
    /// </summary>
    public interface IJobRequest
    {
        string Hostname { get; set; }
        IEnumerable<string>? Services { get; set; }
    }
}