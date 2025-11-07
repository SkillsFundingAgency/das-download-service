using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.DownloadService.Api.Client.Interfaces;
using SFA.DAS.DownloadService.Api.Controllers;
using SFA.DAS.DownloadService.Api.Infrastructure;
using SFA.DAS.DownloadService.Api.Types.Roatp.Common;
using SFA.DAS.DownloadService.Api.Types.Roatp.Models;
using SFA.DAS.DownloadService.Api.Types.Roatp.Responses;
using SFA.DAS.DownloadService.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

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
            _mockRoatpApiClient.Setup(z => z.GetRoatpSummaryByUkprn(It.IsAny<int>())).ReturnsAsync((OrganisationModel)null);
            _mockRoatpApiClient.Setup(z => z.GetRoatpSummary()).ReturnsAsync((GetOrganisationResponse)null);

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
                .ReturnsAsync(new OrganisationModel());

            // Act
            var result = await _controller.Get(validUkprn);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public async Task Get_ReturnsOk_WhenResultsFound()
        {
            // Arrange

            var model = new OrganisationModel
            {
                Ukprn = 12345678,
                Status = OrganisationStatus.Active
            };

            // Set up the mocks to return results
            _mockRoatpApiClient.Setup(x => x.GetRoatpSummaryByUkprn(It.IsAny<int>()))
                .ReturnsAsync(model);

            // Act
            var result = await _controller.Get((int)model.Ukprn);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task GetAll_OkResponse_ReturnsOrganisationsThatAreNotInRemovedStatus()
        {
            // Arrange
            var response = new GetOrganisationResponse
            {
                Organisations = new List<OrganisationModel>
                {
                    new OrganisationModel
                    {
                        Ukprn = 12345678,
                        Status = OrganisationStatus.Active
                    },
                    new OrganisationModel
                    {
                        Ukprn = 19876543,
                        Status = OrganisationStatus.OnBoarding
                    },
                    new OrganisationModel
                    {
                        Ukprn = 14376543,
                        Status = OrganisationStatus.Removed
                    }
                }
            };

            // Set up the mocks to return results
            _mockRoatpApiClient.Setup(x => x.GetRoatpSummary())
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetAll();
            var okResult = result as OkObjectResult;

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.That(okResult.Value, Has.Count.EqualTo(2));
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
