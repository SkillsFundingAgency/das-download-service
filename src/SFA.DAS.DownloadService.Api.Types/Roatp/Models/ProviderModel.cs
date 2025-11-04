using SFA.DAS.DownloadService.Api.Types.Roatp.Common;
using System;
using System.Globalization;
using System.Linq;

namespace SFA.DAS.DownloadService.Api.Types.Roatp.Models;
public class ProviderModel
{
    public int Ukprn { get; set; }
    public string Name { get; set; }
    public string Uri { get; set; }
    public string ApplicationType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? ApplicationDeterminedDate { get; set; }
    public bool? CurrentlyNotStartingNewApprentices { get; set; }

    public static implicit operator ProviderModel(OrganisationModel source) =>
        new()
        {
            Ukprn = source.Ukprn,
            Name = string.IsNullOrWhiteSpace(source.TradingName) ? source.LegalName : source.LegalName + " T/A " + source.TradingName,
            ApplicationType = string.IsNullOrWhiteSpace(source.ProviderTypeName) ? string.Empty : string.Concat(source.ProviderTypeName
                .Split(['_', '-', ' '], StringSplitOptions.RemoveEmptyEntries)
                .Select(s => CultureInfo.InvariantCulture.TextInfo.ToTitleCase(s.ToLower()))),
            StartDate = source.StartDate,
            ApplicationDeterminedDate = source.ApplicationDeterminedDate,
            CurrentlyNotStartingNewApprentices = source.Status == OrganisationStatus.ActiveNoStarts ? true : false
        };
}