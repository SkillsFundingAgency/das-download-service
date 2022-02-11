using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.DownloadService.Api.Types.Roatp;
using SFA.DAS.DownloadService.Services.Interfaces;
using SFA.DAS.DownloadService.Services.Services.Roatp;
using SFA.DAS.DownloadService.Web.Controllers;
using SFA.DAS.Roatp.Api.Client.Interfaces;

namespace SFA.DAS.DownloadService.UnitTests.Controllers
{
    [TestFixture]
    public class RoatpControllerTests
    {
        private RoatpController _controller;
        private Mock<ILogger<RoatpController>> _mockLogger;
        private Mock<IDownloadServiceApiClient> _mockClient;

        [SetUp]
        public void Init()
        {
            _mockLogger = new Mock<ILogger<RoatpController>>();
            _mockClient = new Mock<IDownloadServiceApiClient>();
            _controller = new RoatpController(_mockClient.Object, Mock.Of<RoatpMapper>(), Mock.Of<IRetryService>(), _mockLogger.Object);
        }

        [Test]
        public void Csv_NoRecordsReturnedFromApi_RedirectToServiceUnavailable()
        {
            _mockClient.Setup(x => x.GetRoatpSummary()).ReturnsAsync(new List<Provider>());
            var result =  _controller.Csv();
            var redirectResult = result as RedirectToActionResult;
            Assert.AreEqual("ServiceUnavailable",redirectResult.ActionName);
            _mockClient.Verify(x=> x.GetRoatpSummary(),Times.Once);
        }

        [Test]
        public void Csv_RecordsReturnedFromApi_ExpectedCSVDownloaded()
        {
            var dateUpdated = DateTime.Now.AddDays(-1);
            _mockClient.Setup(x => x.GetRoatpSummary()).ReturnsAsync(new List<Provider> {new Provider()});
            _mockClient.Setup(x => x.GetLatestNonOnboardingOrganisationChangeDate()).ReturnsAsync(dateUpdated);
            var result = _controller.Csv();
            var fileDownloadResult = result as FileContentResult;
            var expectedFileName = $"roatp-{dateUpdated:yyyy-MM-dd-HH-mm-ss}.csv";
            Assert.AreEqual("text/csv", fileDownloadResult.ContentType);
            Assert.AreEqual(expectedFileName,fileDownloadResult.FileDownloadName);
            _mockClient.Verify(x => x.GetRoatpSummary(), Times.Once);
            _mockClient.Verify(x => x.GetLatestNonOnboardingOrganisationChangeDate(), Times.Once);
        }
    }
}