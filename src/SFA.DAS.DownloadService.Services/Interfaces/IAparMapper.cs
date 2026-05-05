using SFA.DAS.DownloadService.Api.Types;
using System.Collections.Generic;

namespace SFA.DAS.DownloadService.Services.Interfaces
{
    public interface IAparMapper
    {
        CsvAparEntry MapCsv(AparEntry aparEntry);
        List<CsvAparEntry> MapCsv(List<AparEntry> aparEntries);
    }
}
