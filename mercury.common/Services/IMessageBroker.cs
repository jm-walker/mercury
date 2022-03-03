using Mercury.Common.Models;
using Mercury.Plugin;

namespace Mercury.Common.Services
{
    public interface IMessageBroker
    {
        void EnqueueServiceRequest(IServiceJobMessage msg);
        void EnqueueServiceResponse(IServiceResult msg, string ID);
        void EnqueueJobCompletion(IJob job);

        void RegisterServiceRequestListener(string service, Func<IServiceJobMessage, Task<bool>> receive);
        void RegisterServiceResponseListener(Func<IServiceResult, string, Task<bool>> receive);
        void RegisterJobCompleteListener(string ID, Func<IJob, Task<bool>> receive);

    }
}
