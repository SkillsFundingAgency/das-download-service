namespace SFA.DAS.DownloadService.Settings
{
    public interface IClientApiAuthentication : IManagedIdentityApiAuthentication
    {
        string Instance { get; set; }
        string TenantId { get; set; }
        string ClientId { get; set; }
        string ClientSecret { get; set; }
    }
}
