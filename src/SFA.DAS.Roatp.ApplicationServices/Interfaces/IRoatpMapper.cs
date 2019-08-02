using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.DownloadService.Api.Types.Roatp;
using SFA.DAS.Roatp.ApplicationServices.Models;

namespace SFA.DAS.Roatp.ApplicationServices.Interfaces
{
    public interface IRoatpMapper
    {
        Provider Map(RoatpResult roatpResult);
        List<Provider> Map(List<RoatpResult> roatpResults);

        CsvProvider MapCsv(RoatpResult results);
        List<CsvProvider> MapCsv(List<RoatpResult> roatpResults);

    }
}
