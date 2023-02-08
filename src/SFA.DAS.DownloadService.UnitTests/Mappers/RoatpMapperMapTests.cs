using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.DownloadService.Api.Types.Roatp;
using SFA.DAS.DownloadService.Services.Services.Roatp;

namespace SFA.DAS.DownloadService.UnitTests.Mappers
{
    [TestFixture]
    public class RoatpMapperMapTests
    {
        private RoatpMapper _mapper;
        [SetUp]
        public void Init()
        {
            _mapper = new RoatpMapper();
        }

        [Test]
        public void ShouldMapRoatpResultWithoutUkprnToNull()
        {
            var roatpResult = new RoatpResult();
            var mappedResult = _mapper.Map(roatpResult);
            Assert.IsNull(mappedResult);
        }


        [Test]
        public void ShouldMapRoatpResultsToProvidersAsAList()
        {
            var ukprn1 = "22345678";
            var ukprn2 = "87654321";

            var roatpResultstoMap = new List<RoatpResult>
            {
                new RoatpResult
                {
                    Ukprn = ukprn1
                },
                new RoatpResult
                {
                    Ukprn = ukprn2
                }
            };


            var mappedResults = _mapper.Map(roatpResultstoMap);
            Assert.AreEqual(mappedResults.Count,roatpResultstoMap.Count);
            Assert.IsTrue(mappedResults.Select(x => x.Ukprn.ToString() == ukprn1).Any());
            Assert.IsTrue(mappedResults.Select(x => x.Ukprn.ToString() == ukprn2).Any());
        }

        [TestCase(12345678)]
        [TestCase(1)]
        public void ShouldMapRoatpResultUkprnToProviderUkprn(long ukprn)
        {
            var roatpResult = new RoatpResult();
            roatpResult.Ukprn = ukprn.ToString();
            var mappedResult = _mapper.Map(roatpResult);
            Assert.AreEqual(roatpResult.Ukprn,mappedResult.Ukprn.ToString());         
        }

        [TestCase("org name")]
        [TestCase("org name 2")]
        [TestCase("")]
        [TestCase(null)]
        public void ShouldMapRoatpResultOrganisationNameToProviderName(string organisationName)
        {
            var roatpResult = new RoatpResult();
            roatpResult.Ukprn = "12345678";
            roatpResult.OrganisationName = organisationName;
            var mappedResult = _mapper.Map(roatpResult);
            Assert.AreEqual(roatpResult.OrganisationName, mappedResult.Name);   
        }

        [TestCase(null, ProviderType.Unknown)]
        [TestCase("test words", ProviderType.Unknown)]
        [TestCase("mainprovider", ProviderType.Unknown)]
        [TestCase("main provider", ProviderType.MainProvider)]
        [TestCase("MAIN Provider", ProviderType.MainProvider)]
        [TestCase("employer provider", ProviderType.EmployerProvider)]
        [TestCase("EMPLOYER provider", ProviderType.EmployerProvider)]
        [TestCase("supporting provider", ProviderType.SupportingProvider)]
        [TestCase("SUpporting Provider", ProviderType.SupportingProvider)]
        public void ShouldMapRoatpResultProviderTypeToProviderProviderType(string providerType, ProviderType expectedProviderType)
        {
            var roatpResult = new RoatpResult();
            roatpResult.Ukprn = "12345678";
            roatpResult.OrganisationName = "org name";
            roatpResult.ApplicationType = providerType;
            var mappedResult = _mapper.Map(roatpResult);
            Assert.AreEqual(((ProviderType)expectedProviderType).ToString(), mappedResult.ApplicationType.ToString());
        }

        [TestCase(null, null)]
        [TestCase("2019-08-05", "2019-08-05")]
        public void ShouldMapRoatpResultStartDateToProviderStartDate(DateTime? startDate, DateTime? expectedMapping)
        {
            var roatpResult = new RoatpResult();
            roatpResult.Ukprn = "12345678";
            roatpResult.OrganisationName = "org name";
            roatpResult.ApplicationType = "main provider";
            roatpResult.StartDate = startDate;
            var mappedResult = _mapper.Map(roatpResult);
            Assert.AreEqual(expectedMapping, mappedResult.StartDate);
        }


        [TestCase(null, null)]
        [TestCase("2019-08-05", "2019-08-05")]
        public void ShouldMapRoatpResultApplicationDeterminedDateToProviderApplicationDate(DateTime? applicationDeterminedDate, DateTime? expectedMapping)
        {
            var roatpResult = new RoatpResult();
            roatpResult.Ukprn = "12345678";
            roatpResult.OrganisationName = "org name";
            roatpResult.ApplicationType = "main provider";
            roatpResult.StartDate = DateTime.Today;
            roatpResult.ApplicationDeterminedDate = applicationDeterminedDate;
            var mappedResult = _mapper.Map(roatpResult);
            Assert.AreEqual(expectedMapping, mappedResult.ApplicationDeterminedDate);
        }


        [TestCase(null, false)]
        [TestCase("2019-08-04", true)]
        public void ShouldMapRoatpResultCurrentlyNotStartingToProviderCurrentlyNotStarting(DateTime? providerNotCurrentlyStartingNewApprentices, bool expectedMapping)
        {
            var roatpResult = new RoatpResult();
            roatpResult.Ukprn = "12345678";
            roatpResult.OrganisationName = "org name";
            roatpResult.ApplicationType = "main provider";
            roatpResult.StartDate = DateTime.Today;
            roatpResult.ApplicationDeterminedDate = DateTime.Today;
            roatpResult.ProviderNotCurrentlyStartingNewApprentices = providerNotCurrentlyStartingNewApprentices;
            var mappedResult = _mapper.Map(roatpResult);
            Assert.AreEqual(expectedMapping, mappedResult.CurrentlyNotStartingNewApprentices);
        }
    }
}
