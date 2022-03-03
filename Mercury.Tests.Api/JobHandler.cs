using Xunit;
using Moq;
using Mercury.Api.Controllers;
using Mercury.Api.Logic;
using Mercury.Api.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using Mercury.Common.Services;
using System.Linq;
using Mercury.Common.Models;

namespace Mercury.Tests.Api
{
    public class JobHandlerTest
    {
        
        public Mock<ILogger<JobHandler>> _loggerMock = new Mock<ILogger<JobHandler>>();
        public Mock<IJobPersist> _persistMock = new Mock<IJobPersist>();
        public Mock<IMessageBroker> _messageBrokerMock = new Mock<IMessageBroker>();

        private IJobRequest _jobRequest = new JobRequest()
        {
            Hostname = "localhost",
            Services = new List<string> { "PING", "GEOIP", "PORTSCAN" }
        };
        private IJobRequest _nullJobRequest = new JobRequest()
        {
            Hostname = null,
            Services = new List<string> { "PING", "GEOIP", "PORTSCAN" }
        };

        [Fact]
        public async void Enqueue_CallsSvcTimes()
        {

            var handler = new JobHandler(_loggerMock.Object, _persistMock.Object, _messageBrokerMock.Object);
            Guid result = await handler.EnqueueRequest(_jobRequest);

            _messageBrokerMock.Verify( 
                mock => mock.EnqueueServiceRequest(It.IsAny<IServiceJobMessage>()), 
                Times.Exactly(_jobRequest.Services.Count()));

        }

        [Fact]
        public async void Enqueue_PersistsID()
        {

            IJob? createdJob = null;

            _persistMock.Setup(p => p.SaveJob(It.IsAny<IJob>()))
                .Callback<IJob>((o) => createdJob = o);

            var handler = new JobHandler(_loggerMock.Object, _persistMock.Object, _messageBrokerMock.Object);
            Guid result = await handler.EnqueueRequest(_jobRequest);

            _persistMock.Verify(mock => mock.SaveJob(It.IsAny<IJob>()), Times.Once());

            Assert.NotNull(createdJob);
            Assert.Equal(createdJob?.ID, result);

        }

        [Fact]
        public async void Enqueue_ErrorOnNullHostname()
        {
            var handler = new JobHandler(_loggerMock.Object, _persistMock.Object, _messageBrokerMock.Object);
            await Assert.ThrowsAsync<ArgumentNullException>( async () =>
                await handler.EnqueueRequest(_nullJobRequest)
            );
        }




    }
}