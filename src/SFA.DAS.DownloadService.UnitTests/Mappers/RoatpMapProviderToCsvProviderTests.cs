using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SFA.DAS.DownloadService.Api.Types.Roatp;
using SFA.DAS.DownloadService.Services.Services.Roatp;

namespace SFA.DAS.DownloadService.UnitTests.Mappers
{
    [TestFixture]
    public class RoatpMapProviderToCsvProviderTests
    {
        private RoatpMapper _mapper;
        [SetUp]
        public void Init()
        {
            _mapper = new RoatpMapper();
        }

        [Test]
        public void ShouldMapProviderToCsvProvider()
        {
            var provider = new Provider();
            var mappedResult = _mapper.MapProviderToCsvProvider(provider);
            Assert.IsTrue(mappedResult.GetType().ToString().Contains("CsvProvider"));
        }

        [TestCase(12345678)]
        [TestCase(1)]
        public void ShouldMapProviderUkprnToCsvProviderUkprn(long ukprn)
        {
            var provider = new Provider();
            provider.Ukprn = ukprn;
            var mappedResult = _mapper.MapProviderToCsvProvider(provider);
            Assert.AreEqual(provider.Ukprn, mappedResult.Ukprn);
        }

        [TestCase("org name")]
        [TestCase("org name 2")]
        [TestCase("")]
        [TestCase(null)]
        public void ShouldMapProviderNameToCsvProviderName(string organisationName)
        {
            var provider = new Provider();
            provider.Ukprn = 12345678;
            provider.Name = organisationName;
            var mappedResult = _mapper.MapProviderToCsvProvider(provider);
            Assert.AreEqual(provider.Name, mappedResult.Name);
        }

        [TestCase("MainProvider", "Main provider")]
        [TestCase("EmployerProvider", "Employer provider")]
        [TestCase("SupportingProvider", "Supporting provider")]
        public void ShouldMapProviderTypeToCsvProviderProviderType(string providerTypeText, string expectedProviderType)
        {
            var providerType = ProviderType.MainProvider;
            if (providerTypeText == "EmployerProvider")
                providerType = ProviderType.EmployerProvider;
            if (providerTypeText == "SupportingProvider")
                providerType = ProviderType.SupportingProvider;
            var provider = new Provider
            {
                Ukprn = 12345678,
                Name = "org name",
                ApplicationType = providerType
            };
            var mappedResult = _mapper.MapProviderToCsvProvider(provider);
            Assert.AreEqual(expectedProviderType, mappedResult.ApplicationType);
        }


        [TestCase(null, "")]
        [TestCase("2019-08-05", "05/08/2019")]
        [TestCase("2021-08-05", "05/08/2021")]
        public void ShouldMapProviderStartDateToCsvProviderStartDate(DateTime? startDate, string expectedMapping)
        {
            var provider = new Provider
            {
                Ukprn = 12345678,
                Name = "org name",
                ApplicationType = ProviderType.MainProvider,
                StartDate = startDate
            };
            var mappedResult = _mapper.MapProviderToCsvProvider(provider);
            Assert.AreEqual(expectedMapping, mappedResult.StartDate);
        }


        [TestCase(null, "")]
        [TestCase("2019-08-05", "05/08/2019")]
        [TestCase("2021-08-05", "05/08/2021")]
        public void ShouldMapProviderApplicationDeterminedDateToCsvProviderApplicationDate(DateTime? applicationDeterminedDate, string expectedMapping)
        {
            var provider = new Provider
            {
                Ukprn = 12345678,
                Name = "org name",
                ApplicationType = ProviderType.MainProvider,
                StartDate = DateTime.Today,
                ApplicationDeterminedDate = applicationDeterminedDate
            };
            var mappedResult = _mapper.MapProviderToCsvProvider(provider);
            Assert.AreEqual(expectedMapping, mappedResult.ApplicationDeterminedDate);
        }


        [TestCase(false, "")]
        [TestCase(true, "Not Currently Starting New Apprentices")]
        public void ShouldMapProviderCurrentlyNotStartingToProviderCurrentlyNotStarting(bool providerNotCurrentlyStartingNewApprentices, string expectedMapping)
        {
            var provider = new Provider
            {
                Ukprn = 12345678,
                Name = "org name",
                ApplicationType = ProviderType.MainProvider,
                StartDate = DateTime.Today,
                ApplicationDeterminedDate = DateTime.Today
            };
            provider.CurrentlyNotStartingNewApprentices = providerNotCurrentlyStartingNewApprentices;
            var mappedResult = _mapper.MapProviderToCsvProvider(provider);
            Assert.AreEqual(expectedMapping, mappedResult.Status);
        }


        [Test]
        public void ShouldMapRoatpResultsToCsvProvidersAsAList()
        {
            var ukprn1 = 22345678;
            var ukprn2 = 87654321;

            var providers = new List<Provider>
            {
                new Provider
                {
                    Ukprn = ukprn1
                },
                new Provider
                {
                    Ukprn = ukprn2
                }
            };


            var mappedResults = _mapper.MapProvidersToCsvProviders(providers).ToList();
            Assert.AreEqual(mappedResults.Count, providers.Count);
            Assert.IsTrue(mappedResults.Select(x => x.Ukprn == ukprn1).Any());
            Assert.IsTrue(mappedResults.Select(x => x.Ukprn == ukprn2).Any());
        }
    }
}