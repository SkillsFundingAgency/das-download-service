using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.DownloadService.Api.Types;
using SFA.DAS.DownloadService.Services.Interfaces;
using SFA.DAS.DownloadService.Services.Utility;
using SFA.DAS.DownloadService.Web.Models;
using SFA.DAS.DownloadService.Api.Client.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.DownloadService.Web.Controllers
{
    public class AparController : Controller
    {
        private readonly IDownloadServiceApiClient _downloadServiceApiClient;
        private readonly IAparMapper _mapper;
        private readonly ILogger<AparController> _logger;

        public AparController(IDownloadServiceApiClient downloadServiceApiClient, IAparMapper mapper, ILogger<AparController> logger)
        {
            _downloadServiceApiClient = downloadServiceApiClient;
            _mapper = mapper;
            _logger = logger;
        }

        [ResponseCache(Duration = 600)]
        public async Task<IActionResult> Index()
        {
            
            DateTime? date;
            try
            {
                date = await _downloadServiceApiClient.GetLatestNonOnboardingOrganisationChangeDate();
            }
            catch (Exception ex)
            {
                _logger.LogError("Unable to retrieve results for latest non-onboarding organisation change", ex);
                date = DateTime.Now;
            }

            var viewModel = new AparDownloadViewModel { Filename = GenerateFilename(date.Value), LastUpdated = date.Value };
            return View(viewModel);
        }


        [ResponseCache(Duration = 600)]
        public async Task<ActionResult> DownloadCsv()
        {
            var aparCsv = new List<CsvAparEntry>();

            try
            {
                _logger.LogDebug("Getting results from GetAparSummary");

                var apar = await _downloadServiceApiClient.GetAparSummary();

                if (!apar?.Any() ?? false)
                {
                    _logger.LogError("No results from GetAparSummary");
                    return RedirectToAction("ServiceUnavailable");
                }

                _logger.LogDebug($"{apar.Count()} results from GetAparSummary");

                var aparFiltered = apar.Where(x => x.IsDateValid(DateTime.Now));

                _logger.LogDebug($"{aparFiltered.Count()} results filtered from GetAparSummary");

                aparCsv = _mapper.MapCsv(aparFiltered.ToList());

                _logger.LogDebug($"{aparCsv.Count()} apar entries mapped to CSV-ready state");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unable to retrieve results for getting all APAR details, message: [{ex.Message}]", ex);
                throw; 
            }

            var date = await _downloadServiceApiClient.GetLatestNonOnboardingOrganisationChangeDate() ?? DateTime.Now;

            using (var memoryStream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(memoryStream))
                {
                    using (var csvWriter = new CsvWriter(streamWriter))
                    {
                        csvWriter.Configuration.Delimiter = ",";
                        csvWriter.WriteRecords(aparCsv);

                        await streamWriter.FlushAsync();
                        memoryStream.Position = 0;
                        return File(memoryStream.ToArray(), "text/csv", GenerateFilename(date));
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
