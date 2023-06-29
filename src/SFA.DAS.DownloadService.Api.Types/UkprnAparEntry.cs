using System;

namespace SFA.DAS.DownloadService.Api.Types
{
    public class UkprnAparEntry : AparEntry
    {
        public Epao Epao { get; set; }

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

    public class Epao
    {
        /// <summary>
        /// The name of the epao
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// The date this epao started on the register
        /// </summary>
        public DateTime? StartDate { get; set; }

        public DateTime? ApplicationDeterminedDate { get; set; }
    }
}
