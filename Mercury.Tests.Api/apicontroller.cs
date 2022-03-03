using Xunit;
using Moq;
using Mercury.Api.Controllers;
using Mercury.Api.Logic;
using Mercury.Api.Models;
using Mercury.Common.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Mercury.Api;
using Microsoft.Extensions.Options;

namespace Mercury.Tests.Api
{
    public class ApiControllerTest
    {
        public Mock<IJobHandler> _jobHandlerMock = new Mock<IJobHandler>();
        public Mock<ILogger<JobsController>> _loggerMock = new Mock<ILogger<JobsController>>();
        public Mock<IOptions<ApiConfig>> _options = new Mock<IOptions<ApiConfig>>();
       
        public ApiConfig _configStub = new ApiConfig() { AllowedServices = new string[] { "PING" } };
        public IOptions<ApiConfig> _optionsStub;

        public ApiControllerTest()
        {
            _optionsStub = Options.Create(_configStub);
        }

        [Fact]
        public async Task Post_CallsJobHandlerOnce()
        {
            //_jobHandlerMock.Setup(jh => jh.EnqueueRequest(It.IsAny<IJobRequest>()));
            var api = new JobsController(_loggerMock.Object, _jobHandlerMock.Object, _optionsStub);
            IActionResult? result = await api.NewJob(new JobRequest() { Services = new List<string>() { "PING"}, Hostname = "example.com" });

            Assert.NotNull(result);
            Assert.True(result is OkObjectResult);
            var objresult = result as OkObjectResult;
            Assert.NotNull(objresult);


            Assert.Equal(StatusCodes.Status200OK, objresult?.StatusCode);

            _jobHandlerMock.Verify(mock => mock.ProcessRequest(It.IsAny<IJobRequest>()), Times.Once);
        }

        [Fact]
        public async Task Post_BadServiceReturns400()
        {
            var api = new JobsController(_loggerMock.Object, _jobHandlerMock.Object, _optionsStub);
            IActionResult? result = await api.NewJob(new JobRequest() { Services = new List<string>() { "BADSERVICE" }, Hostname = "example.com" });

            Assert.True(result is ObjectResult);
            var objresult = result as ObjectResult;
            Assert.NotNull(objresult);


            Assert.Equal(StatusCodes.Status400BadRequest, objresult?.StatusCode);

        }

        [Theory]
        [InlineData(@"!! %/\$$@#@")]
        [InlineData(@"ftp://example.com")]
        [InlineData(@"example.com:80")]
        [InlineData(@"_______")]
        [InlineData(@"")]
        //[InlineData(@"500.11.333.20")] //TODO: Handle invalid large IPs
        [InlineData(@"2001:0db8:85a3:0000:0000:8a2e:0370")]
        [InlineData(@"1200:0000:AB00:1234:O000:2552:7777:1313")]
        [InlineData(@"5")]
        public async Task Post_RejectsBadURLs(string URL)
        {
            //_jobHandlerMock.Setup(jh => jh.EnqueueRequest(It.IsAny<IJobRequest>()));
            var api = new JobsController(_loggerMock.Object, _jobHandlerMock.Object, _optionsStub);
            IActionResult? result = await api.NewJob(new JobRequest() { Services = new List<string>(), Hostname = URL });
            
            
            _jobHandlerMock.Verify(mock => mock.EnqueueRequest(It.IsAny<IJobRequest>()), Times.Never);

            Assert.NotNull(result);
            Assert.True(result is ObjectResult);
            var objresult = result as ObjectResult;
            Assert.NotNull(objresult);


            Assert.Equal(StatusCodes.Status400BadRequest, objresult?.StatusCode);
        }


        [Theory]
        [InlineData(@"example.com")]
        [InlineData(@"localhost")]
        [InlineData(@"127.0.0.1")]
        [InlineData(@"10.0.20.52")]
        [InlineData(@"google.com")]
        [InlineData(@"2001:0db8:85a3:0000:0000:8a2e:0370:7334")]
        [InlineData(@"0:0:0:0:0:0:0:1")]
        [InlineData(@"::1")]
        public async Task Post_AllowsGoodURLs(string URL)
        {
            //_jobHandlerMock.Setup(jh => jh.EnqueueRequest(It.IsAny<IJobRequest>()));
            var api = new JobsController(_loggerMock.Object, _jobHandlerMock.Object, _optionsStub);
            IActionResult? result = await api.NewJob(new JobRequest() { Services = new List<string>(), Hostname = URL });


            Assert.NotNull(result);
            Assert.True(result is OkObjectResult);
            var objresult = result as OkObjectResult;
            Assert.NotNull(objresult);


            Assert.Equal(StatusCodes.Status200OK, objresult?.StatusCode);
        }

        [Fact]
        public async Task GetID_CallsHandlerOnce()
        {
            _jobHandlerMock.Setup(mock => mock.GetJob(It.IsAny<Guid>())).ReturnsAsync( (Guid g) => (new Job() {  ID = g, Status = Common.Enums.JobStatus.Success }));

            var api = new JobsController(_loggerMock.Object, _jobHandlerMock.Object, _optionsStub);
            IActionResult? result = await api.Get(Guid.NewGuid());

            Assert.NotNull(result);
            Assert.True(result is OkObjectResult);
            var objresult = result as OkObjectResult;
            Assert.NotNull(objresult);
            
            Assert.Equal(StatusCodes.Status200OK, objresult?.StatusCode);

            _jobHandlerMock.Verify(mock => mock.GetJob(It.IsAny<Guid>()), Times.Once);

        }

        [Fact]
        public async Task GetID_ReturnsCorrectObject()
        {
            Guid g = Guid.NewGuid();
            _jobHandlerMock.Setup(mock => mock.GetJob(g)).ReturnsAsync(new Job() { ID = g, Status = Common.Enums.JobStatus.Success });

            var api = new JobsController(_loggerMock.Object, _jobHandlerMock.Object, _optionsStub);
            IActionResult? result = await api.Get(g);

            Assert.NotNull(result);
            Assert.True(result is OkObjectResult);

            var objresult = result as OkObjectResult;
            Assert.NotNull(objresult);
            Assert.Equal(StatusCodes.Status200OK, objresult?.StatusCode);
            
            Assert.True(objresult?.Value is IJob);
            Assert.True((objresult?.Value as IJob)?.ID == g);

        }

        [Fact]
        public async Task GetID_Returns404()
        {
            _jobHandlerMock.Setup(mock => mock.GetJob(It.IsAny<Guid>())).ReturnsAsync(null as IJob);

            var api = new JobsController(_loggerMock.Object, _jobHandlerMock.Object, _optionsStub);
            IActionResult? result = await api.Get(Guid.NewGuid());

            Assert.NotNull(result);
            Assert.True(result is NotFoundObjectResult);

            var objresult = result as NotFoundObjectResult;
            Assert.NotNull(objresult);
            Assert.Equal(StatusCodes.Status404NotFound, objresult?.StatusCode);


        }

    }
}