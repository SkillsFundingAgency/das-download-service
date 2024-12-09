using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using SFA.DAS.DownloadService.Api.Client.Interfaces;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SFA.DAS.DownloadService.Api.Client.Clients
{
    public class ApiClientBase
    {
        private readonly ILogger<ApiClientBase> _logger;
        private readonly ITokenService _tokenService;
        private readonly HttpClient _httpClient;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

        protected ITokenService TokenService => _tokenService;
        protected HttpClient HttpClient => _httpClient;

        protected ApiClientBase(HttpClient httpClient, ITokenService tokenService, ILogger<ApiClientBase> logger)
        {
            _tokenService = tokenService;
            _logger = logger;
            _httpClient = httpClient;

            _retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,
                    retryAttempt)));
        }

        protected async Task<T> Get<T>(HttpRequestMessage request, string message)
        {
            HttpRequestMessage clonedRequest = null;

            var result = await _retryPolicy.ExecuteAsync(async () =>
            {
                clonedRequest = new HttpRequestMessage(request.Method, request.RequestUri);
                clonedRequest.Headers.Add("Accept", "application/json");
                if (_tokenService != null)
                {
                    clonedRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await _tokenService.GetTokenAsync());
                }

                return await _httpClient.SendAsync(clonedRequest);
            });

            if (result.StatusCode == HttpStatusCode.OK)
            {
                var serializedObject = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(serializedObject);
            }
            else
            {
                _logger.LogError(message);
            }

            return default;
        }
    }
}
