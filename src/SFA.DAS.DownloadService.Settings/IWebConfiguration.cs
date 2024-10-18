using Newtonsoft.Json;

namespace SFA.DAS.DownloadService.Settings
{
    public interface IWebConfiguration
    {
        ApiAuthentication ApiAuthentication { get; set; }
        ClientApiAuthentication RoatpApiAuthentication { get; set; }
        ManagedIdentityApiAuthentication DownloadServiceApiAuthentication { get; set; }
        string RedisConnectionString { get; set; }
        string DataProtectionKeysDatabase { get; set; }
    }

    public class WebConfiguration : IWebConfiguration
    {
        [JsonRequired] public ApiAuthentication ApiAuthentication { get; set; }
        [JsonRequired] public ClientApiAuthentication RoatpApiAuthentication { get; set; }
        [JsonRequired] public ManagedIdentityApiAuthentication DownloadServiceApiAuthentication { get; set; }
        [JsonRequired] public string RedisConnectionString { get; set; }
        [JsonRequired] public string DataProtectionKeysDatabase { get; set; }
    }
}
