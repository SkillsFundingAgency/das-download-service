using SFA.DAS.DownloadService.Api.Types.Roatp.Models;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.DownloadService.Api.Types.Roatp.Responses;
public class GetOrganisationResponse
{
    public IEnumerable<OrganisationModel> Organisations { get; set; } = Enumerable.Empty<OrganisationModel>();
}