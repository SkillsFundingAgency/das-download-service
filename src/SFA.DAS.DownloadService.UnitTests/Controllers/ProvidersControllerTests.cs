

//using System;
//using System.Collections.Generic;
//using System.Net;
//using System.Net.Http;
//using System.Threading;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Logging;
//using Moq;
//using Moq.Protected;
//using Newtonsoft.Json;
//using NUnit.Framework;
//using SFA.DAS.DownloadService.Api.Roatp.Controllers;
//using SFA.DAS.DownloadService.Api.Types.Roatp;
//using SFA.DAS.DownloadService.Services.Interfaces;
//using SFA.DAS.DownloadService.Services.Services;
//using SFA.DAS.DownloadService.Services.Services.Roatp;
//using SFA.DAS.Roatp.Api.Client;
//using SFA.DAS.Roatp.Api.Client.Interfaces;

//namespace SFA.DAS.DownloadService.UnitTests.Controllers
//{
//    [TestFixture]
//    public class ProvidersControllerTests
//    {


//        private ProvidersController _controller;
//        private Moq.Mock<ILogger<ProvidersController>> _mockLogger;
//        private RoatpApiClient _client;
//        private IRoatpMapper _mapper;
//        private Mock<IHostingEnvironment> _mockEnv;
//        private IRetryService _retryService;

//        private Mock<ILogger<RoatpApiClient>> _clientLogger;

//        private Mock<ILogger<RetryService>> _mockRetryServiceLogger;

//        protected Mock<HttpContext> HttpContext;
//        protected Mock<HttpRequest> HttpContextRequest;
//        protected Mock<HttpResponse> HttpResponseMock;

//        private Mock<HttpMessageHandler> _mockMessageHandler;


//        private int _ukprn;
//        [SetUp]
//        public void Init()
//        {
//            _ukprn = 12345678;
//            _mockLogger = new Mock<ILogger<ProvidersController>>();
     

//            var _mockMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
//            _mockMessageHandler
//                .Protected()
//                // Setup the PROTECTED method to mock
//                .Setup<Task<HttpResponseMessage>>(
//                    "SendAsync",
//                    ItExpr.IsAny<HttpRequestMessage>(),
//                    ItExpr.IsAny<CancellationToken>()
//                )
//                // prepare the expected response of the mocked http call
//                .ReturnsAsync(new HttpResponseMessage()
//                {
//                    StatusCode = HttpStatusCode.OK,
//                    Content = new StringContent("[{'id':1,'value':'1'}]")
//                })
//                .Verifiable();
   
//            _clientLogger = new Mock<ILogger<RoatpApiClient>>();
//            var httpClient = new HttpClient(_mockMessageHandler.Object)
//            {
//                BaseAddress = new Uri("http://test.com/"),
//            };

//            _client = new RoatpApiClient(httpClient, _clientLogger.Object, null);

//            _mockEnv = new Mock<IHostingEnvironment>();
//            _mockRetryServiceLogger = new Mock<ILogger<RetryService>>();
//            _retryService = new RetryService(_mockRetryServiceLogger.Object);

//            _mapper = new RoatpMapper();
//            //_client.Setup(z => z.GetRoatpSummaryByUkprn(It.IsAny<int>())).ReturnsAsync((IEnumerable<RoatpResult>)null);
//            //_client.Setup(z => z.GetRoatpSummary()).ReturnsAsync((IEnumerable<RoatpResult>)null);

//            //var url = $"api/v1/download/roatp-summary/{ukprn}";
//            //_logger.LogInformation($"Retrieving RoATP summary data from {url}");
//            //return await Get<IEnumerable<RoatpResult>>($"{url}");

//            HttpContextRequest = new Mock<HttpRequest>();
//            HttpContextRequest.Setup(r => r.Method).Returns("GET");
//            HttpContext = new Mock<HttpContext>();
//            HttpContext.Setup(x => x.Request.Scheme).Returns("http");
//            HttpContext.Setup(x => x.Request.Host).Returns(new HostString("localhost"));

//            _controller = new ProvidersController(_mockLogger.Object, _client, _mapper,
//               _mockEnv.Object, _retryService);

