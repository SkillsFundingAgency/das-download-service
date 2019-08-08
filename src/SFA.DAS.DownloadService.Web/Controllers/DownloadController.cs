using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.DownloadService.Api.Types.Roatp;
using SFA.DAS.DownloadService.Services.Interfaces;
using SFA.DAS.DownloadService.Web.Models;
using SFA.DAS.Roatp.Api.Client;
using SFA.DAS.Roatp.Api.Client.Interfaces;

namespace SFA.DAS.DownloadService.Web.Controllers
{
    public class DownloadController : Controller
    {
        private readonly IRoatpApiClient _roatpApiClient;
        private readonly IRoatpMapper _mapper;

        public DownloadController(IRoatpApiClient roatpApiClient, IRoatpMapper mapper)
        {
            _roatpApiClient = roatpApiClient;
            _mapper = mapper;
        }

        [ResponseCache(Duration = 600)]
        public ActionResult Index()
        {

            var date = _roatpApiClient.GetLatestNonOnboardingOrganisationChangeDate().Result;

            var viewModel = new DownloadViewModel { Filename = GenerateFilename(date.Value), LastUpdated = date.Value };
            return View(viewModel);
        }


        [ResponseCache(Duration = 600)]
        public ActionResult Csv()
        {

            var roatpResults = _roatpApiClient.GetRoatpSummary().Result;
                            
            //.Where(x => x.IsDateValid(DateTime.Now));
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
            return $"roatp-{date.ToString("yyyy-MM-dd HH-mm-ss")}.csv";
        }
    }
}
