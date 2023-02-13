using Newtonsoft.Json;

namespace SFA.DAS.DownloadService.Api.Types.Roatp
{
    public class CsvProvider
    {
        public long Ukprn { get; set; }

        public string Name { get; set; }

        [JsonProperty("Application Type")]
        public string ApplicationType { get; set; }

        [JsonProperty("Start Date")]
        public string StartDate { get; set; }

        [JsonProperty("Status")]
        public string Status { get; set; }

        [JsonProperty("Application Determined Date")]
        public string ApplicationDeterminedDate { get; set; }
    }
}