//            _controller.ControllerContext = new ControllerContext {HttpContext = HttpContext.Object};

//        }

//        [TestCase(1)]
//        [TestCase(12)]
//        [TestCase(123)]
//        [TestCase(1234)]
//        [TestCase(12345)]
//        [TestCase(123456)]
//        [TestCase(1234567)]
//        [TestCase(123456789)]
//        public void ShouldThrowBadRequestIfUkprnIsInvalid(int ukprn)
//        {
//            var res = _controller.Get(ukprn).Result;
//            Assert.AreEqual((int)HttpStatusCode.BadRequest, ((ObjectResult)res).StatusCode);
//        }

//        //[TestCase("presentUkprn", "today", (int) HttpStatusCode.OK)]
//        //[TestCase("presentUkprn", "tomorrow", (int) HttpStatusCode.NotFound)]
//        //[TestCase("presentUkprn", "yesterday", (int) HttpStatusCode.OK)]
//        [TestCase("absentUkprn", "today", (int)HttpStatusCode.NotFound)]
//        [TestCase("absentUkprn", "tomorrow", (int)HttpStatusCode.NotFound)]
//        [TestCase("absentUkprn", "yesterday", (int)HttpStatusCode.NotFound)]

//        public void ShouldThrowNotFoundIfUkprnNotMatchedOrStartDateNotTodayOrAfter(string ukprnType, string startDate,
//            int httpStatusCode)
//        {

//            DateTime startDateToUse;
//            switch (startDate)
//            {
//                case "tomorrow":
//                    startDateToUse = DateTime.Today.AddDays(1);
//                    break;
//                case "yesterday":
//                    startDateToUse = DateTime.Today.AddDays(-1);
//                    break;
//                default:
//                    startDateToUse = DateTime.Today;
//                    break;
//            }


//            var roatpResult = new RoatpResult
//            {
//                Ukprn = _ukprn.ToString(),
//                StartDate = startDateToUse
//            };

//            if (ukprnType != "absentUkprn")
//            {
//                //        _client.Setup(z => z.GetRoatpSummaryByUkprn(_ukprn)).ReturnsAsync(new List<RoatpResult> {roatpResult});
//                _mockMessageHandler
//                    .Protected()
//                    // Setup the PROTECTED method to mock
//                    .Setup<Task<HttpResponseMessage>>(
//                        "SendAsync",
//                        ItExpr.IsAny<HttpRequestMessage>(),
//                        ItExpr.IsAny<CancellationToken>()
//                    )
//                    // prepare the expected response of the mocked http call
//                    .ReturnsAsync(new HttpResponseMessage()
//                    {
//                        StatusCode = HttpStatusCode.OK,
//                        Content = new StringContent(JsonConvert.SerializeObject(new List<RoatpResult> {roatpResult}))
//                    })
//                    .Verifiable();
//            }
//            //else
//            //{
//            //    _mockMessageHandler
//            //        .Protected()
//            //        // Setup the PROTECTED method to mock
//            //        .Setup<Task<HttpResponseMessage>>(
//            //            "SendAsync",
//            //            ItExpr.IsAny<HttpRequestMessage>(),
//            //            ItExpr.IsAny<CancellationToken>()
//            //        )
//            //        // prepare the expected response of the mocked http call
//            //        .ReturnsAsync(new HttpResponseMessage()
//            //        {
//            //            StatusCode = HttpStatusCode.OK,
//            //            Content = new StringContent(JsonConvert.SerializeObject(string.Empty))
//            //        })
//            //        .Verifiable();
//            //}

        

//        var resultFromGet = _controller.Get(_ukprn).Result;

//            Assert.IsTrue(((ObjectResult)resultFromGet).StatusCode == httpStatusCode);

//            if (httpStatusCode == (int)HttpStatusCode.OK)
//            {
//                var returnedProvider = (Provider)((ObjectResult)resultFromGet).Value;
//                Assert.AreEqual(_ukprn, returnedProvider.Ukprn);
//            }
//            else
//            {
//                var returnedDetails = (string)((ObjectResult)resultFromGet).Value;
//                Assert.IsNotEmpty(returnedDetails);
//            }
//        }


