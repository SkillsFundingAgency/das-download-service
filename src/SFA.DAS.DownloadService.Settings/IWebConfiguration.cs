using Newtonsoft.Json;

namespace SFA.DAS.DownloadService.Settings
{
    public interface IWebConfiguration
    {
        ManagedIdentityApiAuthentication RoatpApiAuthentication { get; set; }
        string RedisConnectionString { get; set; }
        string DataProtectionKeysDatabase { get; set; }
    }

    public class WebConfiguration : IWebConfiguration
    {
        [JsonRequired] public ManagedIdentityApiAuthentication RoatpApiAuthentication { get; set; }
        [JsonRequired] public string RedisConnectionString { get; set; }
        [JsonRequired] public string DataProtectionKeysDatabase { get; set; }
    }
}
