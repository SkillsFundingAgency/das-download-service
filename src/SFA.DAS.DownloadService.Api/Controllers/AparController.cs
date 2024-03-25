using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.DownloadService.Api.Client.Interfaces;
using SFA.DAS.DownloadService.Api.Infrastructure;
using SFA.DAS.DownloadService.Api.Types;
using SFA.DAS.DownloadService.Api.Types.Assessor;
using SFA.DAS.DownloadService.Api.Types.Roatp;
using SFA.DAS.DownloadService.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace SFA.DAS.DownloadService.Api.Controllers
{
    [Route("api")]
    [ApiController]
    [ApiExplorerSettings(GroupName = @"Providers")]
    public class AparController : Controller
    {
        private readonly IRoatpApiClient _roatpApiClient;
        private readonly IAssessorApiClient _assessorApiClient;

        private readonly ILogger<AparController> _log;
        private readonly IAparMapper _mapper;

        private readonly IDateTimeProvider _dateTimeProvider;

        public AparController(ILogger<AparController> log, IRoatpApiClient roatpApiClient, IAssessorApiClient assessorApiClient, IAparMapper mapper, IDateTimeProvider dateTimeProvider)
        {
            _log = log;
            _roatpApiClient = roatpApiClient;
            _assessorApiClient = assessorApiClient;
            _mapper = mapper;
            _dateTimeProvider = dateTimeProvider;
        }

        /// <summary>
        /// Check if ukprn exists in the APAR
        /// </summary>
        /// <param name="ukprn">The Ukprn to check in the APAR</param>
        /// <returns></returns>
        [SwaggerResponse((int)HttpStatusCode.NoContent)]
        [SwaggerOperation("GetOk", "Check a UKPRN exists in the APAR")]
        [HttpHead("providers/{ukprn}")]
        public async Task<IActionResult> Head(int ukprn)
        {
            _log.LogDebug($"Fetching HEAD for UKPRN: [{ukprn}]");
            var resultFromGet = await Get(ukprn);
            return ((ObjectResult)resultFromGet).StatusCode == (int)HttpStatusCode.OK ? NoContent() : resultFromGet;
        }

        /// <summary>
        /// Check if you can get active APAR entries
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

        /// <summary>
        /// Get an APAR entry by UKPRN
        /// </summary>
        /// <param name="ukprn">The UKPRN to get from the APAR</param>
        /// <returns></returns>
        [SwaggerOperation("Get")]
        [SwaggerResponse((int)HttpStatusCode.OK, "OK", typeof(UkprnAparEntry))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "APAR entry not found or start date in future")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Invalid UKPRN (should be 8 numbers long)")]
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(SwaggerHelpers.Examples.UkprnAparExample))]
        [HttpGet("providers/{ukprn}")]
        public async Task<IActionResult> Get(int ukprn)
        {
            _log.LogDebug($"Fetching APAR entries for UKPRN: [{ukprn}]");

            if (ukprn.ToString().Length != 8)
            {
                _log.LogDebug($"Invalid UKPRN (must be 8 numbers): [{ukprn}]");
                return BadRequest("Invalid UKPRN (should be 8 numbers long)");
            }

            IEnumerable<RoatpResult> roatpResults;
            EpaoResult epaoResult;

            try
            {
                var roatpTask = _roatpApiClient.GetRoatpSummaryByUkprn(ukprn);
                var epaoTask = _assessorApiClient.GetAparSummaryByUkprn(ukprn);

                await Task.WhenAll(roatpTask, epaoTask);

                roatpResults = (await roatpTask)
                    .Where(x => x.IsDateValid(DateTime.UtcNow));

                epaoResult = await epaoTask;
            }
            catch (Exception ex)
            {
                var message = $"Unable to fetch APAR entries from ROATP or EPAO with UKPRN: {ukprn}";
                _log.LogError(ex, message);
                return StatusCode(500, message);
            }

            var roatpResult = roatpResults.FirstOrDefault();
            if (roatpResult == null && epaoResult == null)
            {
                var message = $"APAR entry for UKPRN: {ukprn} is not found or start date in future";
                _log.LogDebug(message);
                return NotFound(message);
            }

            var ukprnApar = _mapper.Map(roatpResults.FirstOrDefault(), epaoResult, Resolve);
            return Ok(ukprnApar);
        }

        [SwaggerOperation("GetLatestTime")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("getLatestTime")]
        public async Task<IActionResult> GetLatestTime()
        {
            _log.LogDebug($"Fetching GET latest change date");

            DateTime? latestChange = _dateTimeProvider.GetCurrentDateTime();
            try
            {
                var roatpResult = await _roatpApiClient.GetLatestNonOnboardingOrganisationChangeDate();
                var aparResult = await _assessorApiClient.GetAparSummaryLastUpdated();

                if (roatpResult.HasValue && aparResult.HasValue)
                {
                    latestChange = roatpResult > aparResult ? roatpResult : aparResult;
                }
                else if (roatpResult.HasValue)
                {
                    latestChange = roatpResult;
                }
                else if (aparResult.HasValue)
                {
                    latestChange = aparResult;
                }
            }
            catch (Exception ex)
            {
                const string message = "Unable to fetch latest APAR summary change date";
                _log.LogError(ex, message);
                return StatusCode(500, message);
            }

            return Ok(latestChange);
        }


        /// <summary>
        /// Gets active APAR entries
        /// </summary>
        /// <returns></returns>
        [SwaggerOperation("GetAll")]
        [SwaggerResponse((int)HttpStatusCode.OK, "OK", typeof(IEnumerable<AparEntry>))]
        [HttpGet("providers")]
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(SwaggerHelpers.Examples.AparExample))]
        public async Task<IActionResult> GetAll()
        {
            _log.LogDebug($"Fetching APAR entries for all UKPRN's");

            try
            {
                var roatpTask = _roatpApiClient.GetRoatpSummary();
                var epaoTask = _assessorApiClient.GetAparSummary();

                await Task.WhenAll(roatpTask, epaoTask);


                var roatpData = await roatpTask;
                var roatpResults = roatpData == null
                    ? Enumerable.Empty<RoatpResult>()
                    : roatpData.Where(x => x.IsDateValid(DateTime.UtcNow));

                var epaoData = await epaoTask;
                var epaoResults = epaoData ?? Enumerable.Empty<EpaoResult>();

                var apprenticeshipProviders = _mapper.Map(roatpResults.ToList(), Resolve).Where(p => p != null);
                var assessmentOrganisations = _mapper.Map(epaoResults.ToList(), Resolve);

                var apar = apprenticeshipProviders.Concat(assessmentOrganisations);

                return Ok(apar);
            }
            catch (Exception ex)
            {
                const string message = "Unable to fetch APAR entries from ROATP or EPAO";
                _log.LogError(ex, message);
                return StatusCode(500, message);
            }
        }

        private string Resolve(long ukprn)
        {
            var scheme = ControllerContext.HttpContext.Request.Scheme;
            var path = ControllerContext.HttpContext.Request.Host.Value;
            return $"{scheme}://{path}/api/providers/{ukprn}";
        }
    }
}
