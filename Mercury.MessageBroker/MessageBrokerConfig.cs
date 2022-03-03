namespace Mercury.Common.Services
{
    /// <summary>
    /// Config structure for appsetting.json
    /// </summary>
    public class MessageBrokerConfig
    {
        /// <summary>
        /// MQ hostname
        /// </summary>
        public string Hostname { get; set; } = string.Empty;

        /// <summary>
        /// MQ Username
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// MQ Password
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// MQ port - default 5672
        /// </summary>
        public int Port { get; set; } = 5672;
    }
}
