﻿using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SFA.DAS.DownloadService.Api.Types.Roatp
{
    public class Provider
    {
        /// <summary>
        /// United Kingdom provider reference number
        /// </summary>
        [JsonRequired]
        public long Ukprn { get; set; }
        /// <summary>
        /// The name of the provider
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The Uri to this resource
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// The type of provider
        /// </summary>
        [JsonRequired]
        [JsonConverter(typeof(StringEnumConverter))]
        public ProviderType ProviderType { get; set; }

        //public bool ContractedForNonLeviedEmployers { get; set; }

        /// <summary>
        /// if a parent company has issued a guarantee in support of this provider
        /// </summary>
        [JsonRequired]
        public bool ParentCompanyGuarantee { get; set; }

        /// <summary>
        /// If the organisation is new and has yet to file financials
        /// </summary>
        [JsonRequired]
        public bool NewOrganisationWithoutFinancialTrackRecord { get; set; }

        /// <summary>
        /// The date this provider started on the register
        /// </summary>
        public DateTime? StartDate { get; set; }

        public DateTime? ApplicationDeterminedDate { get; set; }

        /// <summary>
        /// Indicates if the provider is not starting new apprentices at the moment
        /// </summary>
        public bool CurrentlyNotStartingNewApprentices { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentDate">This is usually now</param>
        /// <returns>whether this resource is valid in the time period you specify</returns>
        public bool IsDateValid(DateTime currentDate)
        {
            return StartDate.HasValue && StartDate.Value.Date <= currentDate.Date;
        }
    }
}
