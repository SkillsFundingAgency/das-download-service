using SFA.DAS.DownloadService.Api.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Api.Client.Interfaces
{
    public interface IDownloadServiceApiClient
    {
        Task<IEnumerable<AparEntry>> GetAparSummary();
        Task<IEnumerable<AparEntry>> GetAparSummaryByUkprn(int ukprn);
        Task<DateTime?> GetLatestNonOnboardingOrganisationChangeDate();
    }
}
