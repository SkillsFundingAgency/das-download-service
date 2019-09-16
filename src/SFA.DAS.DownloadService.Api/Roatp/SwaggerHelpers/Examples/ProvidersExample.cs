using System;
using System.Collections.Generic;
using SFA.DAS.DownloadService.Api.Types.Roatp;
using Swashbuckle.AspNetCore.Examples;

namespace SFA.DAS.DownloadService.Api.Roatp.SwaggerHelpers.Examples
{
    public class ProvidersExample : IExamplesProvider
    {
        public object GetExamples()
        {
            var providers = new List<Provider>
            {
                new Provider
                {
                    Ukprn = 12345678,
                    Name = "AtoA Trainers Ltd",
                    ProviderType = ProviderType.MainProvider,
                    ParentCompanyGuarantee = true,
                    NewOrganisationWithoutFinancialTrackRecord = false,
                    StartDate = new DateTime(DateTime.Now.Year - 1, 05, 17),
                    ApplicationDeterminedDate = null
                },
                new Provider
                {
                    Ukprn = 87654321,
                    Name = "AotA Trainers Ltd",
                    ProviderType = ProviderType.EmployerProvider,
                    ParentCompanyGuarantee = false,
                    NewOrganisationWithoutFinancialTrackRecord = true,
                    StartDate = new DateTime(DateTime.Now.Year - 1, 01, 04),
                    ApplicationDeterminedDate = DateTime.Today
                }
            };

            return providers;
        }
    }
}
