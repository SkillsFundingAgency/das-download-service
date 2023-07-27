using SFA.DAS.DownloadService.Api.Types.Assessor;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.DownloadService.Api.Client.Interfaces
{
    public interface IAssessorApiClient
    {
        Task<List<EpaoResult>> GetAssessmentOrganisationsList();
        Task<EpaoResult> GetAssessmentOrganisationsListByUkprn(int ukprn);
    }
}
