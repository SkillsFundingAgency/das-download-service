﻿using System;
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
    public class DownloadServiceApiClient : IDownloadServiceApiClient
    {

        private readonly HttpClient _client;
        private readonly ILogger<DownloadServiceApiClient> _logger;
        private readonly ITokenService _tokenService;
        private readonly string _baseUrl;
        public DownloadServiceApiClient( ILogger<DownloadServiceApiClient> logger, ITokenService tokenService, IWebConfiguration configuration)
        {
            _logger = logger;
            _tokenService = tokenService;
            _baseUrl = configuration.DownloadServiceApiClientBaseUrl;
            _client = new HttpClient { BaseAddress = new Uri($"{_baseUrl}") };
        }

        public async Task<IEnumerable<Provider>> GetRoatpSummary()
        {
            var url = $"{_baseUrl}/api/providers";
            _logger.LogInformation($"Retrieving RoATP summary data from {url}");
            return await Get<IEnumerable<Provider>>($"{url}");
        }

        public async Task<IEnumerable<Provider>> GetRoatpSummaryByUkprn(int ukprn)
        {
            var url = $"{_baseUrl}/api/providers/{ukprn}";
            _logger.LogInformation($"Retrieving RoATP summary data from {url}");
            return await Get<IEnumerable<Provider>>($"{url}");
        }

        public async  Task<DateTime?> GetLatestNonOnboardingOrganisationChangeDate()
        {
            var url = $"{_baseUrl}/api/GetLatestTime";
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
