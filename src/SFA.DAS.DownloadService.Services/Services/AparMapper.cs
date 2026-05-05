using SFA.DAS.DownloadService.Api.Types;
using SFA.DAS.DownloadService.Services.Interfaces;
using SFA.DAS.DownloadService.Services.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.DownloadService.Services.Services
{
    public class AparMapper : IAparMapper
    {
        public CsvAparEntry MapCsv(AparEntry aparEntry)
        {
            if (aparEntry == null)
                return null;

            var csvAparEntry = new CsvAparEntry
            {
                Ukprn = aparEntry.Ukprn,
                Name = aparEntry.Name,
                ApplicationType = Enumerations.GetEnumDescription(aparEntry.ApplicationType),
                StartDate = FormatDate(aparEntry.StartDate),
                Status = aparEntry.CurrentlyNotStartingNewApprentices.GetValueOrDefault(false) ? "Not Currently Starting New Apprentices" : string.Empty,
                ApplicationDeterminedDate = FormatDate(aparEntry.ApplicationDeterminedDate)
            };

            return csvAparEntry;
        }

        public List<CsvAparEntry> MapCsv(List<AparEntry> aparEntries)
        {
            return aparEntries?.Select(MapCsv).ToList();
        }

        private static string FormatDate(DateTime? date)
        {
            if (date == null)
                return null;

            return date.ToMapperDateString();
        }
    }
}
