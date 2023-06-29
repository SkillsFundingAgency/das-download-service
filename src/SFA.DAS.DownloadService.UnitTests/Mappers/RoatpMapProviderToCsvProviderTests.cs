using NUnit.Framework;
using SFA.DAS.DownloadService.Api.Types;
using SFA.DAS.DownloadService.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.DownloadService.UnitTests.Mappers
{
    [TestFixture]
    public class RoatpMapProviderToCsvProviderTests
    {
        private AparMapper _mapper;
        [SetUp]
        public void Init()
        {
            _mapper = new AparMapper();
        }

        [Test]
        public void ShouldMapProviderToCsvProvider()
        {
            var provider = new AparEntry();
            var mappedResult = _mapper.MapCsv(provider);
            Assert.IsTrue(mappedResult.GetType().ToString().Contains("CsvProvider"));
        }

        [TestCase(12345678)]
        [TestCase(1)]
        public void ShouldMapProviderUkprnToCsvProviderUkprn(long ukprn)
        {
            var provider = new AparEntry();
            provider.Ukprn = ukprn;
            var mappedResult = _mapper.MapCsv(provider);
            Assert.AreEqual(provider.Ukprn, mappedResult.Ukprn);
        }

        [TestCase("org name")]
        [TestCase("org name 2")]
        [TestCase("")]
        [TestCase(null)]
        public void ShouldMapProviderNameToCsvProviderName(string organisationName)
        {
            var provider = new AparEntry();
            provider.Ukprn = 12345678;
            provider.Name = organisationName;
            var mappedResult = _mapper.MapCsv(provider);
            Assert.AreEqual(provider.Name, mappedResult.Name);
        }

        [TestCase("MainProvider", "Main provider")]
        [TestCase("EmployerProvider", "Employer provider")]
        [TestCase("SupportingProvider", "Supporting provider")]
        public void ShouldMapProviderTypeToCsvProviderProviderType(string providerTypeText, string expectedProviderType)
        {
            var providerType = AparEntryType.MainProvider;
            if (providerTypeText == "EmployerProvider")
                providerType = AparEntryType.EmployerProvider;
            if (providerTypeText == "SupportingProvider")
                providerType = AparEntryType.SupportingProvider;
            var provider = new AparEntry
            {
                Ukprn = 12345678,
                Name = "org name",
                ApplicationType = providerType
            };
            var mappedResult = _mapper.MapCsv(provider);
            Assert.AreEqual(expectedProviderType, mappedResult.ApplicationType);
        }


        [TestCase(null, "")]
        [TestCase("2019-08-05", "05/08/2019")]
        [TestCase("2021-08-05", "05/08/2021")]
        public void ShouldMapProviderStartDateToCsvProviderStartDate(DateTime? startDate, string expectedMapping)
        {
            var provider = new AparEntry
            {
                Ukprn = 12345678,
                Name = "org name",
                ApplicationType = AparEntryType.MainProvider,
                StartDate = startDate
            };
            var mappedResult = _mapper.MapCsv(provider);
            Assert.AreEqual(expectedMapping, mappedResult.StartDate);
        }


        [TestCase(null, "")]
        [TestCase("2019-08-05", "05/08/2019")]
        [TestCase("2021-08-05", "05/08/2021")]
        public void ShouldMapProviderApplicationDeterminedDateToCsvProviderApplicationDate(DateTime? applicationDeterminedDate, string expectedMapping)
        {
            var provider = new AparEntry
            {
                Ukprn = 12345678,
                Name = "org name",
                ApplicationType = AparEntryType.MainProvider,
                StartDate = DateTime.Today,
                ApplicationDeterminedDate = applicationDeterminedDate
            };
            var mappedResult = _mapper.MapCsv(provider);
            Assert.AreEqual(expectedMapping, mappedResult.ApplicationDeterminedDate);
        }


        [TestCase(false, "")]
        [TestCase(true, "Not Currently Starting New Apprentices")]
        public void ShouldMapProviderCurrentlyNotStartingToProviderCurrentlyNotStarting(bool providerNotCurrentlyStartingNewApprentices, string expectedMapping)
        {
            var provider = new AparEntry
            {
                Ukprn = 12345678,
                Name = "org name",
                ApplicationType = AparEntryType.MainProvider,
                StartDate = DateTime.Today,
                ApplicationDeterminedDate = DateTime.Today
            };
            provider.CurrentlyNotStartingNewApprentices = providerNotCurrentlyStartingNewApprentices;
            var mappedResult = _mapper.MapCsv(provider);
            Assert.AreEqual(expectedMapping, mappedResult.Status);
        }


        [Test]
        public void ShouldMapRoatpResultsToCsvProvidersAsAList()
        {
            var ukprn1 = 22345678;
            var ukprn2 = 87654321;

            var providers = new List<AparEntry>
            {
                new AparEntry
                {
                    Ukprn = ukprn1
                },
                new AparEntry
                {
                    Ukprn = ukprn2
                }
            };

            var mappedResults = _mapper.MapCsv(providers).ToList();
            Assert.AreEqual(mappedResults.Count, providers.Count);
            Assert.IsTrue(mappedResults.Select(x => x.Ukprn == ukprn1).Any());
            Assert.IsTrue(mappedResults.Select(x => x.Ukprn == ukprn2).Any());
        }
    }
}