using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Api.Client.Interfaces
{
    public interface ITokenService
    {
        Task<string> GetTokenAsync();
    }
}
