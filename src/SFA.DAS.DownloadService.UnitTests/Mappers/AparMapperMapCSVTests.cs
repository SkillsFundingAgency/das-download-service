using NUnit.Framework;
using SFA.DAS.DownloadService.Api.Types;
using SFA.DAS.DownloadService.Api.Types.Roatp;
using SFA.DAS.DownloadService.Services.Services;
using System;
using System.Collections.Generic;
using System.Text;

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
            Assert.Multiple(() =>
            {
                Assert.NotNull(result);
                Assert.AreEqual(12345678, result.Ukprn);
                Assert.AreEqual("TestName", result.Name);
                Assert.AreEqual("Main provider", result.ApplicationType); //Assuming GetEnumDescription returns string representation of the enum value
                Assert.AreEqual("Not Currently Starting New Apprentices", result.Status);
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
            Assert.Multiple(() =>
            {
                Assert.NotNull(results);
                Assert.AreEqual(2, results.Count);
                Assert.AreEqual(12345678, results[0].Ukprn);
                Assert.AreEqual("TestName1", results[0].Name);
                Assert.AreEqual("Main provider", results[0].ApplicationType);
                Assert.AreEqual("Not Currently Starting New Apprentices", results[0].Status);

                Assert.AreEqual(87654321, results[1].Ukprn);
                Assert.AreEqual("TestName2", results[1].Name);
                Assert.AreEqual("EPAO", results[1].ApplicationType);
                Assert.AreEqual(string.Empty, results[1].Status);
            });
        }
    }
}
