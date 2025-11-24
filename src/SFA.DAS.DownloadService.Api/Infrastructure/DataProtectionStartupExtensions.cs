using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using System.Diagnostics.CodeAnalysis;


namespace SFA.DAS.DownloadService.Api.Infrastructure;

[ExcludeFromCodeCoverage]
public static class DataProtectionStartupExtensions
{
    public static IServiceCollection AddDataProtection(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        if (environment.IsDevelopment() || configuration == null)
        {
            return services;
        }

        var redisConnectionString = configuration.GetSection("RedisConnectionString").Get<string>();
        var dataProtectionKeysDatabase = configuration.GetSection("DataProtectionKeysDatabase").Get<string>();

        var redis = ConnectionMultiplexer
            .Connect($"{redisConnectionString},{dataProtectionKeysDatabase}");

        services.AddDataProtection()
            .SetApplicationName("das-download-service")
            .PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys");

        return services;
    }
}
