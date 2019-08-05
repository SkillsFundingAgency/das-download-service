using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.DownloadService.Api.Types.Roatp;
using SFA.DAS.Roatp.ApplicationServices.Interfaces;
namespace SFA.DAS.Roatp.ApplicationServices.Services
{
    public class RoatpMapper : IRoatpMapper
    {
        public Provider Map(RoatpResult roatpResult)
        {

            if (!long.TryParse(roatpResult?.Ukprn, out long ukprn))
            {
                return null;
            }

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

        public List<CsvProvider> MapCsv(List<RoatpResult> roatpResults)
        {
            return roatpResults.Select(MapCsv).ToList();
        }

        private static string FormatDate(DateTime? date)
        {
            return date?.ToString("dd/MM/yyyy") ?? string.Empty;
        }
    }
}
