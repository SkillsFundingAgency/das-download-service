using SFA.DAS.DownloadService.Api.Types;
using SFA.DAS.DownloadService.Api.Types.Roatp;
using SFA.DAS.DownloadService.Services.Interfaces;
using SFA.DAS.DownloadService.Services.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.DownloadService.Services.Services
{
    public class AparMapper : IAparMapper
    {
        public AparEntry Map(RoatpResult roatpResult, Func<long, string> uriResolver)
        {
            if (roatpResult == null)
                return null;

            if (roatpResult.EndDate != null && roatpResult.EndDate <= DateTime.Today)
                return null;

            return new AparEntry
            {
                Ukprn = roatpResult.Ukprn,
                Name = roatpResult.OrganisationName,
                Uri = uriResolver(roatpResult.Ukprn),
                ApplicationType = MapAparEntryType(roatpResult.ApplicationType),
                StartDate = roatpResult.StartDate,
                ApplicationDeterminedDate = roatpResult.ApplicationDeterminedDate,
                CurrentlyNotStartingNewApprentices = roatpResult.ProviderNotCurrentlyStartingNewApprentices != null,
            };
        }

        public List<AparEntry> Map(List<RoatpResult> roatpResults, Func<long, string> uriResolver)
        {
            return roatpResults?.Select(roatpResult => Map(roatpResult, uriResolver)).ToList();
        }

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

        private static ProviderType MapAparEntryType(string aparEntryType)
        {
            ProviderType returnedAparEntryType;
            switch (aparEntryType?.ToLower())
            {
                case "main provider":
                    returnedAparEntryType = ProviderType.MainProvider;
                    break;
                case "employer provider":
                    returnedAparEntryType = ProviderType.EmployerProvider;
                    break;
                case "supporting provider":
                    returnedAparEntryType = ProviderType.SupportingProvider;
                    break;
                default:
                    returnedAparEntryType = ProviderType.Unknown;
                    break;
            }
            return returnedAparEntryType;
        }

        private static string FormatDate(DateTime? date)
        {
            if (date == null)
                return null;

            return date.ToMapperDateString();
        }
    }
}
