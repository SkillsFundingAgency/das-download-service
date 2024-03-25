using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using SFA.DAS.DownloadService.Api.Client.Interfaces;
using SFA.DAS.DownloadService.Api.Types;
using SFA.DAS.DownloadService.Services.Services;
using SFA.DAS.DownloadService.Web.Controllers;

namespace SFA.DAS.DownloadService.UnitTests.Web.Controllers
{
    [TestFixture]
    public class AparControllerTests
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
        public async Task When_DownloadCsv_IsCalled_GetAparSummary_IsCalled()
        {
            _mockClient.Setup(x => x.GetAparSummary()).ReturnsAsync(new List<AparEntry>());
            var result = await _controller.DownloadCsv();
            _mockClient.Verify(x => x.GetAparSummary(), Times.Once);
        }

        [Test]
        public async Task When_DownloadCsv_IsCalled_And_NoRecordsAreReturnedFromGetAparSummary_RedirectToServiceUnavailable()
        {
            _mockClient.Setup(x => x.GetAparSummary()).ReturnsAsync(new List<AparEntry>());
            var result = await _controller.DownloadCsv();
            var redirectResult = result as RedirectToActionResult;
            ClassicAssert.AreEqual("ServiceUnavailable", redirectResult.ActionName);
        }

        public async Task When_DownloadCsv_IsCalled_And_RecordsAreReturnedFromGetAparSummary_GetLatestNonOnboardingOrganisationChangeDate_IsCalled()
        {
            var dateUpdated = DateTime.Now.AddDays(-1);
            _mockClient.Setup(x => x.GetAparSummary()).ReturnsAsync(new List<AparEntry> { new AparEntry() });
            _mockClient.Setup(x => x.GetLatestNonOnboardingOrganisationChangeDate()).ReturnsAsync(dateUpdated);
            var result = await _controller.DownloadCsv();
            _mockClient.Verify(x => x.GetLatestNonOnboardingOrganisationChangeDate(), Times.Once);
        }

        [Test]
        public async Task When_DownloadCsv_IsCalled_And_RecordsAreReturnedFromGetAparSummary_ExpectedCSVDownloaded()
        {
            var dateUpdated = DateTime.Now.AddDays(-1);
            _mockClient.Setup(x => x.GetAparSummary()).ReturnsAsync(new List<AparEntry> { new AparEntry() });
            _mockClient.Setup(x => x.GetLatestNonOnboardingOrganisationChangeDate()).ReturnsAsync(dateUpdated);
            var result = await _controller.DownloadCsv();
            var fileDownloadResult = result as FileContentResult;
            var expectedFileName = $"apar-{dateUpdated:yyyy-MM-dd-HH-mm-ss}.csv";

            ClassicAssert.Multiple(() =>
            {
                ClassicAssert.AreEqual("text/csv", fileDownloadResult.ContentType);
                ClassicAssert.AreEqual(expectedFileName, fileDownloadResult.FileDownloadName);
            });
        }

        [Test]
        public void IndexRoapt_ShouldLogWarning()
        {
            // Act
            _controller.IndexRoatp();

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<object>(v => v.ToString().Contains("Deprecated endpoint 'roatp' called for AparController")),
                    It.IsAny<Exception>(),
                    (Func<object, Exception, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Test]
        public void IndexRoapt_ShouldRedirectToRouteAparGetIndex()
        {
            // Act
            var result = _controller.IndexRoatp() as RedirectToRouteResult;

            // Assert
            ClassicAssert.AreEqual("RouteAparGetIndex", result?.RouteName);
        }

        [Test]
        public void DownloadCsvRoatp_ShouldLogWarning()
        {
            // Act
            _controller.DownloadCsvRoatp();

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<object>(v => v.ToString().Contains("Deprecated endpoint 'roatp/downloadcsv' called for AparController")),
                    It.IsAny<Exception>(),
                    (Func<object, Exception, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Test]
        public void DownloadCsvRoatp_ShouldRedirectToRouteAparDownloadCsv()
        {
            // Act
            var result = _controller.DownloadCsvRoatp() as RedirectToRouteResult;

            // Assert
            ClassicAssert.AreEqual("RouteAparDownloadCsv", result?.RouteName);
        }
    }
}
