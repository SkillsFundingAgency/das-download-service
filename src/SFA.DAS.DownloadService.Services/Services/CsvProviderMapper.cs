using System;
using SFA.DAS.DownloadService.Api.Types.Roatp;
using SFA.DAS.DownloadService.Services.Utility;

namespace SFA.DAS.DownloadService.Services.Services
{
    public static class CsvProviderMapper
    {
        public static CsvProvider Map(ProviderDocument providerDocument)
        {
            var csvProvider = new CsvProvider
            {
                Ukprn = providerDocument.Ukprn,
                Name = providerDocument.Name,
                ProviderType = Enumerations.GetEnumDescription(providerDocument.ProviderType),
                NewOrganisationWithoutFinancialTrackRecord = providerDocument.NewOrganisationWithoutFinancialTrackRecord,
                ParentCompanyGuarantee = providerDocument.ParentCompanyGuarantee,
                StartDate = FormatDate(providerDocument.StartDate),
                ProviderNotCurrentlyStartingNewApprentices = providerDocument.CurrentlyNotStartingNewApprentices ? "TRUE" : string.Empty,
                ApplicationDeterminedDate = FormatDate(providerDocument.ApplicationDeterminedDate)
            };

            return csvProvider;
        }

        private static string FormatDate(DateTime? date)
        {
            return date?.ToString("dd/MM/yyyy") ?? string.Empty;
        }
    }
}
