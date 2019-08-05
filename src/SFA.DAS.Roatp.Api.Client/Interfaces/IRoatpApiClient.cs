using System;
using System.Collections.Generic;
using SFA.DAS.DownloadService.Api.Types.Roatp;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Api.Client.Interfaces
{
    public interface IRoatpApiClient
    {
        Task<List<RoatpResult>> GetRoatpSummary();
        Task<RoatpResult> GetRoatpSummaryByUkprn(int ukprn);
        Task<DateTime?> GetLatestNonOnboardingOrganisationChangeDate();
        ///// <summary>
        ///// Get a provider details
        ///// GET /providers/{ukprn}
        ///// </summary>
        ///// <param name="ukprn">the provider ukprn this should be 8 numbers long</param>
        ///// <returns>a provider details based on ukprn</returns>
        ///// <exception cref="EntityNotFoundException">when the resource you requested doesn't exist</exception>
        ///// <exception cref="HttpRequestException">There was an unexpected error</exception>
        //Provider Get(string ukprn);

        ///// <summary>
        ///// Get a provider details
        ///// GET /providers/{ukprn}
        ///// </summary>
        ///// <param name="ukprn">the provider ukprn this should be 8 numbers long</param>
        ///// <returns></returns>
        ///// <exception cref="EntityNotFoundException">when the resource you requested doesn't exist</exception>
        ///// <exception cref="HttpRequestException">There was an unexpected error</exception>
        //Provider Get(long ukprn);

        ///// <summary>
        ///// Get a provider details
        ///// GET /providers/{ukprn}
        ///// </summary>
        ///// <param name="ukprn">the provider ukprn this should be 8 numbers long</param>
        ///// <returns></returns>
        ///// <exception cref="EntityNotFoundException">when the resource you requested doesn't exist</exception>
        ///// <exception cref="HttpRequestException">There was an unexpected error</exception>
        //Provider Get(int ukprn);
        ///// <summary>
        ///// Check if a provider exists
        ///// HEAD /providers/{ukprn}
        ///// </summary>
        ///// <param name="ukprn">the provider ukprn this should be 8 numbers long</param>
        ///// <returns>bool</returns>
        ///// <exception cref="EntityNotFoundException">when the resource you requested doesn't exist</exception>
        ///// <exception cref="HttpRequestException">There was an unexpected error</exception>
        //bool Exists(string ukprn);
        ///// <summary>
        ///// Check if a provider exists
        ///// HEAD /providers/{ukprn}
        ///// </summary>
        ///// <param name="ukprn">the provider ukprn this should be 8 numbers long</param>
        ///// <returns>bool</returns>
        ///// <exception cref="EntityNotFoundException">when the resource you requested doesn't exist</exception>
        ///// <exception cref="HttpRequestException">There was an unexpected error</exception>
        //bool Exists(int ukprn);
        ///// <summary>
        ///// Check if a provider exists
        ///// HEAD /providers/{ukprn}
        ///// </summary>
        ///// <param name="ukprn">the provider ukprn this should be 8 numbers long</param>
        ///// <returns>bool</returns>
        ///// <exception cref="EntityNotFoundException">when the resource you requested doesn't exist</exception>
        ///// <exception cref="HttpRequestException">There was an unexpected error</exception>
        //bool Exists(long ukprn);
        ///// <summary>
        ///// Get a list of active providers on ROATP
        ///// GET /providers
        ///// </summary>
        ///// <returns></returns>
        ///// <exception cref="EntityNotFoundException">when the resource you requested doesn't exist</exception>
        ///// <exception cref="HttpRequestException">There was an unexpected error</exception>
        //IEnumerable<Provider> FindAll();
    }
}