using Microsoft.Extensions.Logging;
using SFA.DAS.DownloadService.Api.Types.Roatp;
using SFA.DAS.Roatp.Api.Client.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Api.Client.Clients
{
    public class RoatpApiClient : ApiClientBase, IRoatpApiClient
    {
        public RoatpApiClient(HttpClient httpClient, IRoatpTokenService tokenService, ILogger<ApiClientBase> logger)
            : base(httpClient, tokenService, logger)
        {
        }

        public async Task<IEnumerable<RoatpResult>> GetRoatpSummary()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"api/v1/download/roatp-summary"))
            {
                return await Get<IEnumerable<RoatpResult>>(request, $"Could not retrieve roatp summary");
            }
        }

        public async Task<IEnumerable<RoatpResult>> GetRoatpSummaryByUkprn(int ukprn)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"api/v1/download/roatp-summary/{ukprn}"))
            {
                return await Get<IEnumerable<RoatpResult>>(request, $"Could not retrieve roatp summary for {ukprn}");
            }
        }

        public async Task<DateTime?> GetLatestNonOnboardingOrganisationChangeDate()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"api/v1/download/roatp-summary/most-recent"))
            {
                return await Get<DateTime?>(request, $"Could not retrieve roatp most recent change");
            }
        }
    }
}
