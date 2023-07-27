using Microsoft.Extensions.Logging;
using SFA.DAS.DownloadService.Api.Client.Interfaces;
using SFA.DAS.DownloadService.Api.Types.Assessor;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.DownloadService.Api.Client.Clients
{
    public class AssessorApiClient : ApiClientBase, IAssessorApiClient
    {
        public AssessorApiClient(HttpClient httpClient, IAssessorTokenService tokenService, ILogger<ApiClientBase> logger)
            : base(httpClient, tokenService, logger)
        {
        }

        public async Task<List<EpaoResult>> GetAssessmentOrganisationsList()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"api/ao/assessment-organisations/list"))
            {
                return await Get<List<EpaoResult>>(request, $"Could not retrieve assessment organisations list");
            }
        }

        public async Task<EpaoResult> GetAssessmentOrganisationsListByUkprn(int ukprn)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"api/ao/assessment-organisations/list/{ukprn}"))
            {
                return await Get<EpaoResult>(request, $"Could not retrieve assessment organsisation for ukprn {ukprn}");
            }
        }
    }
}
