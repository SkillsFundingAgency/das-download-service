using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SFA.DAS.DownloadService.Api.Types.Roatp;
using SFA.DAS.DownloadService.Services.Services.Roatp;

namespace SFA.DAS.DownloadService.UnitTests.Mappers
{
    [TestFixture]
    public class RoatpMapperMapCsvTests
    {
        private RoatpMapper _mapper;
        [SetUp]
        public void Init()
        {
            _mapper = new RoatpMapper();
        }

        [Test]
        public void ShouldMapRoatpResultWithoutUkprnToNull()
        {
            var roatpResult = new RoatpResult();
            var mappedResult = _mapper.MapCsv(roatpResult);
            Assert.IsNull(mappedResult);
        }

        [TestCase(12345678)]
        [TestCase(1)]
        public void ShouldMapRoatpResultUkprnToCSVProviderUkprn(long ukprn)
        {
            var roatpResult = new RoatpResult();
            roatpResult.Ukprn = ukprn.ToString();
            var mappedResult = _mapper.MapCsv(roatpResult);
            Assert.AreEqual(roatpResult.Ukprn, mappedResult.Ukprn.ToString());
        }

        [TestCase("org name")]
        [TestCase("org name 2")]
        [TestCase("")]
        [TestCase(null)]
        public void ShouldMapRoatpResultOrganisationNameToCSVProviderName(string organisationName)
        {
            var roatpResult = new RoatpResult();
            roatpResult.Ukprn = "12345678";
            roatpResult.OrganisationName = organisationName;
            var mappedResult = _mapper.MapCsv(roatpResult);
            Assert.AreEqual(roatpResult.OrganisationName, mappedResult.Name);
        }

        [TestCase(null, null)]
        [TestCase("main provider","main provider")]
        [TestCase("MAIN provider", "MAIN provider")]
        public void ShouldMapRoatpResultProviderTypeToCSVProviderProviderType(string providerType, string expectedProviderType)
        {
            var roatpResult = new RoatpResult();
            roatpResult.Ukprn = "12345678";
            roatpResult.OrganisationName = "org name";
            roatpResult.ApplicationType = providerType;
            var mappedResult = _mapper.MapCsv(roatpResult);
            Assert.AreEqual(expectedProviderType, mappedResult.ApplicationType);
        }

        [TestCase(null, "")]
        [TestCase("2019-08-05", "05/08/2019")]
        [TestCase("2021-08-05", "05/08/2021")]
        public void ShouldMapRoatpResultStartDateToCSVProviderStartDate(DateTime? startDate, string expectedMapping)
        {
            var roatpResult = new RoatpResult();
            roatpResult.Ukprn = "12345678";
            roatpResult.OrganisationName = "org name";
            roatpResult.ApplicationType = "main provider";
            roatpResult.StartDate = startDate;
            var mappedResult = _mapper.MapCsv(roatpResult);
            Assert.AreEqual(expectedMapping, mappedResult.StartDate);
        }


        [TestCase(null, "")]
        [TestCase("2019-08-05", "05/08/2019")]
        [TestCase("2021-08-05", "05/08/2021")]
        public void ShouldMapRoatpResultApplicationDeterminedDateToCsvProviderApplicationDate(DateTime? applicationDeterminedDate, string expectedMapping)
        {
            var roatpResult = new RoatpResult();
            roatpResult.Ukprn = "12345678";
            roatpResult.OrganisationName = "org name";
            roatpResult.ApplicationType = "main provider";
            roatpResult.StartDate = DateTime.Today;
            roatpResult.ApplicationDeterminedDate = applicationDeterminedDate;
            var mappedResult = _mapper.MapCsv(roatpResult);
            Assert.AreEqual(expectedMapping, mappedResult.ApplicationDeterminedDate);
        }


        [TestCase(null, "")]
        [TestCase("2019-08-04", "Not Currently Starting New Apprentices")]
        public void ShouldMapRoatpResultCurrentlyNotStartingToProviderCurrentlyNotStarting(DateTime? providerNotCurrentlyStartingNewApprentices, string expectedMapping)
        {
            var roatpResult = new RoatpResult();
            roatpResult.Ukprn = "12345678";
            roatpResult.OrganisationName = "org name";
            roatpResult.ApplicationType = "main provider";
            roatpResult.StartDate = DateTime.Today;
            roatpResult.ApplicationDeterminedDate = DateTime.Today;
            roatpResult.ProviderNotCurrentlyStartingNewApprentices = providerNotCurrentlyStartingNewApprentices;
            var mappedResult = _mapper.MapCsv(roatpResult);
            Assert.AreEqual(expectedMapping, mappedResult.Status);
        }


        [Test]
        public void ShouldMapRoatpResultsToCsvProvidersAsAList()
        {
            var ukprn1 = "22345678";
            var ukprn2 = "87654321";

            var roatpResultstoMap = new List<RoatpResult>
            {
                new RoatpResult
                {
                    Ukprn = ukprn1
                },
                new RoatpResult
                {
                    Ukprn = ukprn2
                }
            };

            var mappedResults = _mapper.MapCsv(roatpResultstoMap);
            Assert.AreEqual(mappedResults.Count, roatpResultstoMap.Count);
            Assert.IsTrue(mappedResults.Select(x => x.Ukprn.ToString() == ukprn1).Any());
            Assert.IsTrue(mappedResults.Select(x => x.Ukprn.ToString() == ukprn2).Any());
        }
    }
}
