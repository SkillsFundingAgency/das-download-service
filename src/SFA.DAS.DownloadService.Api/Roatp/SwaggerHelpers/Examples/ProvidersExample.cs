﻿using System;
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
                    ApplicationType = ProviderType.MainProvider,
                    StartDate = new DateTime(DateTime.Now.Year - 1, 05, 17),
                    ApplicationDeterminedDate = null
                },
                new Provider
                {
                    Ukprn = 87654321,
                    Name = "AotA Trainers Ltd",
                    ApplicationType = ProviderType.EmployerProvider,
                    StartDate = new DateTime(DateTime.Now.Year - 1, 01, 04),
                    ApplicationDeterminedDate = DateTime.Today
                }
            };

            return providers;
        }
    }
}
