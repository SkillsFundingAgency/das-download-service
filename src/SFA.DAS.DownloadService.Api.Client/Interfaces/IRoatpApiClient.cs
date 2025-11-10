using Refit;
using SFA.DAS.DownloadService.Api.Types.Roatp.Models;
using SFA.DAS.DownloadService.Api.Types.Roatp.Responses;
using System.Threading.Tasks;

namespace SFA.DAS.DownloadService.Api.Client.Interfaces
{
    public interface IRoatpApiClient
    {
        [Get("/organisations")]
        Task<GetOrganisationResponse> GetRoatpSummary();

        [Get("/organisations/{ukprn}")]
        Task<OrganisationModel> GetRoatpSummaryByUkprn([AliasAs("ukprn")] int ukprn);
    }
}