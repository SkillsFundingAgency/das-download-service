using System;

namespace SFA.DAS.DownloadService.Api.Types.Assessor
{
    public class EpaoResult
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Ukprn { get; set; }
        public DateTime EarliestDateStandardApprovedOnRegister { get; set; }
        public DateTime EarliestEffectiveFromDate { get; set; }
    }
}
