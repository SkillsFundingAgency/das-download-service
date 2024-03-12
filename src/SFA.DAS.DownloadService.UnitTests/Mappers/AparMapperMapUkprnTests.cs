using System;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using SFA.DAS.DownloadService.Api.Types;
using SFA.DAS.DownloadService.Api.Types.Assessor;
using SFA.DAS.DownloadService.Api.Types.Roatp;
using SFA.DAS.DownloadService.Services.Services;

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

            ClassicAssert.IsNull(result);
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
            ClassicAssert.Multiple(() =>
            {
                ClassicAssert.NotNull(result);
                ClassicAssert.That(12345678, Is.EqualTo(result.Ukprn));
                ClassicAssert.AreEqual("Org1", result.Name);
                ClassicAssert.AreEqual("http://example.com/12345678", result.Uri);
                ClassicAssert.AreEqual(AparEntryType.MainProvider, result.ApplicationType);
                ClassicAssert.AreEqual(startDate.AddMonths(-1), result.StartDate);
                ClassicAssert.AreEqual(applicationDeterminedDate.AddMonths(-2), result.ApplicationDeterminedDate);
                ClassicAssert.IsFalse(result.CurrentlyNotStartingNewApprentices);
                ClassicAssert.IsNull(result.Epao);
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
            ClassicAssert.Multiple(() =>
            {
                ClassicAssert.NotNull(result);
                ClassicAssert.AreEqual(12345678, result.Ukprn);
                ClassicAssert.AreEqual("EpaoName", result.Name);
                ClassicAssert.AreEqual("http://example.com/12345678", result.Uri);
                ClassicAssert.AreEqual(AparEntryType.EPAO, result.ApplicationType);
                ClassicAssert.AreEqual(earliestEffectiveFromDate.AddMonths(-1), result.StartDate);
                ClassicAssert.AreEqual(earliestDateStandardApprovedOnRegister.AddMonths(-1), result.ApplicationDeterminedDate);
                ClassicAssert.IsNull(result.CurrentlyNotStartingNewApprentices);

                ClassicAssert.IsNotNull(result.Epao);
                ClassicAssert.AreEqual(epaoResult.Name, result.Epao.Name);
                ClassicAssert.AreEqual(earliestEffectiveFromDate.AddMonths(-1), result.Epao.StartDate);
                ClassicAssert.AreEqual(earliestDateStandardApprovedOnRegister.AddMonths(-1), result.Epao.ApplicationDeterminedDate);
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
            ClassicAssert.Multiple(() =>
            {
                ClassicAssert.NotNull(result);
                ClassicAssert.AreEqual(12345678, result.Ukprn);
                ClassicAssert.AreEqual("Org1", result.Name);
                ClassicAssert.AreEqual("http://example.com/12345678", result.Uri);
                ClassicAssert.AreEqual(AparEntryType.MainProvider, result.ApplicationType);
                ClassicAssert.AreEqual(startDate.AddMonths(-1), result.StartDate);
                ClassicAssert.AreEqual(applicationDeterminedDate.AddMonths(-2), result.ApplicationDeterminedDate);
                ClassicAssert.IsFalse(result.CurrentlyNotStartingNewApprentices);

                ClassicAssert.IsNotNull(result.Epao);
                ClassicAssert.AreEqual(epaoResult.Name, result.Epao.Name);
                ClassicAssert.AreEqual(earliestEffectiveFromDate.AddMonths(-1), result.Epao.StartDate);
                ClassicAssert.AreEqual(earliestDateStandardApprovedOnRegister.AddMonths(-1), result.Epao.ApplicationDeterminedDate);
            });
        }
    }

}
