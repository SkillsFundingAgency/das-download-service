using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using SFA.DAS.DownloadService.Api.Types;
using SFA.DAS.DownloadService.Api.Types.Roatp;
using SFA.DAS.DownloadService.Services.Services;

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
                Ukprn = "12345678",
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
            ClassicAssert.Multiple(() =>
            {
                ClassicAssert.NotNull(result);
                ClassicAssert.AreEqual(12345678, result.Ukprn);
                ClassicAssert.AreEqual("Org1", result.Name);
                ClassicAssert.AreEqual("http://example.com/12345678", result.Uri);
                ClassicAssert.AreEqual(AparEntryType.MainProvider, result.ApplicationType);
                ClassicAssert.AreEqual(startDate.AddMonths(-1), result.StartDate);
                ClassicAssert.AreEqual(applicationDeterminedDate.AddMonths(-2), result.ApplicationDeterminedDate);
                ClassicAssert.AreEqual(expectedCurrentlyNotStartingNewApprentices, result.CurrentlyNotStartingNewApprentices);
            });
        }

        [Test]
        public void Map_RoatpResultToAparEntry_InvalidUkprn_ReturnsNull()
        {
            // Arrange
            var roatpResult = new RoatpResult { Ukprn = "invalid", OrganisationName = "Org1", ApplicationType = "main provider", StartDate = DateTime.Now };
            Func<long, string> uriResolver = ukprn => $"http://example.com/{ukprn}";

            // Act
            var result = _mapper.Map(roatpResult, uriResolver);

            // Assert
            ClassicAssert.IsNull(result);
        }

        [Test]
        public void Map_RoatpResultToAparEntry_EndDateBeforeToday_ReturnsNull()
        {
            // Arrange
            var roatpResult = new RoatpResult { Ukprn = "12345678", OrganisationName = "Org1", ApplicationType = "main provider", EndDate = DateTime.Today.AddDays(-1), StartDate = DateTime.Now };
            Func<long, string> uriResolver = ukprn => $"http://example.com/{ukprn}";

            // Act
            var result = _mapper.Map(roatpResult, uriResolver);

            // Assert
            ClassicAssert.IsNull(result);
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
                    Ukprn = "12345678",
                    OrganisationName = "Org1",
                    ApplicationType = "main provider",
                    StartDate = startDate.AddMonths(-1),
                    ApplicationDeterminedDate = applicationDeterminedDate.AddMonths(-2),
                    ProviderNotCurrentlyStartingNewApprentices = null,
                },
                new RoatpResult
                {
                    Ukprn = "23456789",
                    OrganisationName = "Org2",
                    ApplicationType = "employer provider",
                    StartDate = startDate.AddMonths(-2),
                    ApplicationDeterminedDate = applicationDeterminedDate.AddMonths(-3),
                    ProviderNotCurrentlyStartingNewApprentices = DateTime.Now.AddMonths(10),
                },
                new RoatpResult
                {
                    Ukprn = "34567890",
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
            ClassicAssert.Multiple(() =>
            {
                ClassicAssert.NotNull(results);
                ClassicAssert.AreEqual(12345678, results[0].Ukprn);
                ClassicAssert.AreEqual("Org1", results[0].Name);
                ClassicAssert.AreEqual("http://example.com/12345678", results[0].Uri);
                ClassicAssert.AreEqual(AparEntryType.MainProvider, results[0].ApplicationType);
                ClassicAssert.AreEqual(startDate.AddMonths(-1), results[0].StartDate);
                ClassicAssert.AreEqual(applicationDeterminedDate.AddMonths(-2), results[0].ApplicationDeterminedDate);
                ClassicAssert.IsFalse(results[0].CurrentlyNotStartingNewApprentices);

                ClassicAssert.AreEqual(23456789, results[1].Ukprn);
                ClassicAssert.AreEqual("Org2", results[1].Name);
                ClassicAssert.AreEqual("http://example.com/23456789", results[1].Uri);
                ClassicAssert.AreEqual(AparEntryType.EmployerProvider, results[1].ApplicationType);
                ClassicAssert.AreEqual(startDate.AddMonths(-2), results[1].StartDate);
                ClassicAssert.AreEqual(applicationDeterminedDate.AddMonths(-3), results[1].ApplicationDeterminedDate);
                ClassicAssert.IsTrue(results[1].CurrentlyNotStartingNewApprentices);

                ClassicAssert.AreEqual(34567890, results[2].Ukprn);
                ClassicAssert.AreEqual("Org3", results[2].Name);
                ClassicAssert.AreEqual("http://example.com/34567890", results[2].Uri);
                ClassicAssert.AreEqual(AparEntryType.SupportingProvider, results[2].ApplicationType);
                ClassicAssert.AreEqual(startDate.AddMonths(-3), results[2].StartDate);
                ClassicAssert.AreEqual(applicationDeterminedDate.AddMonths(-4), results[2].ApplicationDeterminedDate);
                ClassicAssert.IsFalse(results[2].CurrentlyNotStartingNewApprentices);
            });
        }
    }
}