using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using SFA.DAS.DownloadService.Infrastructure.Settings;
using SFA.DAS.Roatp.Api.Client;
using SFA.DAS.Roatp.Api.Client.Interfaces;
using SFA.DAS.Roatp.ApplicationServices.Interfaces;
using SFA.DAS.Roatp.ApplicationServices.Services;
using StructureMap;

namespace SFA.DAS.DownloadService.Infrastructure.DependencyResolution
{
    public sealed class InfrastructureRegistry : Registry
    {
        public InfrastructureRegistry()
        {
            //For<ILog>().Use(x => new NLogLogger(
            //    x.ParentType,
            //    x.GetInstance<IRequestContext>(),
            //    GetProperties())).AlwaysUnique();
            // For<IConfigurationSettings>().Use<ApplicationSettings>();
            //For<IGetProviders>().Use<ProviderRepository>();

            For<IRoatpApiClient>().Use<RoatpApiClient>();
            For<IRoatpMapper>().Use<RoatpMapper>();

            //For<IRoatpApiClient>().Use<RoatpApiClient>();  // http://localhost:37951/

        }

        private IDictionary<string, object> GetProperties()
        {
            var properties = new Dictionary<string, object>();
            properties.Add("Version", GetVersion());
            return properties;
        }

        private string GetVersion()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fileVersionInfo.ProductVersion;
        }
    }
}
