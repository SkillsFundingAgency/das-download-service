﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.DownloadService.Api.Client.Interfaces;
using SFA.DAS.DownloadService.Api.Types;
using SFA.DAS.DownloadService.Services.Interfaces;
using SFA.DAS.DownloadService.Services.Utility;
using SFA.DAS.DownloadService.Web.Models;

namespace SFA.DAS.DownloadService.Web.Controllers
{
    public class AparController : Controller
    {
        private readonly IDownloadServiceApiClient _downloadServiceApiClient;
        private readonly IAparMapper _mapper;
        private readonly ILogger<AparController> _logger;

        private const string RouteAparDownloadCsv = nameof(RouteAparDownloadCsv);
        private const string RouteAparGetIndex = nameof(RouteAparGetIndex);

        public AparController(IDownloadServiceApiClient downloadServiceApiClient, IAparMapper mapper, ILogger<AparController> logger)
        {
            _downloadServiceApiClient = downloadServiceApiClient;
            _mapper = mapper;
            _logger = logger;
        }

        [Route("apar", Name = RouteAparGetIndex)]
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
                _logger.LogError(ex, "Unable to retrieve results for latest non-onboarding organisation change");
                date = DateTime.Now;
            }

            var viewModel = new AparDownloadViewModel { Filename = GenerateFilename(date.Value), LastUpdated = date.Value };
            return View(viewModel);
        }


        [Route("apar/downloadcsv", Name = RouteAparDownloadCsv)]
        [ResponseCache(Duration = 600)]
        public async Task<IActionResult> DownloadCsv()
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

                _logger.LogDebug($"{aparCsv.Count} apar entries mapped to CSV-ready state");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unable to retrieve results for getting all APAR details, message: [{ex.Message}]", ex);
                throw;
            }

            var date = await _downloadServiceApiClient.GetLatestNonOnboardingOrganisationChangeDate() ?? DateTime.Now;

            using var memoryStream = new MemoryStream();
            using var streamWriter = new StreamWriter(memoryStream);
            var config = new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = "," };
            using var csvWriter = new CsvWriter(streamWriter, config);
            csvWriter.WriteRecords(aparCsv);

            await streamWriter.FlushAsync();
            memoryStream.Position = 0;
            return File(memoryStream.ToArray(), "text/csv", GenerateFilename(date));
        }

        [Route("roatp")]
        public IActionResult IndexRoatp()
        {
            _logger.LogWarning("Deprecated endpoint 'roatp' called for AparController");
            return RedirectToRoute(RouteAparGetIndex);
        }

        [Route("roatp/downloadcsv")]
        public IActionResult DownloadCsvRoatp()
        {
            _logger.LogWarning("Deprecated endpoint 'roatp/downloadcsv' called for AparController");
            return RedirectToRoute(RouteAparDownloadCsv);
        }

        private static string GenerateFilename(DateTime date)
        {
            return $"apar-{date.ToSeoFormat()}.csv";
        }

        [Route("/service-unavailable")]
        public IActionResult ServiceUnavailable()
        {
            return View("ServiceUnavailable");
        }
    }
}
