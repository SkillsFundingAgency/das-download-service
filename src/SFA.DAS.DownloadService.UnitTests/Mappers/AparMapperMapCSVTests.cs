using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using SFA.DAS.DownloadService.Api.Types;
using SFA.DAS.DownloadService.Services.Services;

namespace SFA.DAS.DownloadService.UnitTests.Mappers
{
    [TestFixture]
    public class AparMapperMapCSVTests
    {
        private AparMapper _mapper;

        [SetUp]
        public void SetUp()
        {
            _mapper = new AparMapper();
        }

        [Test]
        public void MapCsv_SingleAparEntry_MapsCorrectly()
        {
            // Arrange
            var aparEntry = new AparEntry
            {
                Ukprn = 12345678,
                Name = "TestName",
                ApplicationType = AparEntryType.MainProvider,
                StartDate = DateTime.Now,
                CurrentlyNotStartingNewApprentices = true,
                ApplicationDeterminedDate = DateTime.Now
            };

            // Act
            var result = _mapper.MapCsv(aparEntry);

            // Assert
            ClassicAssert.Multiple(() =>
            {
                ClassicAssert.NotNull(result);
                ClassicAssert.AreEqual(12345678, result.Ukprn);
                ClassicAssert.AreEqual("TestName", result.Name);
                ClassicAssert.AreEqual("Main provider", result.ApplicationType); //Assuming GetEnumDescription returns string representation of the enum value
                ClassicAssert.AreEqual("Not Currently Starting New Apprentices", result.Status);
            });
        }

        [Test]
        public void MapCsv_ListOfAparEntries_MapsCorrectly()
        {
            // Arrange
            var aparEntries = new List<AparEntry>
            {
                new AparEntry
                {
                    Ukprn = 12345678,
                    Name = "TestName1",
                    ApplicationType = AparEntryType.MainProvider,
                    StartDate = DateTime.Now,
                    CurrentlyNotStartingNewApprentices = true,
                    ApplicationDeterminedDate = DateTime.Now
                },
                new AparEntry
                {
                    Ukprn = 87654321,
                    Name = "TestName2",
                    ApplicationType = AparEntryType.EPAO,
                    StartDate = DateTime.Now,
                    CurrentlyNotStartingNewApprentices = false,
                    ApplicationDeterminedDate = DateTime.Now
                }
            };

            // Act
            var results = _mapper.MapCsv(aparEntries);

            // Assert
            ClassicAssert.Multiple(() =>
            {
                ClassicAssert.NotNull(results);
                ClassicAssert.AreEqual(2, results.Count);
                ClassicAssert.AreEqual(12345678, results[0].Ukprn);
                ClassicAssert.AreEqual("TestName1", results[0].Name);
                ClassicAssert.AreEqual("Main provider", results[0].ApplicationType);
                ClassicAssert.AreEqual("Not Currently Starting New Apprentices", results[0].Status);

                ClassicAssert.AreEqual(87654321, results[1].Ukprn);
                ClassicAssert.AreEqual("TestName2", results[1].Name);
                ClassicAssert.AreEqual("EPAO", results[1].ApplicationType);
                ClassicAssert.AreEqual(string.Empty, results[1].Status);
            });
        }
    }
}
