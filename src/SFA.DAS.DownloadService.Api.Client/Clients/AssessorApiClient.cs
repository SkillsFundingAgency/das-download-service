using Microsoft.Extensions.Logging;
using SFA.DAS.DownloadService.Api.Client.Interfaces;
using SFA.DAS.DownloadService.Api.Types.Assessor;
using System;
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

        public async Task<List<EpaoResult>> GetAparSummary()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"api/ao/assessment-organisations/list"))
            {
                return await Get<List<EpaoResult>>(request, $"Could not retrieve APAR summary");
            }
        }

        public async Task<EpaoResult> GetAparSummaryByUkprn(int ukprn)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"api/ao/assessment-organisations/list/{ukprn}"))
            {
                return await Get<EpaoResult>(request, $"Could not retrieve APAR summary for ukprn {ukprn}");
            }
        }

        public async Task<DateTime?> GetAparSummaryLastUpdated()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"api/ao/assessment-organisations/apar-summary-last-updated"))
            {
                return await Get<DateTime?>(request, $"Could not retrieve last updated date from assessors api");
            }
        }
    }
}
