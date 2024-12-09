using System;
using System.Collections.Generic;
using SFA.DAS.DownloadService.Api.Types;
using Swashbuckle.AspNetCore.Examples;

namespace SFA.DAS.DownloadService.Api.SwaggerHelpers.Examples
{
    public class AparExample : IExamplesProvider
    {
        public object GetExamples()
        {
            var apar = new List<AparEntry>
            {
                new AparEntry
                {
                    Ukprn = 12345678,
                    Name = "Good Trainers Ltd",
                    ApplicationType = AparEntryType.MainProvider,
                    StartDate = new DateTime(DateTime.Now.Year - 1, 05, 17, 0, 0, 0, DateTimeKind.Utc),
                    ApplicationDeterminedDate = null,
                    CurrentlyNotStartingNewApprentices = false,
                },
                new AparEntry
                {
                    Ukprn = 87654321,
                    Name = "Best Trainers Ltd",
                    ApplicationType = AparEntryType.EmployerProvider,
                    StartDate = new DateTime(DateTime.Now.Year - 1, 01, 04, 0, 0, 0, DateTimeKind.Utc),
                    ApplicationDeterminedDate = DateTime.Today,
                    CurrentlyNotStartingNewApprentices = true
                }
            };

            return apar;
        }
    }
}
