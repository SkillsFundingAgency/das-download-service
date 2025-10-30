using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.DownloadService.Api.Client.Interfaces;
using SFA.DAS.DownloadService.Api.Infrastructure;
using SFA.DAS.DownloadService.Api.SwaggerHelpers.Examples;
using SFA.DAS.DownloadService.Api.Types;
using SFA.DAS.DownloadService.Api.Types.Roatp;
using SFA.DAS.DownloadService.Services.Interfaces;
using Swashbuckle.AspNetCore.Filters;

namespace SFA.DAS.DownloadService.Api.Controllers
{
    [Route("api")]
    [ApiController]
    [ApiExplorerSettings(GroupName = @"Providers")]
    public class AparController : ControllerBase
    {
        private readonly IRoatpApiClient _roatpApiClient;
        private readonly IAparMapper _mapper;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ILogger<AparController> _logger;

        public AparController(IRoatpApiClient roatpApiClient, IAparMapper mapper, IDateTimeProvider dateTimeProvider, ILogger<AparController> logger)
        {
            _roatpApiClient = roatpApiClient;
            _mapper = mapper;
            _dateTimeProvider = dateTimeProvider;
            _logger = logger;
        }

        /// <summary>
        /// Check if UKPRN exists in the APAR
        /// </summary>
        /// <param name="ukprn">The UKPRN to check for in the APAR</param>
        /// <returns></returns>
        [HttpHead("providers/{ukprn}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]//UPDATE THE OTHER ENDPOINTS TO MATCH THIS PATTERN
        public async Task<IActionResult> Head(int ukprn)
        {
            _logger.LogInformation("Fetching HEAD for UKPRN: {Ukprn}", ukprn);
            var resultFromGet = await Get(ukprn);
            return ((ObjectResult)resultFromGet).StatusCode == (int)HttpStatusCode.OK ? NoContent() : resultFromGet;
        }

        /// <summary>
        /// Check if you can get active APAR entries
        /// </summary>
        [HttpHead("providers")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ApiExplorerSettings(IgnoreApi = true)]
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
        [ProducesResponseType(typeof(UkprnAparEntry), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(UkprnAparExample))]
        [HttpGet("providers/{ukprn}")]
        public async Task<IActionResult> Get(int ukprn)
        {
            _logger.LogInformation("Fetching APAR entries for UKPRN: {Ukprn}", ukprn);

            if (ukprn.ToString().Length != 8)
            {
                var message = "Invalid UKPRN (should be 8 numbers): {ukprn}";
                return BadRequest(message);
            }

            try
            {
                var roatpResult = (await _roatpApiClient.GetRoatpSummaryByUkprn(ukprn))
                    .FirstOrDefault(x => x.IsDateValid(_dateTimeProvider.GetCurrentDateTime()));

                if (roatpResult == null)
                {
                    var message = "APAR entry from RoATP for UKPRN: {ukprn} is not found or start date in future";
                    return NotFound(message);
                }

                var ukprnApar = _mapper.Map(roatpResult, Resolve);
                return Ok(ukprnApar);
            }
            catch (Exception)
            {
                var message = $"Unable to fetch APAR entry from RoATP for UKPRN: {ukprn}";
                return StatusCode(500, message);
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("getLatestTime")]
        public async Task<IActionResult> GetLatestTime()
        {
            _logger.LogInformation("Fetching GET latest change date");

            DateTime? latestChange = _dateTimeProvider.GetCurrentDateTime();
            try
            {
                var roatpResult = await _roatpApiClient.GetLatestNonOnboardingOrganisationChangeDate();
                if (roatpResult.HasValue)
                {
                    latestChange = roatpResult;
                }
            }
            catch (Exception)
            {
                var message = "Unable to fetch latest APAR change date";
                return StatusCode(500, message);
            }

            return Ok(latestChange);
        }

        /// <summary>
        /// Gets active APAR entries
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(typeof(IEnumerable<AparEntry>), StatusCodes.Status200OK)]
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(AparExample))]
        [HttpGet("providers")]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Fetching APAR entries for all UKPRN's");

            try
            {
                var roatpData = await _roatpApiClient.GetRoatpSummary() ?? Enumerable.Empty<RoatpResult>();

                var roatpResults = roatpData
                    .Where(x => x.IsDateValid(_dateTimeProvider.GetCurrentDateTime()))
                    .ToList();

                var apprenticeshipProviders = _mapper.Map(roatpResults, Resolve).Where(p => p != null);
                return Ok(apprenticeshipProviders);
            }
            catch (Exception)
            {
                var message = "Unable to fetch APAR entries from RoATP";
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
