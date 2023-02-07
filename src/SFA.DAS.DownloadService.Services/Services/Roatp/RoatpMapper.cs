using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.DownloadService.Api.Types.Roatp;
using SFA.DAS.DownloadService.Services.Interfaces;
using SFA.DAS.DownloadService.Services.Utility;

namespace SFA.DAS.DownloadService.Services.Services.Roatp
{
    public class RoatpMapper : IRoatpMapper
    {
        public Provider Map(RoatpResult roatpResult)
        {

            if (!long.TryParse(roatpResult?.Ukprn, out long ukprn))
            {
                return null;
            }

            if (roatpResult.EndDate != null && roatpResult.EndDate <= DateTime.Today)
                return null;

            return new Provider
            {
                Ukprn = ukprn,
                Name = roatpResult.OrganisationName,
                ApplicationType = MapProviderType(roatpResult?.ApplicationType),
                StartDate = roatpResult?.StartDate,
                ApplicationDeterminedDate = roatpResult?.ApplicationDeterminedDate,
                CurrentlyNotStartingNewApprentices = roatpResult.ProviderNotCurrentlyStartingNewApprentices != null,
            };
        }

        private static ApplicationType MapProviderType(string providerType)
        {
            ApplicationType returnedProviderType;
            switch (providerType?.ToLower())
            {
                case "main provider":
                    returnedProviderType = ApplicationType.MainProvider;
                    break;
                case "employer provider":
                    returnedProviderType = ApplicationType.EmployerProvider;
                    break;
                case "supporting provider":
                    returnedProviderType = ApplicationType.SupportingProvider;
                    break;
                default:
                    returnedProviderType = ApplicationType.Unknown;
                    break;
            }
            return returnedProviderType;
        }

        public List<Provider> Map(List<RoatpResult> roatpResults)
        {
            return roatpResults.Select(Map).ToList();
        }

        public CsvProvider MapCsv(RoatpResult result)
        {
            if (!long.TryParse(result?.Ukprn, out long ukprn))
            {
                return null;
            }

            var csvProvider = new CsvProvider
            {
                Ukprn = ukprn,
                Name = result.OrganisationName,
                ApplicationType = result?.ApplicationType,
                StartDate = FormatDate(result?.StartDate),
                Status = result.ProviderNotCurrentlyStartingNewApprentices != null ? "Not Currently Starting New Apprentices" : string.Empty,
                ApplicationDeterminedDate = FormatDate(result?.ApplicationDeterminedDate)
            };

            return csvProvider;
        }

        public CsvProvider MapProviderToCsvProvider(Provider result)
        {
            var csvProvider = new CsvProvider
            {
                Ukprn = result.Ukprn,
                Name = result.Name,
                ApplicationType = Enumerations.GetEnumDescription(result.ApplicationType),
                StartDate = FormatDate(result.StartDate),
                Status = result.CurrentlyNotStartingNewApprentices ? "Not Currently Starting New Apprentices" : string.Empty,
                ApplicationDeterminedDate = FormatDate(result.ApplicationDeterminedDate)
            };

            return csvProvider;
        }

        public List<CsvProvider> MapProvidersToCsvProviders(List<Provider> providers)
        {
            var csvProviders = new List<CsvProvider>();
            foreach (var provider in providers)
            {
                csvProviders.Add(MapProviderToCsvProvider(provider));

            }

            return csvProviders;
        }

        public List<CsvProvider> MapCsv(List<RoatpResult> roatpResults)
        {
            return roatpResults.Select(MapCsv).ToList();
        }

        private static string FormatDate(DateTime? date)
        {
            return date.ToMapperDateString();
        }
    }
}
