using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using SFA.DAS.DownloadService.Api.Types;
using SFA.DAS.DownloadService.Api.Types.Assessor;
using SFA.DAS.DownloadService.Services.Services;

namespace SFA.DAS.DownloadService.UnitTests.Mappers
{
    [TestFixture]
    public class AparMapperEpaoResultTests
    {
        private AparMapper _mapper;

        [SetUp]
        public void SetUp()
        {
            _mapper = new AparMapper();
        }

        [Test]
        public void Map_EpaoResultToAparEntry_ValidInput_MapsCorrectly()
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
            var result = _mapper.Map(epaoResult, uriResolver);

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
            });
        }

        // Testing List<EpaoResult> mapping
        [Test]
        public void Map_ListOfEpaoResult_MapsCorrectly()
        {
            // Arrange
            // Arrange
            var earliestEffectiveFromDate = DateTime.Now;
            var earliestDateStandardApprovedOnRegister = DateTime.Now.AddDays(-1);

            var epaoResults = new List<EpaoResult>
            {
                new EpaoResult
                {
                    Ukprn = 12345678,
                    Name = "EpaoName1",
                    EarliestEffectiveFromDate = earliestEffectiveFromDate.AddMonths(-1),
                    EarliestDateStandardApprovedOnRegister = earliestDateStandardApprovedOnRegister.AddMonths(-1)
                },
                new EpaoResult
                {
                    Ukprn = 23456789,
                    Name = "EpaoName2",
                    EarliestEffectiveFromDate = earliestEffectiveFromDate.AddMonths(-2),
                    EarliestDateStandardApprovedOnRegister = earliestDateStandardApprovedOnRegister.AddMonths(-2)
                }
            };

            Func<long, string> uriResolver = ukprn => $"http://example.com/{ukprn}";

            // Act
            var results = _mapper.Map(epaoResults, uriResolver);

            // Assert
            ClassicAssert.Multiple(() =>
            {
                ClassicAssert.NotNull(results);
                ClassicAssert.AreEqual(12345678, results[0].Ukprn);
                ClassicAssert.AreEqual("EpaoName1", results[0].Name);
                ClassicAssert.AreEqual("http://example.com/12345678", results[0].Uri);
                ClassicAssert.AreEqual(AparEntryType.EPAO, results[0].ApplicationType);
                ClassicAssert.AreEqual(earliestEffectiveFromDate.AddMonths(-1), results[0].StartDate);
                ClassicAssert.AreEqual(earliestDateStandardApprovedOnRegister.AddMonths(-1), results[0].ApplicationDeterminedDate);
                ClassicAssert.IsNull(results[0].CurrentlyNotStartingNewApprentices);

                ClassicAssert.AreEqual(23456789, results[1].Ukprn);
                ClassicAssert.AreEqual("EpaoName2", results[1].Name);
                ClassicAssert.AreEqual("http://example.com/23456789", results[1].Uri);
                ClassicAssert.AreEqual(AparEntryType.EPAO, results[1].ApplicationType);
                ClassicAssert.AreEqual(earliestEffectiveFromDate.AddMonths(-2), results[1].StartDate);
                ClassicAssert.AreEqual(earliestDateStandardApprovedOnRegister.AddMonths(-2), results[1].ApplicationDeterminedDate);
                ClassicAssert.IsNull(results[1].CurrentlyNotStartingNewApprentices);
            });
        }
    }
}
