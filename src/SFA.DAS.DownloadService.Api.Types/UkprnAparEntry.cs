using System;
using System.ComponentModel;

namespace SFA.DAS.DownloadService.Api.Types
{
    [DisplayName("UkprnProvider")]
    public class UkprnAparEntry : AparEntry
    {
        public static UkprnAparEntry FromAparEntry(AparEntry aparEntry)
        {
            return new UkprnAparEntry 
            { 
                Ukprn = aparEntry.Ukprn,
                Name = aparEntry.Name,
                Uri = aparEntry.Uri,
                ApplicationType = aparEntry.ApplicationType,
                StartDate = aparEntry.StartDate,
                ApplicationDeterminedDate = aparEntry.ApplicationDeterminedDate,
                CurrentlyNotStartingNewApprentices = aparEntry.CurrentlyNotStartingNewApprentices
            };
        }
    }
}
