using System;
using System.Collections.Generic;
using System.Text;
using SFA.DAS.DownloadService.Core.Configuration;

namespace SFA.DAS.DownloadService.Infrastructure.Settings
{
    public sealed class ApplicationSettings : IConfigurationSettings
    {
       // public string RoatpProviderIndexAlias => string.Empty; // CloudConfigurationManager.GetSetting("RoatpProviderIndexAlias");

        public string EnvironmentName => string.Empty; //CloudConfigurationManager.GetSetting("EnvironmentName");

        //?        public bool IgnoreSslCertificateEnabled => false; //CloudConfigurationManager.GetSetting("FeatureToggle.IgnoreSslCertificateFeature") == "true";

        public string RoatpApiBaseUrl => string.Empty; //CloudConfigurationManager.GetSetting("RoatpApiBaseUrl");


    }
}
