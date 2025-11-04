using Refit;
using SFA.DAS.DownloadService.Api.Types.Roatp;
using SFA.DAS.DownloadService.Api.Types.Roatp.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.DownloadService.Api.Client.Interfaces
{
    public interface IRoatpApiClient
    {
        [Get("/api/v1/download/roatp-summary")]
        Task<IEnumerable<RoatpResult>> GetRoatpSummary();

        [Get("/organisations/{ukprn}")]
        Task<OrganisationModel> GetRoatpSummaryByUkprn([AliasAs("ukprn")] int ukprn);

        [Get("/api/v1/download/roatp-summary/most-recent")]
        Task<DateTime?> GetLatestNonOnboardingOrganisationChangeDate();
    }
}