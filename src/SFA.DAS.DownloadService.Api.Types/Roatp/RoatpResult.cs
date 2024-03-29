﻿using System;
using Newtonsoft.Json;

namespace SFA.DAS.DownloadService.Api.Types.Roatp
{
    public class RoatpResult
    {
        public string Ukprn { get; set; }

        [JsonProperty("Organisation Name")]
        public string OrganisationName { get; set; }

        [JsonProperty("Application Type")]
        public string ApplicationType { get; set; }

        [JsonProperty("Contracted to deliver to non-levied employers")]
        public string ContractedToDeliverToNonLeviedEmployers { get; set; }

        [JsonProperty("Start Date")]
        public DateTime? StartDate { get; set; }

        [JsonProperty("End Date")]
        public DateTime? EndDate { get; set; }

        [JsonProperty("Provider not currently starting new apprentices")]
        public DateTime? ProviderNotCurrentlyStartingNewApprentices { get; set; }

        [JsonProperty("Application Determined Date")]
        public DateTime? ApplicationDeterminedDate { get; set; }

        public bool IsDateValid(DateTime currentDate)
        {
            if (StartDate == null)
            {
                return false;
            }

            if (StartDate.Value.Date <= currentDate.Date && currentDate.Date <= EndDate)
            {
                return true;
            }

            return StartDate.Value.Date <= currentDate.Date && EndDate == null;
        }
    }
}
