using Mercury.Common.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercury.CorrelationWorker
{
    internal class CorrelationService : IHostedService, IDisposable
    {
        private ILogger<CorrelationService> _logger;
        private IMessageBroker _broker;
        private IRequestCache _requestCache;

        public CorrelationService(
            ILogger<CorrelationService> logger,
            IMessageBroker broker,
            IRequestCache requestCache)
        {
            _logger = logger;
            _broker = broker;
            _requestCache = requestCache;
        }



        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
