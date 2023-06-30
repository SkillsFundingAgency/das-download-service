using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.DownloadService.Settings;
using System.Collections.Generic;

namespace SFA.DAS.DownloadService.Api.Authentication
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddApiAuthentication(this IServiceCollection services, ApiAuthentication apiAuthentication)
        {
            services.AddAuthentication(o =>
            {
                o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(o =>
                {
                    var validAudiences = new List<string>();
                    validAudiences.AddRange(apiAuthentication.Audience.Split(","));

                    o.Authority = $"https://login.microsoftonline.com/{apiAuthentication.TenantId}";
                    o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
                        ValidAudiences = validAudiences
                    };
                });

            return services;
        }
    }
}
