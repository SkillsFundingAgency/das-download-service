using NUnit.Framework;
using SFA.DAS.DownloadService.Api.Types;
using SFA.DAS.DownloadService.Api.Types.Assessor;
using SFA.DAS.DownloadService.Api.Types.Roatp;
using SFA.DAS.DownloadService.Services.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.DownloadService.UnitTests.Mappers
{
    [TestFixture]
    public class AparMapperMapUkprnTests
    {
        private AparMapper _aparMapper;

        [SetUp]
        public void SetUp()
        {
            _aparMapper = new AparMapper();
        }

        [Test]
        public void Map_ReturnsNull_WhenBothRoatpAndEpaoResultsAreNull()
        {
            var result = _aparMapper.Map(null, null, ukprn => ukprn.ToString());

            Assert.IsNull(result);
        }

        [Test]
        public void Map_MapsRoatpResult_WhenRoatpResultIsNotNullAndEpaoResultIsNull()
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
                ProviderNotCurrentlyStartingNewApprentices = null,
            };
            
            Func<long, string> uriResolver = ukprn => $"http://example.com/{ukprn}";

            // Act
            var result = _aparMapper.Map(roatpResult, null, uriResolver);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.NotNull(result);
                Assert.AreEqual(12345678, result.Ukprn);
                Assert.AreEqual("Org1", result.Name);
                Assert.AreEqual("http://example.com/12345678", result.Uri);
                Assert.AreEqual(AparEntryType.MainProvider, result.ApplicationType);
                Assert.AreEqual(startDate.AddMonths(-1), result.StartDate);
                Assert.AreEqual(applicationDeterminedDate.AddMonths(-2), result.ApplicationDeterminedDate);
                Assert.IsFalse(result.CurrentlyNotStartingNewApprentices);
                Assert.IsNull(result.Epao);
            });
        }

        [Test]
        public void Map_MapsEpaoResult_WhenRoatpResultIsNullAndEpaoResultIsNotNull()
        {
            // Arrange
            var earliestEffectiveFromDate = DateTime.Now;
            var earliestDateStandardApprovedOnRegister = DateTime.Now.AddDays(-1);

            var epaoResult = new EpaoResult
            {
                Ukprn = 12345678,
                Name = "EpaoName",
                EarliestEffectiveFromDate = earliestEffectiveFromDate.AddMonths(-1),
                EarliestDateStandardApprovedOnRegister = earliestDateStandardApprovedOnRegister.AddMonths(-1)
            };

            Func<long, string> uriResolver = ukprn => $"http://example.com/{ukprn}";

            // Act
            var result = _aparMapper.Map(null, epaoResult, uriResolver);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.NotNull(result);
                Assert.AreEqual(12345678, result.Ukprn);
                Assert.AreEqual("EpaoName", result.Name);
                Assert.AreEqual("http://example.com/12345678", result.Uri);
                Assert.AreEqual(AparEntryType.EPAO, result.ApplicationType);
                Assert.AreEqual(earliestEffectiveFromDate.AddMonths(-1), result.StartDate);
                Assert.AreEqual(earliestDateStandardApprovedOnRegister.AddMonths(-1), result.ApplicationDeterminedDate);
                Assert.IsNull(result.CurrentlyNotStartingNewApprentices);

                Assert.IsNotNull(result.Epao);
                Assert.AreEqual(epaoResult.Name, result.Epao.Name);
                Assert.AreEqual(earliestEffectiveFromDate.AddMonths(-1), result.Epao.StartDate);
                Assert.AreEqual(earliestDateStandardApprovedOnRegister.AddMonths(-1), result.Epao.ApplicationDeterminedDate);
            });
        }

        [Test]
        public void Map_MapsBoth_WhenBothRoatpAndEpaoResultsAreNotNull()
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
                ProviderNotCurrentlyStartingNewApprentices = null,
            };

            var earliestEffectiveFromDate = DateTime.Now;
            var earliestDateStandardApprovedOnRegister = DateTime.Now.AddDays(-1);

            var epaoResult = new EpaoResult
            {
                Ukprn = 12345678,
                Name = "EpaoName",
                EarliestEffectiveFromDate = earliestEffectiveFromDate.AddMonths(-1),
                EarliestDateStandardApprovedOnRegister = earliestDateStandardApprovedOnRegister.AddMonths(-1)
            };

            Func<long, string> uriResolver = ukprn => $"http://example.com/{ukprn}";

            // Act
            var result = _aparMapper.Map(roatpResult, epaoResult, uriResolver);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.NotNull(result);
                Assert.AreEqual(12345678, result.Ukprn);
                Assert.AreEqual("Org1", result.Name);
                Assert.AreEqual("http://example.com/12345678", result.Uri);
                Assert.AreEqual(AparEntryType.MainProvider, result.ApplicationType);
                Assert.AreEqual(startDate.AddMonths(-1), result.StartDate);
                Assert.AreEqual(applicationDeterminedDate.AddMonths(-2), result.ApplicationDeterminedDate);
                Assert.IsFalse(result.CurrentlyNotStartingNewApprentices);
                
                Assert.IsNotNull(result.Epao);
                Assert.AreEqual(epaoResult.Name, result.Epao.Name);
                Assert.AreEqual(earliestEffectiveFromDate.AddMonths(-1), result.Epao.StartDate);
                Assert.AreEqual(earliestDateStandardApprovedOnRegister.AddMonths(-1), result.Epao.ApplicationDeterminedDate);
            });
        }
    }

}
