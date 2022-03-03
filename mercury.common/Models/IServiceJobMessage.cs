namespace Mercury.Common.Models
{
    public interface IServiceJobMessage
    {
        public Guid ID { get; set; }
        public string Service { get; set; }
        public string URL { get; set; }
    }
}
