using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using SFA.DAS.DownloadService.Services.Interfaces;
using SFA.DAS.DownloadService.Settings;

namespace SFA.DAS.DownloadService.Services.Services
{
    public class TokenService : ITokenService
    {
        private readonly IWebConfiguration _configuration;
        private readonly IHostingEnvironment _hostingEnvironment;
        public TokenService(IWebConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        public string GetToken()
        {
            if (_hostingEnvironment.IsDevelopment())
                return string.Empty;

            var tenantId = _configuration.RoatpApiAuthentication.TenantId;
            var clientId = _configuration.RoatpApiAuthentication.ClientId; 
            var appKey = _configuration.RoatpApiAuthentication.ClientSecret; 
            var resourceId = _configuration.RoatpApiAuthentication.ResourceId; 

            var authority = $"https://login.microsoftonline.com/{tenantId}";
            var clientCredential = new ClientCredential(clientId, appKey);
            var context = new AuthenticationContext(authority, true);
            var result = context.AcquireTokenAsync(resourceId, clientCredential).Result;

            return result.AccessToken;
        }
    }
}
