

namespace Mercury.Plugin
{
    /// <summary>
    /// Result of a single service call
    /// </summary>
    public class ServiceResult : IServiceResult
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public ServiceResult() { }
        /// <summary>
        /// Copy other object into concrete - helper for serialization
        /// </summary>
        /// <param name="orig"></param>
        public ServiceResult(IServiceResult orig)
        {
            this.Service = orig.Service;
            this.Status = orig.Status;
            this.ResultMessage = orig.ResultMessage;
            this.Result = orig.Result;
            this.FromCache = orig.FromCache;
        }
        /// <summary>
        /// Service Name
        /// </summary>
        public string Service { get; set; } = String.Empty;
        /// <summary>
        /// Status of request
        /// </summary>
        public ResultStatus Status { get; set; }
        /// <summary>
        /// Short result message
        /// </summary>
        public string ResultMessage { get; set; } = String.Empty;
        /// <summary>
        /// Dynamic results field - structure depends on service called
        /// </summary>
        public dynamic? Result { get; set; }
        /// <summary>
        /// Flag if it was served from cache
        /// </summary>
        public bool FromCache { get; set; } = false;


    }
}
