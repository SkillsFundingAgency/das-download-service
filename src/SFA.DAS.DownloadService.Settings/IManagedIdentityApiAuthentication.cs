namespace SFA.DAS.DownloadService.Settings
{
    public interface IManagedIdentityApiAuthentication
    {
        string Identifier { get; set; }
        string ApiBaseAddress { get; set; }
    }
}
