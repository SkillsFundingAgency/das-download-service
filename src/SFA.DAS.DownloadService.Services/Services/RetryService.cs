using System;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using SFA.DAS.DownloadService.Services.Interfaces;

namespace SFA.DAS.DownloadService.Services.Services
{
    public class RetryService: IRetryService
    {
        private ILogger<RetryService> _logger;

        public RetryService(ILogger<RetryService> logger)
        {
            _logger = logger;
        }

        AsyncRetryPolicy IRetryService.RetryPolicy(string apiEndpointDescription)
        {
            return Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(4)
                }, (exception, timeSpan, retryCount, context) =>
                {
                    _logger.LogWarning(exception,$"Error retrieving response from API [{apiEndpointDescription}] Reason: {exception.Message}. Retrying in {timeSpan.Seconds} secs...attempt: {retryCount}");
                });
        }
    }
}
