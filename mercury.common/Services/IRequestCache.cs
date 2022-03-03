using Mercury.Plugin;

namespace Mercury.Common.Services
{
    public interface IRequestCache
    {
        Task<IServiceResult?> CheckCache(string service, string url);
        Task Add(string service, string url, IServiceResult result);
    }
}
