namespace Mercury.Common.Models
{
    /// <summary>
    /// Job request message structure
    /// </summary>
    public class ServiceJobMessage : IServiceJobMessage
    {
        /// <summary>
        /// Common ctor
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="service"></param>
        /// <param name="url"></param>
        public ServiceJobMessage(Guid guid, string service, string url)
        {
            ID = guid;
            Service = service;
            URL = url;
        }
        /// <summary>
        /// Default ctor
        /// </summary>
        public ServiceJobMessage()
        { }

        /// <summary>
        /// Service requested
        /// </summary>
        public string Service { get; set; } = String.Empty;
        /// <summary>
        /// Hostname or IP of the request
        /// </summary>
        public string URL { set; get; } = String.Empty;
        /// <summary>
        /// Unique ID for the request
        /// </summary>
        public Guid ID { get; set; }
    }
}
