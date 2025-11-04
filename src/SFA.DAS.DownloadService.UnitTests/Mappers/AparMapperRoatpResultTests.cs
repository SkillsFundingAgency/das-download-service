using NUnit.Framework;
using SFA.DAS.DownloadService.Api.Types;
using SFA.DAS.DownloadService.Api.Types.Roatp;
using SFA.DAS.DownloadService.Services.Services;
using System;
using System.Collections.Generic;

namespace SFA.DAS.DownloadService.UnitTests.Mappers
{
    [TestFixture]
    public class AparMapperRoatpResultTests
    {
        private AparMapper _mapper;

        [SetUp]
        public void SetUp()
        {
            _mapper = new AparMapper();
        }

        [TestCase(null, false)]
        [TestCase("01/01/2000", true)]
        public void Map_RoatpResultToAparEntry_ValidInput_MapsCorrectly(string providerNotCurrentlyStartingNewApprentices, bool expectedCurrentlyNotStartingNewApprentices)
        {
            // Arrange
            var startDate = DateTime.Now;
            var applicationDeterminedDate = DateTime.Now.AddDays(-1);

            var roatpResult = new RoatpResult
            {
                Ukprn = 12345678,
                OrganisationName = "Org1",
                ApplicationType = "main provider",
                StartDate = startDate.AddMonths(-1),
                ApplicationDeterminedDate = applicationDeterminedDate.AddMonths(-2),
                ProviderNotCurrentlyStartingNewApprentices = providerNotCurrentlyStartingNewApprentices != null ? DateTime.Parse(providerNotCurrentlyStartingNewApprentices) : (DateTime?)null,
            };
            Func<long, string> uriResolver = ukprn => $"http://example.com/{ukprn}";

            // Act
            var result = _mapper.Map(roatpResult, uriResolver);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.NotNull(result);
                Assert.AreEqual(12345678, result.Ukprn);
                Assert.AreEqual("Org1", result.Name);
                Assert.AreEqual("http://example.com/12345678", result.Uri);
                Assert.AreEqual(ProviderType.MainProvider, result.ApplicationType);
                Assert.AreEqual(startDate.AddMonths(-1), result.StartDate);
                Assert.AreEqual(applicationDeterminedDate.AddMonths(-2), result.ApplicationDeterminedDate);
                Assert.AreEqual(expectedCurrentlyNotStartingNewApprentices, result.CurrentlyNotStartingNewApprentices);
            });
        }

        [Test]
        public void Map_RoatpResultToAparEntry_EndDateBeforeToday_ReturnsNull()
        {
            // Arrange
            var roatpResult = new RoatpResult { Ukprn = 12345678, OrganisationName = "Org1", ApplicationType = "main provider", EndDate = DateTime.Today.AddDays(-1), StartDate = DateTime.Now };
            Func<long, string> uriResolver = ukprn => $"http://example.com/{ukprn}";

            // Act
            var result = _mapper.Map(roatpResult, uriResolver);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void Map_ListOfRoatpResultToAparEntry_ValidInput_MapsCorrectly()
        {
            // Arrange
            var startDate = DateTime.Now;
            var applicationDeterminedDate = DateTime.Now.AddDays(-1);

            var roatpResults = new List<RoatpResult>
            {
                new RoatpResult
                {
                    Ukprn = 12345678,
                    OrganisationName = "Org1",
                    ApplicationType = "main provider",
                    StartDate = startDate.AddMonths(-1),
                    ApplicationDeterminedDate = applicationDeterminedDate.AddMonths(-2),
                    ProviderNotCurrentlyStartingNewApprentices = null,
                },
                new RoatpResult
                {
                    Ukprn = 23456789,
                    OrganisationName = "Org2",
                    ApplicationType = "employer provider",
                    StartDate = startDate.AddMonths(-2),
                    ApplicationDeterminedDate = applicationDeterminedDate.AddMonths(-3),
                    ProviderNotCurrentlyStartingNewApprentices = DateTime.Now.AddMonths(10),
                },
                new RoatpResult
                {
                    Ukprn = 34567890,
                    OrganisationName = "Org3",
                    ApplicationType = "supporting provider",
                    StartDate = startDate.AddMonths(-3),
                    ApplicationDeterminedDate = applicationDeterminedDate.AddMonths(-4),
                    ProviderNotCurrentlyStartingNewApprentices = null,
                }
            };
            
            Func<long, string> uriResolver = ukprn => $"http://example.com/{ukprn}";

            // Act
            var results = _mapper.Map(roatpResults, uriResolver);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.NotNull(results);
                Assert.AreEqual(12345678, results[0].Ukprn);
                Assert.AreEqual("Org1", results[0].Name);
                Assert.AreEqual("http://example.com/12345678", results[0].Uri);
                Assert.AreEqual(ProviderType.MainProvider, results[0].ApplicationType);
                Assert.AreEqual(startDate.AddMonths(-1), results[0].StartDate);
                Assert.AreEqual(applicationDeterminedDate.AddMonths(-2), results[0].ApplicationDeterminedDate);
                Assert.IsFalse(results[0].CurrentlyNotStartingNewApprentices);

                Assert.AreEqual(23456789, results[1].Ukprn);
                Assert.AreEqual("Org2", results[1].Name);
                Assert.AreEqual("http://example.com/23456789", results[1].Uri);
                Assert.AreEqual(ProviderType.EmployerProvider, results[1].ApplicationType);
                Assert.AreEqual(startDate.AddMonths(-2), results[1].StartDate);
                Assert.AreEqual(applicationDeterminedDate.AddMonths(-3), results[1].ApplicationDeterminedDate);
                Assert.IsTrue(results[1].CurrentlyNotStartingNewApprentices);

                Assert.AreEqual(34567890, results[2].Ukprn);
                Assert.AreEqual("Org3", results[2].Name);
                Assert.AreEqual("http://example.com/34567890", results[2].Uri);
                Assert.AreEqual(ProviderType.SupportingProvider, results[2].ApplicationType);
                Assert.AreEqual(startDate.AddMonths(-3), results[2].StartDate);
                Assert.AreEqual(applicationDeterminedDate.AddMonths(-4), results[2].ApplicationDeterminedDate);
                Assert.IsFalse(results[2].CurrentlyNotStartingNewApprentices);
            });
        }
    }
}