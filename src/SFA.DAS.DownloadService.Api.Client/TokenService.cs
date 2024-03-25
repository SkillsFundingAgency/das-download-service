using System;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using SFA.DAS.DownloadService.Api.Client.Interfaces;
using SFA.DAS.DownloadService.Settings;

namespace SFA.DAS.DownloadService.Api.Client
{
    public class TokenService : IAssessorTokenService, IRoatpTokenService, IDownloadServiceTokenService
    {
        private readonly IManagedIdentityApiAuthentication _apiAuthentication;

        public TokenService(IManagedIdentityApiAuthentication apiAuthentication)
        {
            _apiAuthentication = apiAuthentication;
        }

        public async Task<string> GetTokenAsync()
        {
            Uri uri = new Uri(_apiAuthentication.ApiBaseAddress);
            if (uri.Host == "localhost" || uri.Host == "127.0.0.1" || uri.Host == "::1") return string.Empty;

            var defaultAzureCredential = new DefaultAzureCredential();
            var result = await defaultAzureCredential.GetTokenAsync(
                new TokenRequestContext(scopes: new string[] { _apiAuthentication.Identifier + "/.default" }) { });

            return result.Token;
        }
    }
}
