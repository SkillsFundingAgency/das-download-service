using SFA.DAS.DownloadService.Api.Types.Roatp.Common;
using SFA.DAS.DownloadService.Api.Types.Roatp.Models;
using System;
using System.ComponentModel;

namespace SFA.DAS.DownloadService.Api.Types;

[DisplayName("Provider")]
public class ProviderModel
{
    public int Ukprn { get; set; }
    public string Name { get; set; }
    public string Uri { get; set; }
    public ProviderType ApplicationType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? ApplicationDeterminedDate { get; set; }
    public bool? CurrentlyNotStartingNewApprentices { get; set; }

    public static implicit operator ProviderModel(OrganisationModel source) =>
        new()
        {
            Ukprn = source.Ukprn,
            Name = string.IsNullOrWhiteSpace(source.TradingName) ? source.LegalName : $"{source.LegalName} T/A {source.TradingName}",
            ApplicationType = (ProviderType)(int)source.ProviderType,
            StartDate = source.StartDate,
            ApplicationDeterminedDate = source.ApplicationDeterminedDate,
            CurrentlyNotStartingNewApprentices = source.Status == OrganisationStatus.ActiveNoStarts
        };
}