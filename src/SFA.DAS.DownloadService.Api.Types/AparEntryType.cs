using System.ComponentModel;

namespace SFA.DAS.DownloadService.Api.Types
{
    public enum AparEntryType
    {
        [Description("Unknown")]
        Unknown = 0,
        [Description("Main provider")]
        MainProvider = 1,
        [Description("Supporting provider")]
        SupportingProvider = 2,
        [Description("Employer provider")]
        EmployerProvider = 3
    }
}
