﻿using Newtonsoft.Json;

namespace SFA.DAS.DownloadService.Settings
{
    public interface IWebConfiguration
    {
        string RoatpApiClientBaseUrl { get; set; }
        string DownloadServiceApiClientBaseUrl { get; set; }
        ClientApiAuthentication RoatpApiAuthentication { get; set; }
    }

    public class WebConfiguration : IWebConfiguration
    {
        [JsonRequired] public string RoatpApiClientBaseUrl { get; set; }
        [JsonRequired] public ClientApiAuthentication RoatpApiAuthentication { get; set; }

        [JsonRequired] public string DownloadServiceApiClientBaseUrl { get; set; }
    }
}
