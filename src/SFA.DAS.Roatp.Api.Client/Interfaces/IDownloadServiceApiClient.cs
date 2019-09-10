using SFA.DAS.DownloadService.Api.Types.Roatp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Api.Client.Interfaces
{
    public interface IDownloadServiceApiClient
    {
        Task<IEnumerable<Provider>> GetRoatpSummary();
        Task<IEnumerable<Provider>> GetRoatpSummaryByUkprn(int ukprn);
        Task<DateTime?> GetLatestNonOnboardingOrganisationChangeDate();
    }
}
