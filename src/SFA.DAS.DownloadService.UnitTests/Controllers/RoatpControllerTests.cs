using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.DownloadService.Api.Types.Roatp;
using SFA.DAS.DownloadService.Services.Interfaces;
using SFA.DAS.DownloadService.Services.Services;
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
        private IRoatpMapper _mapper;
        private Mock<IHostingEnvironment> _mockEnv;
        private IRetryService _retryService;
        private Mock<ILogger<RetryService>> _mockRetryServiceLogger;
        protected Mock<HttpContext> HttpContext;
        protected Mock<HttpRequest> HttpContextRequest;

        [SetUp]
        public void Init()
        {
            _mockLogger = new Mock<ILogger<RoatpController>>();
            _mockClient = new Mock<IDownloadServiceApiClient>();
            _mockEnv = new Mock<IHostingEnvironment>();
            _mockRetryServiceLogger = new Mock<ILogger<RetryService>>();
            _retryService = new RetryService(_mockRetryServiceLogger.Object);
            _mapper = new RoatpMapper();
            _mockClient.Setup(z => z.GetRoatpSummary()).ReturnsAsync((IEnumerable<Provider>)null);
            HttpContextRequest = new Mock<HttpRequest>();
            HttpContextRequest.Setup(r => r.Method).Returns("GET");
            HttpContext = new Mock<HttpContext>();
            HttpContext.Setup(x => x.Request.Scheme).Returns("http");
            HttpContext.Setup(x => x.Request.Host).Returns(new HostString("localhost"));

            _controller = new RoatpController(_mockClient.Object, _mapper, _retryService, _mockLogger.Object);
            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = HttpContext.Object;
        }

        [Test]
        public async Task Csv_NoRecordsReturnedFromApi_RedirectToServiceUnavailable()
        {
            _mockClient.Setup(x => x.GetRoatpSummary()).ReturnsAsync(new List<Provider>());
            var result = await _controller.Csv();
            var redirectResult = result as RedirectToActionResult;
            Assert.AreEqual("ServiceUnavailable", redirectResult.ActionName);
            _mockClient.Verify(x => x.GetRoatpSummary(), Times.Once);
        }

        [Test]
        public async Task Csv_ApiNotResponding_RedirectToServiceUnavailable()
        {
            _mockClient.Setup(x => x.GetRoatpSummary()).ThrowsAsync(new Exception());
            var result = await _controller.Csv();
            var redirectResult = result as RedirectToActionResult;
            Assert.AreEqual("ServiceUnavailable", redirectResult.ActionName);
        }

        [Test]
        public async Task Csv_RecordsReturnedFromApi_ExpectedCSVDownloaded()
        {
            var dateUpdated = DateTime.Now.AddDays(-1);
            _mockClient.Setup(x => x.GetRoatpSummary()).ReturnsAsync(new List<Provider> { new Provider() });
            _mockClient.Setup(x => x.GetLatestNonOnboardingOrganisationChangeDate()).ReturnsAsync(dateUpdated);
            var result = await _controller.Csv();
            var fileDownloadResult = result as FileContentResult;
            var expectedFileName = $"roatp-{dateUpdated:yyyy-MM-dd-HH-mm-ss}.csv";
            Assert.AreEqual("text/csv", fileDownloadResult.ContentType);
            Assert.AreEqual(expectedFileName, fileDownloadResult.FileDownloadName);
            _mockClient.Verify(x => x.GetRoatpSummary(), Times.Once);
            _mockClient.Verify(x => x.GetLatestNonOnboardingOrganisationChangeDate(), Times.Once);
        }
    }
}