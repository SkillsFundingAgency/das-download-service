using System;

namespace SFA.DAS.DownloadService.Api.Infrastructure
{
    public interface IDateTimeProvider
    {
        DateTime GetCurrentDateTime();
    }
}
