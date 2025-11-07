using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.DownloadService.Api.Client.Interfaces;
using SFA.DAS.DownloadService.Api.Infrastructure;
using SFA.DAS.DownloadService.Api.SwaggerHelpers.Examples;
using SFA.DAS.DownloadService.Api.Types;
using SFA.DAS.DownloadService.Api.Types.Roatp.Common;
using SFA.DAS.DownloadService.Api.Types.Roatp.Models;
using SFA.DAS.DownloadService.Services.Interfaces;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

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
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
        [ProducesResponseType(typeof(ProviderModel), StatusCodes.Status200OK)]
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

            OrganisationModel model = await _roatpApiClient.GetRoatpSummaryByUkprn(ukprn);

            if (model == null || model.Status == OrganisationStatus.Removed)
            {
                var message = "APAR entry from RoATP for UKPRN: {ukprn} is not found or they are in removed status";
                return NotFound(message);
            }

            ProviderModel providerModel = model;
            providerModel.Uri = Resolve(model.Ukprn);
            return Ok(providerModel);
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
        [ProducesResponseType(typeof(IEnumerable<ProviderModel>), StatusCodes.Status200OK)]
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(AparExample))]
        [HttpGet("providers")]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Fetching APAR entries for all UKPRN's");

            var response = await _roatpApiClient.GetRoatpSummary();

            List<ProviderModel> providersModel = response.Organisations
                .Where(org => org.Status != OrganisationStatus.Removed)
                .OrderByDescending(org => org.LastUpdatedDate)
                .Select(org =>
                {
                    ProviderModel provider = org;
                    provider.Uri = Resolve(org.Ukprn);
                    return provider;
                }).ToList();

            return Ok(providersModel);
        }

        private string Resolve(long ukprn)
        {
            var scheme = ControllerContext.HttpContext.Request.Scheme;
            var path = ControllerContext.HttpContext.Request.Host.Value;
            return $"{scheme}://{path}/api/providers/{ukprn}";
        }
    }
}
