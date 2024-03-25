using Microsoft.Extensions.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;

namespace SFA.DAS.DownloadService.Settings
{
    public static class ConfigurationService
    {
        public static IConfigurationBuilder AddStorageConfiguration(this IConfigurationBuilder builder, IConfiguration configuration)
        {
            builder.AddAzureTableStorage(options =>
            {
                options.ConfigurationKeys = configuration["ConfigNames"].Split(",");
                options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
                options.EnvironmentName = configuration["EnvironmentName"];
                options.PreFixConfigurationKeys = false;
            });

            return builder;
        }
    }
}
