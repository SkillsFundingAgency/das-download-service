using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.DownloadService.Api.Types.Roatp;
using SFA.DAS.DownloadService.Api.Client;
using SFA.DAS.DownloadService.Settings;
using SFA.DAS.Roatp.Api.Client.Interfaces;
using SFA.DAS.Roatp.ApplicationServices.Interfaces;

namespace SFA.DAS.Roatp.Api.Client
{
    public class RoatpApiClient : IRoatpApiClient //ApiClientBase,
    {

        private readonly HttpClient _client;
        private readonly ILogger<RoatpApiClient> _logger;
        private readonly ITokenService _tokenService;
        private IWebConfiguration _configuration;
        private string _baseUrl;
        public RoatpApiClient( ILogger<RoatpApiClient> logger, ITokenService tokenService, IWebConfiguration configuration)
        {
            _logger = logger;
            _tokenService = tokenService;
            _configuration = configuration;
            _baseUrl = _configuration.RoatpApiClientBaseUrl;
            _client = new HttpClient { BaseAddress = new Uri($"{_baseUrl}") };
        }

        public async Task<List<RoatpResult>> GetRoatpSummary()
        {
            var providers = new List<RoatpResult>
            {
                new RoatpResult
                {
                    Ukprn = "10061462",
                    OrganisationName = "AAA TRAINING SOLUTIONS LIMITED",
                    ProviderType = "Main provider",
                    ParentCompanyGuarantee = "N",
                    NewOrganisationWithoutFinancialTrackRecord = "N",
                    StartDate = new DateTime(2017, 05, 17),
                    ApplicationDeterminedDate = null
                },
                new RoatpResult
                {
                    Ukprn = "10046498",
                    OrganisationName = "1ST CARE TRAINING LIMITED",
                    ProviderType = "Main provider",
                    ParentCompanyGuarantee = "N",
                    NewOrganisationWithoutFinancialTrackRecord = "N",
                    StartDate = new DateTime(2017, 03, 13),
                    ApplicationDeterminedDate = new DateTime(2019, 06, 01)
                }
                ,
                new RoatpResult
                {
                    Ukprn = "10001236",
                    OrganisationName = "CBT SOLUTIONS LIMITED. T/A ewfwer",
                    ProviderType = "Employer provider",
                    ParentCompanyGuarantee = "N",
                    NewOrganisationWithoutFinancialTrackRecord = "Y",
                    StartDate = new DateTime(2017, 03, 13),
                    ApplicationDeterminedDate = new DateTime(2019, 06, 01)
                }
            };




            return providers;
        }

        public async Task<RoatpResult> GetRoatpSummaryByUkprn(int ukprn)
        {
            if (ukprn.ToString() == "10061462")
               return new RoatpResult
            {
                Ukprn = "10061462",
                            OrganisationName = "AAA TRAINING SOLUTIONS LIMITED",
                            ProviderType = "Main provider",
                            ParentCompanyGuarantee = "N",
                            NewOrganisationWithoutFinancialTrackRecord = "N",
                            StartDate = new DateTime(2017, 05, 17),
                            ApplicationDeterminedDate = null
                        };


            if (ukprn.ToString() == "10046498")
                return new RoatpResult
                {
                    Ukprn = "10046498",
                    OrganisationName = "1ST CARE TRAINING LIMITED",
                    ProviderType = "Main provider",
                    ParentCompanyGuarantee = "N",
                    NewOrganisationWithoutFinancialTrackRecord = "N",
                    StartDate = new DateTime(2017, 03, 13),
                    ApplicationDeterminedDate = new DateTime(2019, 06, 01)
                };

            if (ukprn.ToString() == "10001236")
                return new RoatpResult
                {
                    Ukprn = "10001236",
                    OrganisationName = "CBT SOLUTIONS LIMITED. T/A ewfwer",
                    ProviderType = "Employer provider",
                    ParentCompanyGuarantee = "N",
                    NewOrganisationWithoutFinancialTrackRecord = "Y",
                    StartDate = new DateTime(2017, 03, 13),
                    ApplicationDeterminedDate = new DateTime(2019, 06, 01)
                };


            return (RoatpResult)null;
        }

        public async  Task<DateTime?> GetLatestNonOnboardingOrganisationChangeDate()
        {
            return DateTime.Now;
        }
        /// <summary>
        /// The constructor to optional set the api url for testing
        /// </summary>
        /// <param name="baseUri">ie: https://roatp.apprenticeships.sfa.bis.gov.uk</param>
        //[Obsolete("This is constructor used for testing upcoming versions of the API")]
        //public RoatpApiClient(string baseUri) : base(baseUri)
        //{
        //}

        ///// <summary>
        ///// The default constructor to connect to https://roatp.apprenticeships.sfa.bis.gov.uk
        ///// </summary>
        //public RoatpApiClient() : base("https://roatp.apprenticeships.sfa.bis.gov.uk")
        //{
        //}


