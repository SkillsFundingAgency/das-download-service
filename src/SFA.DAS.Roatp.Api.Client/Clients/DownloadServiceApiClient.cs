using Microsoft.Extensions.Logging;
using SFA.DAS.DownloadService.Api.Types;
using SFA.DAS.Roatp.Api.Client.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Api.Client.Clients
{
    public class DownloadServiceApiClient : ApiClientBase, IDownloadServiceApiClient
    {
        public DownloadServiceApiClient(HttpClient httpClient, ILogger<ApiClientBase> logger)
            : base(httpClient, null, logger)
        {
        }

        public async Task<IEnumerable<AparEntry>> GetAparSummary()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"api/apar"))
            {
                return await Get<IEnumerable<AparEntry>>(request, $"Could not retrieve apar summary data");
            }
        }

        public async Task<IEnumerable<AparEntry>> GetAparSummaryByUkprn(int ukprn)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"api/apar/{ukprn}"))
            {
                return await Get<IEnumerable<AparEntry>>(request, $"Could not retrieve apar summary data for {ukprn}");
            }
        }

        public async Task<DateTime?> GetLatestNonOnboardingOrganisationChangeDate()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"api/GetLatestTime"))
            {
                return await Get<DateTime?>(request, $"Could not retrieve roatp most recent change");
            }
        }
    }
}
