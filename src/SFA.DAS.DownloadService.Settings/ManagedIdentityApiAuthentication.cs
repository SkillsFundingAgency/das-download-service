using Newtonsoft.Json;

namespace SFA.DAS.DownloadService.Settings
{
    public class ManagedIdentityApiAuthentication : IManagedIdentityApiAuthentication
    {
        [JsonRequired] public string Identifier { get; set; }

        [JsonRequired] public string ApiBaseAddress { get; set; }
    }
}
