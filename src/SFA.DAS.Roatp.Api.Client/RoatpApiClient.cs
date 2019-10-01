
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using SFA.DAS.DownloadService.Settings;
using SFA.DAS.Roatp.Api.Client.Interfaces;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using SFA.DAS.DownloadService.Api.Types.Roatp;
using SFA.DAS.DownloadService.Services.Interfaces;

namespace SFA.DAS.Roatp.Api.Client
{
    public class RoatpApiClient : IRoatpApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<RoatpApiClient> _logger;
        private readonly ITokenService _tokenService;

        public RoatpApiClient(HttpClient client,ILogger<RoatpApiClient> logger, ITokenService tokenService)
        {
            _logger = logger;
            _tokenService = tokenService;
            _client = client;
        }

        public async Task<IEnumerable<RoatpResult>> GetRoatpSummary()
        {
            var url = $"api/v1/download/roatp-summary";
            _logger.LogInformation($"Retrieving RoATP summary data from {url}");
            return await Get<IEnumerable<RoatpResult>>($"{url}");
        }

        public async Task<IEnumerable<RoatpResult>> GetRoatpSummaryByUkprn(int ukprn)
        {
            var url = $"api/v1/download/roatp-summary/{ukprn}";
            _logger.LogInformation($"Retrieving RoATP summary data from {url}");
            return await Get<IEnumerable<RoatpResult>>($"{url}");
        }

        public async Task<DateTime?> GetLatestNonOnboardingOrganisationChangeDate()
        {
            var url = $"api/v1/download/roatp-summary/most-recent";
            _logger.LogInformation($"Retrieving RoATP most recent change from {url}");
            return await Get<DateTime>($"{url}");
        }

        private async Task<T> Get<T>(string uri)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());

            using (var response = await _client.GetAsync(new Uri(uri, UriKind.Relative)))
            {
                var serializedObject = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(serializedObject);
            }
        }
    }
}
