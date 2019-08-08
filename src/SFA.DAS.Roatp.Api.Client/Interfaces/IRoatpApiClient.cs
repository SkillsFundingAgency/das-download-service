using System;
using System.Collections.Generic;
using SFA.DAS.DownloadService.Api.Types.Roatp;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Api.Client.Interfaces
{
    public interface IRoatpApiClient
    {
        Task<IEnumerable<RoatpResult>> GetRoatpSummary();
        Task<IEnumerable<RoatpResult>> GetRoatpSummaryByUkprn(int ukprn);
        Task<DateTime?> GetLatestNonOnboardingOrganisationChangeDate();
    }
}