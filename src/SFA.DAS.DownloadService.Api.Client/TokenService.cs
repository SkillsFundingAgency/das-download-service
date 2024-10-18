using Azure.Core;
using Azure.Identity;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using SFA.DAS.DownloadService.Settings;
using SFA.DAS.DownloadService.Api.Client.Interfaces;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.DownloadService.Api.Client
{
    public class TokenService : IRoatpTokenService, IDownloadServiceTokenService
    {
        private readonly IManagedIdentityApiAuthentication _apiAuthentication;

        public TokenService(IClientApiAuthentication apiAuthentication)
        {
            _apiAuthentication = apiAuthentication;
        }

        public TokenService(IManagedIdentityApiAuthentication apiAuthentication)
        {
            _apiAuthentication = apiAuthentication;
        }

        public async Task<string> GetTokenAsync()
        {
            Uri uri = new Uri(_apiAuthentication.ApiBaseAddress);
            if (uri.Host == "localhost" || uri.Host == "127.0.0.1" || uri.Host == "::1")
                return string.Empty;
            
            if(_apiAuthentication is IClientApiAuthentication clientApiAuthentication)
            {
                var authority = $"{clientApiAuthentication.Instance}/{clientApiAuthentication.TenantId}";
                var clientCredential = new ClientCredential(clientApiAuthentication.ClientId, clientApiAuthentication.ClientSecret);
                var context = new AuthenticationContext(authority, true);
                var result = await context.AcquireTokenAsync(clientApiAuthentication.Identifier, clientCredential);

                return result.AccessToken;
            }
            else
            {
                var defaultAzureCredential = new DefaultAzureCredential();
                var result = await defaultAzureCredential.GetTokenAsync(
                    new TokenRequestContext(scopes: new string[] { _apiAuthentication.Identifier + "/.default" }) { });

                return result.Token;
            }
        }
    }
}
