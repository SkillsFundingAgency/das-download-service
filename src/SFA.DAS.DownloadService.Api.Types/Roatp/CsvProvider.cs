using Newtonsoft.Json;

namespace SFA.DAS.DownloadService.Api.Types.Roatp
{
    public class CsvProvider
    {
        public long Ukprn { get; set; }

        public string Name { get; set; }

        [JsonProperty("Application type")]
        public string ApplicationType { get; set; }

        [JsonProperty("Organisation type")]
        public string OrganisationType { get; set; }

        [JsonProperty("Start Date")]
        public string StartDate { get; set; }

        [JsonProperty("Status")]
        public string Status { get; set; }

        [JsonProperty("Application determined date")]
        public string ApplicationDeterminedDate { get; set; }
    }
}
