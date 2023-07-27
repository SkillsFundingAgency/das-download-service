using System.Threading.Tasks;

namespace SFA.DAS.DownloadService.Api.Client.Interfaces
{
    public interface ITokenService
    {
        Task<string> GetTokenAsync();
    }
}
