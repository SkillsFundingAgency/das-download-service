using SFA.DAS.DownloadService.Api.Types.Roatp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;

namespace SFA.DAS.DownloadService.Api.Client.Interfaces
{
    public interface IRoatpApiClient
    {
        [Get("/api/v1/download/roatp-summary")]
        Task<IEnumerable<RoatpResult>> GetRoatpSummary();

        [Get("/api/v1/download/roatp-summary/{ukprn}")]
        Task<IEnumerable<RoatpResult>> GetRoatpSummaryByUkprn([AliasAs("ukprn")] int ukprn);

        [Get("/api/v1/download/roatp-summary/most-recent")]
        Task<DateTime?> GetLatestNonOnboardingOrganisationChangeDate();
    }
}