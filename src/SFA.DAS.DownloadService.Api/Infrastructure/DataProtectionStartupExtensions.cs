using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;


namespace SFA.DAS.DownloadService.Api.Infrastructure
{
    public static class DataProtectionStartupExtensions
    {
        public static IServiceCollection AddDataProtection(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
        {
            if (!environment.IsDevelopment())
            {
                if (configuration != null)
                {
                    var redisConnectionString = configuration.GetSection("RedisConnectionString").Get<string>();
                    var dataProtectionKeysDatabase = configuration.GetSection("DataProtectionKeysDatabase").Get<string>();

                    var redis = ConnectionMultiplexer
                        .Connect($"{redisConnectionString},{dataProtectionKeysDatabase}");

                    services.AddDataProtection()
                        .SetApplicationName("das-download-service")
                        .PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys");
                }
            }
            return services;
        }
    }
}
