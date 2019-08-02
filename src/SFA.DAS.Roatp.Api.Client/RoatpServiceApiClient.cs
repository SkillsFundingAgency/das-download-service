using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.DownloadService.Api.Types.Roatp;
using SFA.DAS.Roatp.Api.Client.Interfaces;

namespace SFA.DAS.Roatp.Api.Client
{
    public class RoatpServiceApiClient : IRoatpServiceApiClient//, ApiClientBase

    {
        // private readonly IConfigurationSettings _applicationSettings;

        //public RoatpServiceApiClient(IConfigurationSettings applicationSettings): base("https://roatp.apprenticeships.sfa.bis.gov.uk")
        //{
        //    _applicationSettings = applicationSettings;

        //    _httpClient.BaseAddress = new Uri(_applicationSettings.RoatpApiBaseUrl) ;
        //    // _applicationSettings = applicationSettings;
        //    //_applicationSettings = applicationSettings;
        //    // private ConfigurationSettings _applicationSettings;
        //}

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

        public async Task<DateTime?> GetLatestNonOnboardingOrganisationChangeDate()
        {
            return DateTime.Now;

            //using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/download/roatp-summary/most-recent"))
            //{
            //    request.Headers.Add("Accept", "application/json");

            //    using (var response = _httpClient.SendAsync(request))
            //    {
            //        var result = response.Result;
            //        switch (result.StatusCode)
            //        {
            //            case HttpStatusCode.OK:
            //                return JsonConvert.DeserializeObject<DateTime?>(result.Content.ReadAsStringAsync().Result,
            //                    _jsonSettings);
            //            case HttpStatusCode.NotFound:
            //                RaiseResponseError($"The most recent organisation update could not be found", request, result);
            //                break;
            //        }

            //        RaiseResponseError(request, result);
            //    }

            //    return null;
        }


    }
}
