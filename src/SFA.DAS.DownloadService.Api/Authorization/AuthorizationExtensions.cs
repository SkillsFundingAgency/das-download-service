using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.DownloadService.Api.Authorization
{
    public static class AuthorizationExtensions
    {
        public static IServiceCollection AddApiAuthorization(this IServiceCollection services, IHostingEnvironment environment)
        {
            if (!environment.IsDevelopment())
            {
                services.AddAuthorization(x =>
                {
                    x.AddPolicy("Default", policy =>
                    {
                        policy.RequireAuthenticatedUser();
                        policy.RequireRole("Default");
                    });

                    x.DefaultPolicy = x.GetPolicy("Default");
                });
            }

            return services;
        }
    }
}