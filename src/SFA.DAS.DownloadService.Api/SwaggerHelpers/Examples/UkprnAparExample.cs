using System;
using SFA.DAS.DownloadService.Api.Types;
using Swashbuckle.AspNetCore.Filters;

namespace SFA.DAS.DownloadService.Api.SwaggerHelpers.Examples
{
    public class UkprnAparExample : IExamplesProvider<UkprnAparEntry>
    {
        public UkprnAparEntry GetExamples()
        {
            return new UkprnAparEntry
            {
                Ukprn = 87654321,
                Name = "Best Trainers Ltd",
                ApplicationType = AparEntryType.EmployerProvider,
                StartDate = new DateTime(DateTime.Now.Year - 1, 01, 04),
                ApplicationDeterminedDate = DateTime.Today,
                CurrentlyNotStartingNewApprentices = true,
                Epao = new Epao
                {
                    Name = "Assessments R Us Ltd",
                    StartDate = new DateTime(DateTime.Now.Year - 1, 03, 22),
                    ApplicationDeterminedDate = DateTime.Today.AddMonths(-5),
                }
            };
        }
    }
}
