using SFA.DAS.DownloadService.Api.Types;
using Swashbuckle.AspNetCore.Examples;
using System;

namespace SFA.DAS.DownloadService.Api.SwaggerHelpers.Examples
{
    public class UkpnrAparExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new UkprnAparEntry
            {
                Ukprn = 87654321,
                Name = "Best Trainers Ltd",
                ApplicationType = AparEntryType.EmployerProvider,
                StartDate = new DateTime(DateTime.Now.Year - 1, 01, 04, 0, 0, 0, DateTimeKind.Utc),
                ApplicationDeterminedDate = DateTime.Today,
                CurrentlyNotStartingNewApprentices = true
            };
        }
    }
}
