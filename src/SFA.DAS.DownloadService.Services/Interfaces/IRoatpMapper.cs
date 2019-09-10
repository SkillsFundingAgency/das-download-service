using SFA.DAS.DownloadService.Api.Types.Roatp;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.DownloadService.Services.Interfaces
{
    public interface IRoatpMapper
    {
        Provider Map(RoatpResult roatpResult);
        List<Provider> Map(List<RoatpResult> roatpResults);

        CsvProvider MapCsv(RoatpResult results);


        CsvProvider MapProviderToCsvProvider(Provider provider);
        List<CsvProvider> MapProvidersToCsvProviders(List<Provider> providers);

        List<CsvProvider> MapCsv(List<RoatpResult> roatpResults);

    }
}
