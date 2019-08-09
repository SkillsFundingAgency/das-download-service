using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using SFA.DAS.DownloadService.Api.Types.Roatp;
using SFA.DAS.DownloadService.Services.Interfaces;
using SFA.DAS.DownloadService.Services.Utility;
using SFA.DAS.DownloadService.Web.Models;
using SFA.DAS.Roatp.Api.Client;
using SFA.DAS.Roatp.Api.Client.Interfaces;

namespace SFA.DAS.DownloadService.Web.Controllers
{
    public class DownloadController : Controller
    {
        private readonly IRoatpApiClient _roatpApiClient;
        private readonly IRoatpMapper _mapper;
        private readonly IRetryService _retryService;
        private readonly ILogger<DownloadController> _logger;

        public DownloadController(IRoatpApiClient roatpApiClient, IRoatpMapper mapper, IRetryService retryService, ILogger<DownloadController> logger)
        {
            _roatpApiClient = roatpApiClient;
            _mapper = mapper;
            _retryService = retryService;
            _logger = logger;
        }

        [ResponseCache(Duration = 600)]
        public ActionResult Index()
        {
            DateTime? date;
            try { 
            var result = _retryService.RetryPolicy("<roatpService>/api/v1/download/roatp-summary/most-recent").ExecuteAsync(context => _roatpApiClient.GetLatestNonOnboardingOrganisationChangeDate(), new Context());
                date = result.Result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Unable to retrieve results for latest non-onboarding organisation change", ex);
                date = DateTime.Now;
            }

            var viewModel = new DownloadViewModel { Filename = GenerateFilename(date.Value), LastUpdated = date.Value };
            return View(viewModel);
        }


        [ResponseCache(Duration = 600)]
        public ActionResult Csv()
        {

            var roatpResults = _roatpApiClient.GetRoatpSummary().Result.Where(x => x.IsDateValid(DateTime.Now));
            var providers = _mapper.MapCsv(roatpResults.ToList());
            var date = _roatpApiClient.GetLatestNonOnboardingOrganisationChangeDate().Result;
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
    }
}
