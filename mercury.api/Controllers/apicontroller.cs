using Microsoft.AspNetCore.Mvc;
using Mercury.Api.Models;
using Mercury.Api.Logic;

namespace Mercury.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JobsController : ControllerBase
    {
        private readonly ILogger<JobsController> _logger;
        private readonly IJobHandler _jobHandler;

        public JobsController(ILogger<JobsController> logger, IJobHandler jobHandler)
        {
            _logger = logger;
            _jobHandler = jobHandler;
        }

        [HttpGet]
        public IEnumerable<IJob> Get()
        {
            throw new NotImplementedException();
        }

        [HttpGet("{id}")]
        public IJob Get(Guid id)
        {
            throw new NotImplementedException();
        }

        [HttpPost()]
        public void NewJob(IJobRequest req)
        {
            throw new NotImplementedException();   
            // Validate inputs

            // Hand off to job handler

            // Await message

            // Return


        }
    }

}