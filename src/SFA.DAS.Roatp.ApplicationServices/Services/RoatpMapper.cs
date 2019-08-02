using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.DownloadService.Api.Types.Roatp;
using SFA.DAS.Roatp.ApplicationServices.Interfaces;
using SFA.DAS.Roatp.ApplicationServices.Models;

namespace SFA.DAS.Roatp.ApplicationServices.Services
{
    public class RoatpMapper : IRoatpMapper
    {
        public Provider Map(RoatpResult roatpResult)
        {

            // MFCMFC lossy?
            return new Provider
            {
                Ukprn = Convert.ToInt64(roatpResult.Ukprn),
                Name = roatpResult.OrganisationName,
                ProviderType = MapProviderType(roatpResult.ProviderType),
                NewOrganisationWithoutFinancialTrackRecord = !string.IsNullOrEmpty(roatpResult.NewOrganisationWithoutFinancialTrackRecord) & roatpResult.NewOrganisationWithoutFinancialTrackRecord.ToUpper() == "Y",
                ParentCompanyGuarantee = roatpResult.ParentCompanyGuarantee != null && roatpResult.ParentCompanyGuarantee.ToUpper() == "Y",

                //ContractedForNonLeviedEmployers = roatpResult.ContractedToDeliverToNonLeviedEmployers != null && roatpResult.ContractedToDeliverToNonLeviedEmployers.ToUpper() == "Y",
                StartDate = roatpResult.StartDate,
                ApplicationDeterminedDate = roatpResult.ApplicationDeterminedDate,
                //EndDate = roatpResult.EndDate,
                CurrentlyNotStartingNewApprentices = roatpResult.ProviderNotCurrentlyStartingNewApprentices != null,

            };
        }

        private static ProviderType MapProviderType(string providerType)
        {
            ProviderType returnedProviderType;
            switch (providerType)
            {
                case "Main provider":
                    returnedProviderType = ProviderType.MainProvider;
                    break;
                case "Employer provider":
                    returnedProviderType = ProviderType.EmployerProvider;
                    break;
                case "Supporting provider":
                    returnedProviderType = ProviderType.SupportingProvider;
                    break;
                default:
                    returnedProviderType = ProviderType.Unknown;
                    break;
            }
            return returnedProviderType;
        }

        private static string FormatDate(DateTime? date)
        {
            return date?.ToString("dd/MM/yyyy") ?? string.Empty;
        }

        public List<Provider> Map(List<RoatpResult> roatpResults)
        {
            return roatpResults.Select(Map).ToList();
        }

        public CsvProvider MapCsv(RoatpResult result)
        {
            var csvProvider = new CsvProvider
            {
                Ukprn = Convert.ToInt64(result.Ukprn),
                Name = result.OrganisationName,
                ProviderType = result.ProviderType,
                NewOrganisationWithoutFinancialTrackRecord = !string.IsNullOrEmpty(result.NewOrganisationWithoutFinancialTrackRecord) & result.NewOrganisationWithoutFinancialTrackRecord.ToUpper() == "Y",
                ParentCompanyGuarantee = result.ParentCompanyGuarantee != null && result.ParentCompanyGuarantee.ToUpper() == "Y",


                StartDate = FormatDate(result.StartDate),
                ProviderNotCurrentlyStartingNewApprentices = result.ProviderNotCurrentlyStartingNewApprentices != null ? "TRUE" : string.Empty,
                ApplicationDeterminedDate = FormatDate(result.ApplicationDeterminedDate)
            };

            return csvProvider;
        }

        public List<CsvProvider> MapCsv(List<RoatpResult> roatpResults)
        {
            return roatpResults.Select(MapCsv).ToList();
        }
    }
}
