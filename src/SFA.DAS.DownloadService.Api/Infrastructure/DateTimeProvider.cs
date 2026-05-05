using System;

namespace SFA.DAS.DownloadService.Api.Infrastructure
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime GetCurrentDateTime()
        {
            return DateTime.UtcNow;
        }
    }
}
