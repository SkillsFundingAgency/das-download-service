using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using SFA.DAS.DownloadService.Services.Interfaces;
using SFA.DAS.DownloadService.Services.Services;
using SFA.DAS.Roatp.Api.Client;
using SFA.DAS.Roatp.Api.Client.Interfaces;
using StructureMap;

namespace SFA.DAS.DownloadService.Infrastructure.DependencyResolution
{
    //public sealed class InfrastructureRegistry : Registry
    //{
    //    public InfrastructureRegistry()
    //    {
    //        For<IRoatpApiClient>().Use<RoatpApiClient>();
    //        For<IRoatpMapper>().Use<RoatpMapper>();
    //    }

    //    private IDictionary<string, object> GetProperties()
    //    {
    //        var properties = new Dictionary<string, object>();
    //        properties.Add("Version", GetVersion());
    //        return properties;
    //    }

    //    private string GetVersion()
    //    {
    //        Assembly assembly = Assembly.GetExecutingAssembly();
    //        FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
    //        return fileVersionInfo.ProductVersion;
    //    }
    //}
}
