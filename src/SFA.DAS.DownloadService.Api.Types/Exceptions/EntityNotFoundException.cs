using System;

namespace SFA.DAS.DownloadService.Api.Types.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
