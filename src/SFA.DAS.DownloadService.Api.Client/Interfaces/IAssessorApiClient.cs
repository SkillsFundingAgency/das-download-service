using SFA.DAS.DownloadService.Api.Types.Assessor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.DownloadService.Api.Client.Interfaces
{
    public interface IAssessorApiClient
    {
        Task<IEnumerable<EpaoResult>> GetAparSummary();
        Task<EpaoResult> GetAparSummaryByUkprn(int ukprn);
        Task<DateTime?> GetAparSummaryLastUpdated();
    }
}
