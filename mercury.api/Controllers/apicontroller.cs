using Microsoft.AspNetCore.Mvc;
using Mercury.Api.Models;

using Mercury.Api.Logic;
using Mercury.Common.Models;
using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Options;

namespace Mercury.Api.Controllers
{
    /// <summary>
    /// Main Job API Controller
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/[controller]")]
    public class JobsController : ControllerBase
    {
        private readonly ILogger<JobsController> _logger;
        private readonly IJobHandler _jobHandler;
        private readonly ApiConfig _config;

        /// <summary>
        /// Jobs Controller constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="jobHandler"></param>
        /// <param name="config"></param>
        public JobsController(ILogger<JobsController> logger, IJobHandler jobHandler, IOptions<ApiConfig> config)
        {
            _logger = logger;
            _jobHandler = jobHandler;
            _config = config.Value;
        }

        /// <summary>
        /// Retrieve Previously run job from the temporary store
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(Job))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type=typeof(Error))]
        public async Task<IActionResult> Get(Guid id)
        {
            IJob? result = await _jobHandler.GetJob(id);
            if (result == null)
            {
                return NotFound(new Error() { Message = "Not Found" });
            }
            else
            { 
                return Ok(result);
            }

        }

        /// <summary>
        /// Submit a job for processing
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(Job))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type=typeof(Error))]
        public async Task<IActionResult> NewJob(JobRequest req)
        {
            if(String.IsNullOrEmpty(req.Hostname))
            {
                return new BadRequestObjectResult(new Error() { Message = "Bad Hostname" });
            }
            UriHostNameType hostType = Uri.CheckHostName(req.Hostname);
            if( hostType == UriHostNameType.Unknown)
            {
                return new BadRequestObjectResult(new Error() { Message = "Bad Hostname" });
            }
            else if( (hostType == UriHostNameType.IPv4 || 
                     hostType == UriHostNameType.IPv6 ) &&
                     !IsValidIPAddress(req.Hostname))
            {
                return new BadRequestObjectResult(new Error() { Message = "Bad IP Address" });
            }

            if (req.Services == null || !req.Services.Any())
            {
                req.Services = _config.DefaultServices.ToList();
            }

            List<string> badServices = new();
            foreach( string s in req.Services )
            {
                if( !_config.AllowedServices.Contains(s))
                {
                    badServices.Add(s);
                }
            }
            if( badServices.Count > 0 )
            {
                return new BadRequestObjectResult(new Error() { Message = $"Bad services requested: {String.Join(',', badServices)}" });
            }

            


            // Hand off to job handler
            //await _jobHandler.EnqueueRequest(req);
            IJob results = await _jobHandler.ProcessRequest(req);
            // Await message

            // Return
            return new OkObjectResult(results);

        }


        private static bool IsValidIPAddress(string IP)
        {
            if( string.IsNullOrEmpty(IP))
            {
                return false;
            }

            if (!IPAddress.TryParse(IP, out IPAddress? ip))
            {
                return false;
            }

            if (ip.AddressFamily == AddressFamily.InterNetwork &&
               IP.Count( c => c == '.') != 3 )
            {
                return false;
            }

            return true;

        }
    }

}