        ///// <summary>
        ///// Get a provider details
        ///// GET /providers/{ukprn}
        ///// </summary>
        ///// <param name="ukprn">the provider ukprn this should be 8 numbers long</param>
        ///// <returns>a provider details based on ukprn</returns>
        ///// <exception cref="EntityNotFoundException">when the resource you requested doesn't exist</exception>
        ///// <exception cref="HttpRequestException">There was an unexpected error</exception>
        //public Provider Get(string ukprn)
        //{
        //    using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/providers/{ukprn}"))
        //    {
        //        request.Headers.Add("Accept", "application/json");

        //        using (var response = _httpClient.SendAsync(request))
        //        {
        //            var result = response.Result;
        //            if (result.StatusCode == HttpStatusCode.OK)
        //            {
        //                return JsonConvert.DeserializeObject<Provider>(result.Content.ReadAsStringAsync().Result,
        //                    _jsonSettings);
        //            }
        //            if (result.StatusCode == HttpStatusCode.NotFound)
        //            {
        //                RaiseResponseError($"The provider {ukprn} could not be found", request, result);
        //            }

        //            RaiseResponseError(request, result);
        //        }

        //        return null;
        //    }
        //}

        ///// <summary>
        ///// Get a provider details
        ///// GET /providers/{ukprn}
        ///// </summary>
        ///// <param name="ukprn">the provider ukprn this should be 8 numbers long</param>
        ///// <returns>a provider details based on ukprn</returns>
        ///// <exception cref="EntityNotFoundException">when the resource you requested doesn't exist</exception>
        ///// <exception cref="HttpRequestException">There was an unexpected error</exception>
        //public Provider Get(long ukprn)
        //{
        //    return Get(ukprn.ToString());
        //}

        ///// <summary>
        ///// Get a provider details
        ///// GET /providers/{ukprn}
        ///// </summary>
        ///// <param name="ukprn">the provider ukprn this should be 8 numbers long</param>
        ///// <returns>a provider details based on ukprn</returns>
        ///// <exception cref="EntityNotFoundException">when the resource you requested doesn't exist</exception>
        ///// <exception cref="HttpRequestException">There was an unexpected error</exception>
        //public Provider Get(int ukprn)
        //{
        //    return Get(ukprn.ToString());
        //}

        ///// <summary>
        ///// Check if a provider exists
        ///// HEAD /providers/{ukprn}
        ///// </summary>
        ///// <param name="ukprn">the provider ukprn this should be 8 numbers long</param>
        ///// <returns>bool</returns>
        ///// <exception cref="EntityNotFoundException">when the resource you requested doesn't exist</exception>
        ///// <exception cref="HttpRequestException">There was an unexpected error</exception>
        //public bool Exists(string ukprn)
        //{
        //    using (var request = new HttpRequestMessage(HttpMethod.Head, $"/api/providers/{ukprn}"))
        //    {
        //        request.Headers.Add("Accept", "application/json");

        //        using (var response = _httpClient.SendAsync(request))
        //        {
        //            var result = response.Result;
        //            if (result.StatusCode == HttpStatusCode.NoContent)
        //            {
        //                return true;
        //            }
        //            if (result.StatusCode == HttpStatusCode.NotFound)
        //            {
        //                return false;
        //            }

        //            RaiseResponseError("Unexpected exception", request, result);
        //        }

        //        return false;
        //    }
        //}

        ///// <summary>
        ///// Check if a provider exists
        ///// HEAD /providers/{ukprn}
        ///// </summary>
        ///// <param name="ukprn">the provider ukprn this should be 8 numbers long</param>
        ///// <returns>bool</returns>
        ///// <exception cref="EntityNotFoundException">when the resource you requested doesn't exist</exception>
        ///// <exception cref="HttpRequestException">There was an unexpected error</exception>
        //public bool Exists(int ukprn)
        //{
        //    return Exists(ukprn.ToString());
        //}


        ///// <summary>
        ///// Check if a provider exists
        ///// HEAD /providers/{ukprn}
        ///// </summary>
        ///// <param name="ukprn">the provider ukprn this should be 8 numbers long</param>
        ///// <returns>bool</returns>
        ///// <exception cref="EntityNotFoundException">when the resource you requested doesn't exist</exception>
        ///// <exception cref="HttpRequestException">There was an unexpected error</exception>
        //public bool Exists(long ukprn)
        //{
        //    return Exists(ukprn.ToString());
        //}

        ///// <summary>
        ///// Get a list of active providers on ROATP
        ///// GET /providers
        ///// </summary>
        ///// <returns>Active providers</returns>
        ///// <exception cref="EntityNotFoundException">when the resource you requested doesn't exist</exception>
        ///// <exception cref="HttpRequestException">There was an unexpected error</exception>
        //public IEnumerable<Provider> FindAll()
        //{
        //    using (var request = new HttpRequestMessage(HttpMethod.Get, "/api/providers"))
        //    {
        //        request.Headers.Add("Accept", "application/json");

        //        using (var response = _httpClient.SendAsync(request))
        //        {
        //            var result = response.Result;
        //            if (result.StatusCode == HttpStatusCode.OK)
        //            {
        //                return JsonConvert.DeserializeObject<IEnumerable<Provider>>(result.Content.ReadAsStringAsync().Result, _jsonSettings);
        //            }

        //            RaiseResponseError(request, result);
        //        }

        //        return null;
        //    }
        //}

    }
}
