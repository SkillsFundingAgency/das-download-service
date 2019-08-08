using SFA.DAS.DownloadService.Api.Types.Roatp;
using System.Collections.Generic;

namespace SFA.DAS.DownloadService.Services.Interfaces
{
    public interface IRoatpMapper
    {
        Provider Map(RoatpResult roatpResult);
        List<Provider> Map(List<RoatpResult> roatpResults);

        CsvProvider MapCsv(RoatpResult results);
        List<CsvProvider> MapCsv(List<RoatpResult> roatpResults);

    }
}
