using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.DownloadService.Api.Types;
using SFA.DAS.DownloadService.Services.Services;
using SFA.DAS.DownloadService.Web.Controllers;
using SFA.DAS.Roatp.Api.Client.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.DownloadService.UnitTests.Controllers
{
    [TestFixture]
    public class RoatpControllerTests
    {
        private AparController _controller;
        private Mock<ILogger<AparController>> _mockLogger;
        private Mock<IDownloadServiceApiClient> _mockClient;

        [SetUp]
        public void Init()
        {
            _mockLogger = new Mock<ILogger<AparController>>();
            _mockClient = new Mock<IDownloadServiceApiClient>();
            _controller = new AparController(_mockClient.Object, Mock.Of<AparMapper>(), _mockLogger.Object);
        }

        [Test]
        public async Task Csv_NoRecordsReturnedFromApi_RedirectToServiceUnavailable()
        {
            _mockClient.Setup(x => x.GetAparSummary()).ReturnsAsync(new List<AparEntry>());
            var result = await _controller.DownloadCsv();
            var redirectResult = result as RedirectToActionResult;
            Assert.AreEqual("ServiceUnavailable",redirectResult.ActionName);
            _mockClient.Verify(x=> x.GetAparSummary(),Times.Once);
        }

        [Test]
        public async Task Csv_RecordsReturnedFromApi_ExpectedCSVDownloaded()
        {
            var dateUpdated = DateTime.Now.AddDays(-1);
            _mockClient.Setup(x => x.GetAparSummary()).ReturnsAsync(new List<AparEntry> {new AparEntry()});
            _mockClient.Setup(x => x.GetLatestNonOnboardingOrganisationChangeDate()).ReturnsAsync(dateUpdated);
            var result = await _controller.DownloadCsv();
            var fileDownloadResult = result as FileContentResult;
            var expectedFileName = $"roatp-{dateUpdated:yyyy-MM-dd-HH-mm-ss}.csv";
            Assert.AreEqual("text/csv", fileDownloadResult.ContentType);
            Assert.AreEqual(expectedFileName,fileDownloadResult.FileDownloadName);
            _mockClient.Verify(x => x.GetAparSummary(), Times.Once);
            _mockClient.Verify(x => x.GetLatestNonOnboardingOrganisationChangeDate(), Times.Once);
        }
    }
}