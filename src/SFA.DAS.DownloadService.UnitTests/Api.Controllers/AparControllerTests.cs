using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.DownloadService.Api.Controllers;
using SFA.DAS.DownloadService.Api.Types;
using SFA.DAS.DownloadService.Api.Types.Roatp;
using SFA.DAS.DownloadService.Services.Interfaces;
using SFA.DAS.DownloadService.Api.Client.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using SFA.DAS.DownloadService.Api.Infrastructure;

namespace SFA.DAS.DownloadService.UnitTests.Api.Controllers
{
    [TestFixture]
    public class AparControllerTests
    {
        private AparController _controller;
        private Mock<ILogger<AparController>> _mockLogger;
        private Mock<IRoatpApiClient> _mockRoatpApiClient;
        private Mock<IAparMapper> _mockMapper;
        private Mock<IDateTimeProvider> _mockDateTimeProvider;

        protected Mock<HttpContext> HttpContext;
        protected Mock<HttpRequest> HttpContextRequest;
        protected Mock<HttpResponse> HttpResponseMock;

        [SetUp]
        public void Init()
        {
            _mockLogger = new Mock<ILogger<AparController>>();
            _mockRoatpApiClient = new Mock<IRoatpApiClient>();
            _mockDateTimeProvider = new Mock<IDateTimeProvider>();

            _mockMapper = new Mock<IAparMapper>();
            _mockRoatpApiClient.Setup(z => z.GetRoatpSummaryByUkprn(It.IsAny<int>())).ReturnsAsync((IEnumerable<RoatpResult>)null);
            _mockRoatpApiClient.Setup(z => z.GetRoatpSummary()).ReturnsAsync((IEnumerable<RoatpResult>)null);

            HttpContextRequest = new Mock<HttpRequest>();
            HttpContextRequest.Setup(r => r.Method).Returns("GET");
            HttpContext = new Mock<HttpContext>();
            HttpContext.Setup(x => x.Request.Scheme).Returns("http");
            HttpContext.Setup(x => x.Request.Host).Returns(new HostString("localhost"));

            _controller = new AparController(_mockRoatpApiClient.Object, _mockMapper.Object, _mockDateTimeProvider.Object, _mockLogger.Object);

            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = HttpContext.Object;
        }

        [TestCase(1)]
        [TestCase(12)]
        [TestCase(123)]
        [TestCase(1234)]
        [TestCase(12345)]
        [TestCase(123456)]
        [TestCase(1234567)]
        [TestCase(123456789)]
        public void ShouldThrowBadRequestIfUkprnIsInvalid(int ukprn)
        {
            // Act
            var res = _controller.Get(ukprn).Result;

            // Assert
            Assert.AreEqual((int)HttpStatusCode.BadRequest, ((ObjectResult)res).StatusCode);
        }

        [Test]
        public async Task Get_ReturnsNotFound_WhenNoResults()
        {
            // Arrange
            var validUkprn = 12345678;

            // Set up the mocks to return no results
            _mockRoatpApiClient.Setup(x => x.GetRoatpSummaryByUkprn(It.IsAny<int>()))
                .ReturnsAsync(new List<RoatpResult>());
            
            // Act
            var result = await _controller.Get(validUkprn);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public async Task Get_ReturnsOk_WhenResultsFound()
        {
            // Arrange
            _mockDateTimeProvider.Setup(z => z.GetCurrentDateTime()).Returns(new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc));

            var roatpResult = new RoatpResult
            {
                Ukprn = 12345678,
                StartDate = _mockDateTimeProvider.Object.GetCurrentDateTime().AddMonths(-1),
                EndDate = _mockDateTimeProvider.Object.GetCurrentDateTime().AddMonths(1)
            };

            // Set up the mocks to return results
            _mockRoatpApiClient.Setup(x => x.GetRoatpSummaryByUkprn(It.IsAny<int>()))
                .ReturnsAsync(new List<RoatpResult> { roatpResult });

            _mockMapper.Setup(x => x.Map(It.IsAny<RoatpResult>(), It.IsAny<Func<long, string>>()))
                .Returns(new UkprnAparEntry 
                {
                    Ukprn = roatpResult.Ukprn,
                    StartDate = roatpResult.StartDate
                });

            // Act
            var result = await _controller.Get((int)roatpResult.Ukprn);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [TestCase("01/01/2000", "01/01/2000")]
        [TestCase("01/01/2222", "01/01/2222")]
        [TestCase(null, "01/01/1900")]
        public async Task GetLatestTime_ReturnsCorrectDateTime(DateTime? roatpDate, DateTime expected)
        {
            // Arrange
            _mockDateTimeProvider.Setup(z => z.GetCurrentDateTime()).Returns(new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc));

            _mockRoatpApiClient.Setup(x => x.GetLatestNonOnboardingOrganisationChangeDate())
                .ReturnsAsync(roatpDate);

            // Act
            var result = await _controller.GetLatestTime() as OkObjectResult;

            //Assert
            Assert.AreEqual(expected, result.Value);
        }
    }
}
