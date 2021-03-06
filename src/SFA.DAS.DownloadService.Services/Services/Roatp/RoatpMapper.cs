﻿using System;
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
                ProviderType = MapProviderType(roatpResult?.ProviderType),
                NewOrganisationWithoutFinancialTrackRecord = !string.IsNullOrEmpty(roatpResult.NewOrganisationWithoutFinancialTrackRecord) && roatpResult.NewOrganisationWithoutFinancialTrackRecord.ToUpper() == "Y",
                ParentCompanyGuarantee = roatpResult.ParentCompanyGuarantee != null && roatpResult.ParentCompanyGuarantee.ToUpper() == "Y",
                StartDate = roatpResult?.StartDate,
                ApplicationDeterminedDate = roatpResult?.ApplicationDeterminedDate,
                CurrentlyNotStartingNewApprentices = roatpResult.ProviderNotCurrentlyStartingNewApprentices != null,

            };
        }

        private static ProviderType MapProviderType(string providerType)
        {
            ProviderType returnedProviderType;
            switch (providerType?.ToLower())
            {
                case "main provider":
                    returnedProviderType = ProviderType.MainProvider;
                    break;
                case "employer provider":
                    returnedProviderType = ProviderType.EmployerProvider;
                    break;
                case "supporting provider":
                    returnedProviderType = ProviderType.SupportingProvider;
                    break;
                default:
                    returnedProviderType = ProviderType.Unknown;
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
                ProviderType = result?.ProviderType,
                NewOrganisationWithoutFinancialTrackRecord = !string.IsNullOrEmpty(result.NewOrganisationWithoutFinancialTrackRecord) && result.NewOrganisationWithoutFinancialTrackRecord.ToUpper() == "Y",
                ParentCompanyGuarantee = result.ParentCompanyGuarantee != null && result.ParentCompanyGuarantee.ToUpper() == "Y",


                StartDate = FormatDate(result?.StartDate),
                ProviderNotCurrentlyStartingNewApprentices = result.ProviderNotCurrentlyStartingNewApprentices != null ? "TRUE" : string.Empty,
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
                ProviderType = Enumerations.GetEnumDescription(result.ProviderType),
                NewOrganisationWithoutFinancialTrackRecord = result.NewOrganisationWithoutFinancialTrackRecord,
                ParentCompanyGuarantee = result.ParentCompanyGuarantee,
                StartDate = FormatDate(result.StartDate),
                ProviderNotCurrentlyStartingNewApprentices = result.CurrentlyNotStartingNewApprentices ? "TRUE":string.Empty,
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
