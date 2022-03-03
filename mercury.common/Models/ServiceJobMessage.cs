namespace Mercury.Common.Models
{
    public class ServiceJobMessage : IServiceJobMessage
    {
        public ServiceJobMessage(Guid guid, string service, string url)
        {
            ID = guid;
            Service = service;
            URL = url;
        }
        public ServiceJobMessage()
        { }

        public string Service { get; set; } = String.Empty;
        public string URL { set; get; } = String.Empty;
        public Guid ID { get; set; }
    }
}
