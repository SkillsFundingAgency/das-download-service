using Polly.Retry;

namespace SFA.DAS.DownloadService.Services.Interfaces
{
    public interface IRetryService
    {
        AsyncRetryPolicy RetryPolicy(string apiEndpointDescription);
    }
}