//        //[TestCase("presentUkprn", "today", (int)HttpStatusCode.NoContent)]
//        //[TestCase("presentUkprn", "tomorrow", (int)HttpStatusCode.NotFound)]
//        //[TestCase("presentUkprn", "yesterday", (int)HttpStatusCode.NoContent)]
//        //[TestCase("absentUkprn", "today", (int)HttpStatusCode.NotFound)]
//        //[TestCase("absentUkprn", "tomorrow", (int)HttpStatusCode.NotFound)]
//        //[TestCase("absentUkprn", "yesterday", (int)HttpStatusCode.NotFound)]

//        //public void ShouldThrowNotFoundIfUkprnNotMatchedOrStartDateNotTodayOrAfterForHead(string ukprnType, string startDate, int httpStatusCode)
//        //{

//        //    DateTime startDateToUse;
//        //    switch (startDate)
//        //    {
//        //        case "tomorrow":
//        //            startDateToUse = DateTime.Today.AddDays(1);
//        //            break;
//        //        case "yesterday":
//        //            startDateToUse = DateTime.Today.AddDays(-1);
//        //            break;
//        //        default:
//        //            startDateToUse = DateTime.Today;
//        //            break;
//        //    }


//        //    var roatpResult = new RoatpResult
//        //    {
//        //        Ukprn = _ukprn.ToString(),
//        //        StartDate = startDateToUse
//        //    };

//        //    if (ukprnType != "absentUkprn")
//        //        _client.Setup(z => z.GetRoatpSummaryByUkprn(_ukprn)).ReturnsAsync(new List<RoatpResult> { roatpResult });

//        //    var resultFromGet = _controller.Head(_ukprn).Result;

//        //    if (httpStatusCode == (int)HttpStatusCode.NoContent)
//        //    {
//        //        Assert.IsTrue(((NoContentResult)resultFromGet).StatusCode == httpStatusCode);
//        //    }
//        //    else
//        //    {
//        //        Assert.IsTrue(((ObjectResult)resultFromGet).StatusCode == httpStatusCode);
//        //    }

//        //    if (httpStatusCode == (int)HttpStatusCode.OK)
//        //    {
//        //        var returnedProvider = (Provider)((ObjectResult)resultFromGet).Value;
//        //        Assert.AreEqual(_ukprn, returnedProvider.Ukprn);
//        //    }
//        //    else if (httpStatusCode != (int)HttpStatusCode.NoContent)
//        //    {
//        //        var returnedDetails = (string)((ObjectResult)resultFromGet).Value;
//        //        Assert.IsNotEmpty(returnedDetails);
//        //    }
//        //}

        //[TestCase("today", 2)]
        //[TestCase("tomorrow", 1)]
        //[TestCase("yesterday", 2)]
        //public void ShouldReturnExpectedNumberOfRecordsFromGetAll(string startDate, int expectedCount)
        //{
        //    var roatpResults = new List<RoatpResult>
        //    {
        //        new RoatpResult {Ukprn = "11111111", StartDate = DateTime.Today}
        //    };

        //    DateTime startDateToUse;
        //    switch (startDate)
        //    {
        //        case "tomorrow":
        //            startDateToUse = DateTime.Today.AddDays(1);
        //            break;
        //        case "yesterday":
        //            startDateToUse = DateTime.Today.AddDays(-1);
        //            break;
        //        default:
        //            startDateToUse = DateTime.Today;
        //            break;
        //    }

        //    var roatpResult2 = new RoatpResult { Ukprn = "22222222", StartDate = startDateToUse };

        //    roatpResults.Add(roatpResult2);

        //    _client.Setup(z => z.GetRoatpSummary()).ReturnsAsync(roatpResults);

        //    var resultsFromGet = _controller.GetAll().Result;
        //    var expectedProviders = (List<Provider>)((ObjectResult)resultsFromGet).Value;

        //    Assert.AreEqual(expectedCount, expectedProviders.Count);
        //}
//    }
//}
