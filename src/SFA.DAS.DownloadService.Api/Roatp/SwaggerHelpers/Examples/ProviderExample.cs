using System;
using SFA.DAS.DownloadService.Api.Types.Roatp;
using Swashbuckle.AspNetCore.Examples;

namespace SFA.DAS.DownloadService.Api.Roatp.SwaggerHelpers.Examples
{
    public class ProviderExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new Provider
            {
                Ukprn = 12345678,
                Name = "AtoA Trainers Ltd",
                ApplicationType = ProviderType.MainProvider,
                StartDate = new DateTime(DateTime.Now.Year - 1, 05, 17),
                ApplicationDeterminedDate = null
            };
        }
    }
}
