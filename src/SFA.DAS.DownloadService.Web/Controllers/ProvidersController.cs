using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Polly;
using SFA.DAS.DownloadService.Api.Types.Roatp;
using SFA.DAS.DownloadService.Services.Interfaces;
using SFA.DAS.Roatp.Api.Client.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Examples;

namespace SFA.DAS.DownloadService.Web.Controllers
{
    [Route("api")]
    [ApiController]
    public class ProvidersController : Controller
    {
        private readonly IRoatpApiClient _apiClient;
        private readonly ILogger<ProvidersController> _log;
        private readonly IRoatpMapper _mapper;
        private readonly IHostingEnvironment _hostingEnv = null;
        private readonly IRetryService _retryService;

        public ProvidersController(ILogger<ProvidersController> log, IRoatpApiClient apiClient, IRoatpMapper mapper, IHostingEnvironment hostingEnv,  IRetryService retryService) 
        {
            _log = log;
            _apiClient = apiClient;
            _mapper = mapper;
            _hostingEnv = hostingEnv;
            _retryService = retryService;
        }

        /////// <summary>
        /////// Check if provider exists
        /////// </summary>
        /////// <param name="ukprn">UKPRN</param>
        [SwaggerResponse((int)HttpStatusCode.NoContent)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Invalid UKPRN (should be 8 numbers long)")]
        [SwaggerOperation("GetOk", "Check a provider exists", Produces = new string[] { "application/json" })]
        [HttpHead("providers/{ukprn}")]
        public async Task<IActionResult> Head(int ukprn)
        {
            _log.LogDebug($"Fetching HEAD for ukprn: [{ukprn}]");
            var resultFromGet = await Get(ukprn);
            return ((ObjectResult)resultFromGet).StatusCode == (int)HttpStatusCode.OK ? NoContent() : resultFromGet;
        }

        /// <summary>
        /// Get a provider
        /// </summary>
        /// <param name="ukprn"></param>
        /// <returns></returns>
        [SwaggerOperation("Get")]
        [SwaggerResponse((int)HttpStatusCode.OK, "OK", typeof(Provider))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Provider not found or start date in future")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Invalid UKPRN (should be 8 numbers long)")]
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(SwaggerHelpers.Examples.ProviderExample))]
        [HttpGet("providers/{ukprn}")]
        public async Task<IActionResult> Get(int ukprn)
        {
            _log.LogDebug($"Fetching GET for ukprn: [{ukprn}]");

            if (ukprn.ToString().Length != 8)
            {
                _log.LogDebug($"Invalid UKPRN (must be 8 numbers): [{ukprn}]");
                return BadRequest("Invalid UKPRN (should be 8 numbers long)");
            }

            IEnumerable<RoatpResult> roatpResults;

            try
            {
                var result = _retryService.RetryPolicy($"<roatpService>//api/v1/download/roatp-summary/{ukprn}").ExecuteAsync(context => _apiClient.GetRoatpSummaryByUkprn(ukprn), new Context());
                roatpResults = result.Result;
            }
            catch (Exception ex)
            {
                _log.LogError($"Unable to retrieve results for roatp with ukprn [{ukprn}]", ex);
                roatpResults = new List<RoatpResult>();
            }

            var provider = _mapper.Map(roatpResults?.FirstOrDefault());

            if (provider == null || !provider.IsDateValid(DateTime.UtcNow))
            {
                _log.LogDebug($"Invalid UKPRN result for ukprn [{ukprn}]: not found or start date in future");
                return NotFound("Provider not found or start date in future");
            }
            provider.Uri = Resolve(provider.Ukprn);
            return Ok(provider);
        }



        /// <summary>
        /// Get active providers
        /// </summary>
        /// <returns></returns>
        [SwaggerOperation("GetAll")]
        [SwaggerResponse((int)HttpStatusCode.OK, "OK", typeof(IEnumerable<Provider>))]
        [HttpGet("providers")]
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(SwaggerHelpers.Examples.ProvidersExample))]

        public async Task<IActionResult> GetAll()
        {
            try
            {
                _log.LogDebug($"Fetching GET ALL for ukprns");

                IEnumerable<RoatpResult> results;
                
                try
                {
                    var result = _retryService.RetryPolicy("<roatpService>//api/v1/download/roatp-summary").ExecuteAsync(context => _apiClient.GetRoatpSummary(), new Context());
                    results = result.Result.Where(x => x.IsDateValid(DateTime.UtcNow));
                }
                catch (Exception ex)
                {
                    _log.LogError("Unable to retrieve results for all roatps", ex);
                    results = new List<RoatpResult>();
                }

                var providers = new List<Provider>();

                foreach (var result in results)
                {
                    var provider = _mapper.Map(result);
                    if (provider == null) continue;
                    provider.Uri = Resolve(provider.Ukprn);
                    providers.Add(provider);
                }

                return Ok(providers);
            }
            catch (Exception e)
            {
                _log.LogError(e, "/providers");
                throw;
            }
        }

        /// <summary>
        /// Check if you can get active providers
        /// </summary>
        [SwaggerOperation("GetAllOk")]
        [SwaggerResponse((int)HttpStatusCode.NoContent)]
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpHead("providers")]
        public async Task<IActionResult> Head()
        {
            var resultFromGet = await GetAll();
            return ((ObjectResult)resultFromGet).StatusCode == (int)HttpStatusCode.OK ? NoContent() : resultFromGet;
        }

        private string Resolve(long ukprn)
        {
            var scheme = ControllerContext.HttpContext.Request.Scheme;
            var path = ControllerContext.HttpContext.Request.Host.Value;
            return $"{scheme}://{path}/api/providers/{ukprn}";
        }
    }
}
