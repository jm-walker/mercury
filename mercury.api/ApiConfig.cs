namespace Mercury.Api
{
    /// <summary>
    /// Config for main API
    /// </summary>
    public class ApiConfig
    {
        /// <summary>
        /// List of allowed service names to request
        /// </summary>
        public string[] AllowedServices { get; set; } = new string[] { };

        /// <summary>
        /// List of default services to run if none provides
        /// </summary>
        public string[] DefaultServices { get; set; } = new string[] { };
    }
}
