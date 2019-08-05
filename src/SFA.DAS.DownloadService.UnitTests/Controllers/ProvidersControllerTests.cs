

using System;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.DownloadService.Api.Types.Roatp;
using SFA.DAS.DownloadService.Web.Controllers;
using SFA.DAS.Roatp.Api.Client.Interfaces;
using SFA.DAS.Roatp.ApplicationServices.Interfaces;

namespace SFA.DAS.DownloadService.UnitTests.Controllers
{
    [TestFixture]
    public class ProvidersControllerTests
    {


        private ProvidersController _controller;
        private Moq.Mock<ILogger<ProvidersController>> _mockLogger;
        private Mock<IRoatpApiClient> _mockClient;
        private Mock<IRoatpMapper> _mockMapper;
        private Mock<IHostingEnvironment> _mockEnv;


        protected Mock<HttpContext> HttpContext;
        protected Mock<HttpRequest> HttpContextRequest;
        protected Mock<HttpResponse> HttpResponseMock;

        private long _ukprn;
        [SetUp]
        public void Init()
        {
            _ukprn = 12345678;
            _mockLogger = new Mock<ILogger<ProvidersController>>();
            _mockClient = new Mock<IRoatpApiClient>();
            _mockMapper = new Mock<IRoatpMapper>();
            _mockEnv = new Mock<IHostingEnvironment>();
            _mockClient.Setup(z => z.GetRoatpSummaryByUkprn(It.IsAny<int>())).ReturnsAsync((RoatpResult)null);


            HttpContextRequest = new Mock<HttpRequest>();
            HttpContextRequest.Setup(r => r.Method).Returns("GET");
            HttpContext = new Mock<HttpContext>();
            HttpContext.Setup(x => x.Request.Scheme).Returns("http");
            HttpContext.Setup(x => x.Request.Host).Returns(new HostString("localhost"));

            _controller = new ProvidersController(_mockLogger.Object, _mockClient.Object, _mockMapper.Object,
               _mockEnv.Object);

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
            var res = _controller.Get(ukprn).Result;
            Assert.AreEqual((int)HttpStatusCode.BadRequest, ((ObjectResult)res).StatusCode);
        }

        [TestCase("goodukprn", "today", (int)HttpStatusCode.OK)]
        [TestCase("goodukprn", "tomorrow", (int)HttpStatusCode.NotFound)]
        [TestCase("goodukprn", "yesterday", (int)HttpStatusCode.OK)]
        [TestCase("badurkpn", "today", (int)HttpStatusCode.NotFound)]
        [TestCase("badukrpn", "tomorrow", (int)HttpStatusCode.NotFound)]
        [TestCase("badukprn", "yesterday", (int)HttpStatusCode.NotFound)]

        public void ShouldThroNotFoundIfUkprnNotMatchedOrStartDateNotTodayOrAfter(string ukprnType, string startDate, int httpStatusCode)
        {

            DateTime startDateToUse;
            switch (startDate)
            {
                case "tomorrow":
                    startDateToUse = DateTime.Today.AddDays(1);
                    break;
                case "yesterday":
                    startDateToUse = DateTime.Today.AddDays(-1);
                    break;
                default:
                    startDateToUse = DateTime.Today;
                    break;
            }

            var provider = new Provider
            {
                Ukprn = _ukprn,
                StartDate = startDateToUse
            };

            if (ukprnType == "goodukprn")
            {
                _mockMapper.Setup(z => z.Map(It.IsAny<RoatpResult>())).Returns(provider);
            }
            else
            {
                _mockMapper.Setup(z => z.Map(It.IsAny<RoatpResult>())).Returns((Provider)null);
            }
            var resultFromGet = _controller.Get(12345678).Result;

            Assert.IsTrue(((ObjectResult)resultFromGet).StatusCode == httpStatusCode);

            if (httpStatusCode == 200)
            {
                var returnedProvider = (Provider)((ObjectResult)resultFromGet).Value;
                Assert.AreEqual(_ukprn, returnedProvider.Ukprn);
            }
            else
            {
                var returnedDetails = (string)((ObjectResult)resultFromGet).Value;
                Assert.IsNotEmpty(returnedDetails);
            }
        }
    }
}
