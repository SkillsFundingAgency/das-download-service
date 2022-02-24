using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Polly;
using SFA.DAS.DownloadService.Api.Types.Roatp;
using SFA.DAS.DownloadService.Services.Interfaces;
using SFA.DAS.DownloadService.Services.Utility;
using SFA.DAS.DownloadService.Web.Models;
using SFA.DAS.Roatp.Api.Client.Interfaces;

namespace SFA.DAS.DownloadService.Web.Controllers
{
    public class RoatpController : Controller
    {
        private readonly IDownloadServiceApiClient _apiClient;
        private readonly IRoatpMapper _mapper;
        private readonly IRetryService _retryService;
        private readonly ILogger<RoatpController> _logger;

        public RoatpController(IDownloadServiceApiClient apiClient, IRoatpMapper mapper, IRetryService retryService, ILogger<RoatpController> logger)
        {
            _apiClient = apiClient;
            _mapper = mapper;
            _retryService = retryService;
            _logger = logger;
        }

        [ResponseCache(Duration = 600)]
        public async Task<ActionResult> Index()
        {
            
            DateTime? date =DateTime.Now;
            try
            {
                date = await _retryService.RetryPolicy("<roatpService>/api/v1/download/roatp-summary/most-recent")
                    .ExecuteAsync(context => _apiClient.GetLatestNonOnboardingOrganisationChangeDate(), new Context());
            }
            catch (Exception ex)
            {
                _logger.LogError("Unable to retrieve results for latest non-onboarding organisation change", ex);
                date = DateTime.Now;
            }

            var viewModel = new RoatpDownloadViewModel { Filename = GenerateFilename(date.Value), LastUpdated = date.Value };
            return View(viewModel);
        }


        [ResponseCache(Duration = 600)]
        public async Task<ActionResult> Csv()
        {
            var providers = new List<CsvProvider>();
            try
            {
                _logger.LogDebug("Getting results from GetRoatpSummary");

                IEnumerable<Provider> roatpResults;

                try
                {
                    var results = await _retryService.RetryPolicy("<roatpService>/api/v1/download/roatp-summary")
                        .ExecuteAsync(context => _apiClient.GetRoatpSummary(), new Context());
                    roatpResults = results?.ToList();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $@"Error trying to retrieve data from <roatpService>/api/v1/download/roatp-summary");
                    return RedirectToAction("ServiceUnavailable");
                }
                if (roatpResults == null || !roatpResults.Any())
                {
                    _logger.LogError($@"No results from GetRoatpSummary");
                    return RedirectToAction("ServiceUnavailable");
                }
               
                _logger.LogDebug($@"{roatpResults.Count()} results from GetRoatpSummary");
                var roatpResultsFiltered = roatpResults.Where(x => x.IsDateValid(DateTime.Now));
                _logger.LogDebug($@"{roatpResultsFiltered.Count()} results filtered from GetRoatpSummary");

 
                providers = _mapper.MapProvidersToCsvProviders(roatpResultsFiltered.ToList());
                _logger.LogDebug($@"{providers.Count()} providers mapped to CSV-ready state");

            }
            catch (Exception ex)
            {
                _logger.LogError($@"Unable to retrieve results for getting all roatp details, message: [{ex.Message}]", ex);
            }

            var date = await _apiClient.GetLatestNonOnboardingOrganisationChangeDate();
            if (date == null)
                date = DateTime.Now;


            using (var memoryStream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(memoryStream))
                {
                    using (var csvWriter = new CsvWriter(streamWriter))
                    {
                        csvWriter.WriteRecords(providers);
                        streamWriter.Flush();
                        memoryStream.Position = 0;
                        return File(memoryStream.ToArray(), "text/csv", GenerateFilename(date.Value));
                    }
                }
            }
        }

        private static string GenerateFilename(DateTime date)
        {
            return $"roatp-{date.ToSeoFormat()}.csv";
        }

        [Route("/service-unavailable")]
        public IActionResult ServiceUnavailable()
        {
            return View("ServiceUnavailable");
        }
    }
}
