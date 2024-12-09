using SFA.DAS.DownloadService.Api.Types;
using SFA.DAS.DownloadService.Api.Types.Roatp;
using System;
using System.Collections.Generic;

namespace SFA.DAS.DownloadService.Services.Interfaces
{
    public interface IAparMapper
    {
        AparEntry Map(RoatpResult roatpResult, Func<long, string> uriResolver);
        List<AparEntry> Map(List<RoatpResult> roatpResults, Func<long, string> uriResolver);

        CsvAparEntry MapCsv(AparEntry aparEntry);
        List<CsvAparEntry> MapCsv(List<AparEntry> aparEntries);
    }
}
