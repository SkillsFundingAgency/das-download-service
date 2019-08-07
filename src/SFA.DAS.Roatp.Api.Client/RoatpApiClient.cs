
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using SFA.DAS.DownloadService.Settings;
using SFA.DAS.Roatp.Api.Client.Interfaces;
using SFA.DAS.Roatp.ApplicationServices.Interfaces;
using Newtonsoft.Json;
using System.Net;
using Microsoft.Extensions.Logging;
using SFA.DAS.DownloadService.Api.Types.Roatp;

namespace SFA.DAS.Roatp.Api.Client
{
    public class RoatpApiClient : IRoatpApiClient 
    {

        private readonly HttpClient _client;
        private readonly ILogger<RoatpApiClient> _logger;
        private readonly ITokenService _tokenService;
        private readonly string _baseUrl;
        public RoatpApiClient( ILogger<RoatpApiClient> logger, ITokenService tokenService, IWebConfiguration configuration)
        {
            _logger = logger;
            _tokenService = tokenService;
            _baseUrl = configuration.RoatpApiClientBaseUrl;
            _client = new HttpClient { BaseAddress = new Uri($"{_baseUrl}") };
        }

        public async Task<IEnumerable<RoatpResult>> GetRoatpSummary()
        {
            var url = $"{_baseUrl}/api/v1/download/roatp-summary";
            _logger.LogInformation($"Retrieving RoATP summary data from {url}");
            return await Get<IEnumerable<RoatpResult>>($"{url}");
        }

        public async Task<IEnumerable<RoatpResult>> GetRoatpSummaryByUkprn(int ukprn)
        {
            var url = $"{_baseUrl}/api/v1/download/roatp-summary/{ukprn}";
            _logger.LogInformation($"Retrieving RoATP summary data from {url}");
            return await Get<IEnumerable<RoatpResult>>($"{url}");
        }

        public async  Task<DateTime?> GetLatestNonOnboardingOrganisationChangeDate()
        {
            var url = $"{_baseUrl}/api/v1/download/roatp-summary/most-recent";
            _logger.LogInformation($"Retrieving RoATP most recent change from {url}");
            return await Get<DateTime>($"{url}");
        }
       
        private async Task<T> Get<T>(string uri)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());

            using (var response = await _client.GetAsync(new Uri(uri, UriKind.Absolute)))
            {
                var serializedObject = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(serializedObject);
            }
        }
    }
}
