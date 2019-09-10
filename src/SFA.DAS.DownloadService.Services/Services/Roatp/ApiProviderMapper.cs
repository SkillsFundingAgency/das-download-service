using SFA.DAS.DownloadService.Api.Types.Roatp;
using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.DownloadService.Services.Services.Roatp
{
    using ApiProvider = SFA.DAS.DownloadService.Api.Types.Roatp.Provider;
    using ApiType = SFA.DAS.DownloadService.Api.Types.Roatp.ProviderType;
    public static class ApiProviderMapper
    {
        public static ApiProvider Map(ProviderDocument source)
        {
            return new ApiProvider
            {
                Ukprn = source.Ukprn,
                //ContractedForNonLeviedEmployers = source.ContractedForNonLeviedEmployers,
                NewOrganisationWithoutFinancialTrackRecord = source.NewOrganisationWithoutFinancialTrackRecord,
                ParentCompanyGuarantee = source.ParentCompanyGuarantee,
                ProviderType = (ApiType)source.ProviderType,
                StartDate = source.StartDate,
                ApplicationDeterminedDate = source.ApplicationDeterminedDate,
                Name = source.Name,
                CurrentlyNotStartingNewApprentices = source.CurrentlyNotStartingNewApprentices
            };
        }
    }
}
