using SFA.DAS.DownloadService.Api.Types;
using SFA.DAS.DownloadService.Api.Types.Assessor;
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
            if (!long.TryParse(roatpResult?.Ukprn, out long ukprn))
            {
                return null;
            }

            if (roatpResult?.EndDate != null && roatpResult.EndDate <= DateTime.Today)
                return null;

            return new AparEntry
            {
                Ukprn = ukprn,
                Name = roatpResult.OrganisationName,
                Uri = uriResolver(ukprn),
                ApplicationType = MapAparEntryType(roatpResult?.ApplicationType),
                StartDate = roatpResult?.StartDate,
                ApplicationDeterminedDate = roatpResult?.ApplicationDeterminedDate,
                CurrentlyNotStartingNewApprentices = roatpResult.ProviderNotCurrentlyStartingNewApprentices != null,
            };
        }

        public List<AparEntry> Map(List<RoatpResult> roatpResults, Func<long, string> uriResolver)
        {
            return roatpResults?.Select(roatpResult => Map(roatpResult, uriResolver)).ToList();
        }

        public AparEntry Map(EpaoResult epaoResult, Func<long, string> uriResolver)
        {
            if (epaoResult == null)
                return null;

            return new AparEntry
            {
                Ukprn = epaoResult.Ukprn,
                Name = epaoResult.Name,
                Uri = uriResolver(epaoResult.Ukprn),
                ApplicationType = AparEntryType.EPAO,
                StartDate = epaoResult.EarliestEffectiveFromDate,
                ApplicationDeterminedDate = epaoResult.EarliestDateStandardApprovedOnRegister,
                CurrentlyNotStartingNewApprentices = null
            };
        }

        public List<AparEntry> Map(List<EpaoResult> epaoResults, Func<long, string> uriResolver)
        {
            return epaoResults?.Select(epaoResult => Map(epaoResult, uriResolver)).ToList();
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

        public UkprnAparEntry Map(RoatpResult roatpResult, EpaoResult epaoResult, Func<long, string> uriResolver)
        {
            UkprnAparEntry ukprnAparEntry = null;
            if (roatpResult != null)
            {
                ukprnAparEntry = UkprnAparEntry.FromAparEntry(Map(roatpResult, uriResolver));
            }

            if (ukprnAparEntry == null && epaoResult != null)
            {
                ukprnAparEntry = UkprnAparEntry.FromAparEntry(Map(epaoResult, uriResolver));
            }

            if (ukprnAparEntry != null && epaoResult != null)
            {
                ukprnAparEntry.Epao = new Epao
                {
                    Name = epaoResult.Name,
                    StartDate = epaoResult.EarliestEffectiveFromDate,
                    ApplicationDeterminedDate = epaoResult.EarliestDateStandardApprovedOnRegister
                };
            }

            return ukprnAparEntry;
        }

        private static AparEntryType MapAparEntryType(string aparEntryType)
        {
            AparEntryType returnedAparEntryType;
            switch (aparEntryType?.ToLower())
            {
                case "main provider":
                    returnedAparEntryType = AparEntryType.MainProvider;
                    break;
                case "employer provider":
                    returnedAparEntryType = AparEntryType.EmployerProvider;
                    break;
                case "supporting provider":
                    returnedAparEntryType = AparEntryType.SupportingProvider;
                    break;
                default:
                    returnedAparEntryType = AparEntryType.Unknown;
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
