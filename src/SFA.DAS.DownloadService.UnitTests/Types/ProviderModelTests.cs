using System;
using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.DownloadService.Api.Types;
using SFA.DAS.DownloadService.Api.Types.Roatp.Common;
using SFA.DAS.DownloadService.Api.Types.Roatp.Models;

namespace SFA.DAS.DownloadService.UnitTests.Types;

public class ProviderModelTests
{
    [Test]
    public void ProviderModel_MapsOrganisationDataCorrectly()
    {
        // Arrange
        var model = new OrganisationModel
        {
            OrganisationId = Guid.NewGuid(),
            Ukprn = 12345678,
            LegalName = "TestLegalName",
            TradingName = "TestTradingName",
            CompanyNumber = "12345",
            CharityNumber = "12345",
            ProviderType = DownloadService.Api.Types.Roatp.Common.ProviderType.Employer,
            OrganisationTypeId = 1,
            OrganisationType = "TestOrganisationType",
            Status = OrganisationStatus.Active,
            ApplicationDeterminedDate = DateTime.UtcNow.AddMonths(-1),
            RemovedReasonId = 1,
            RemovedReason = "TestRemovedReason",
            StartDate = DateTime.UtcNow,
            LastUpdatedDate = DateTime.UtcNow.AddDays(-1),
            AllowedCourseTypes = new List<AllowedCourseType>
                { new AllowedCourseType(1, "TestCourseTypeName")}
        };

        // Act
        ProviderModel result = model;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.AreEqual(model.Ukprn, result.Ukprn);
            Assert.AreEqual($"{model.LegalName} T/A {model.TradingName}", result.Name);
            Assert.AreEqual(DownloadService.Api.Types.ProviderType.EmployerProvider, result.ApplicationType);
            Assert.AreEqual(model.StartDate, result.StartDate);
            Assert.AreEqual(model.ApplicationDeterminedDate, result.ApplicationDeterminedDate);
            Assert.AreEqual(false, result.CurrentlyNotStartingNewApprentices);
        });
    }

    [Test]
    public void ProviderModel_TradingNameIsEmpty_MapsLeagalName()
    {
        // Arrange
        var model = new OrganisationModel
        {
            LegalName = "TestLegalName",
            TradingName = "",
        };

        // Act
        ProviderModel result = model;

        // Assert
        Assert.AreEqual(model.LegalName, result.Name);
    }

    [Test]
    public void ProviderModel_OrganisationStatusIsActiveNoStarts_MapsCurrentlyNotStartingNewApprenticesAsTrue()
    {
        // Arrange
        var model = new OrganisationModel
        {
            Status = OrganisationStatus.ActiveNoStarts,
        };

        // Act
        ProviderModel result = model;

        // Assert
        Assert.AreEqual(true, result.CurrentlyNotStartingNewApprentices);
    }

    [TestCase(1, true, false, TestName = "CanDeliverApprenticeships is True and CanDeliverApprenticeshipUnits is False")]
    [TestCase(2, false, true, TestName = "CanDeliverApprenticeships is False and CanDeliverApprenticeshipUnits is True")]
    [TestCase(3, false, false, TestName = "CanDeliverApprenticeships is False and CanDeliverApprenticeshipUnits is False")]
    public void ProviderModel_MapsCourseTypeDeliveryCorrectly(int courseTypeId, bool canDeliverApprenticeships, bool canDeliverApprenticeshipUnits)
    {
        // Arrange
        var model = new OrganisationModel
        {
            AllowedCourseTypes = new List<AllowedCourseType>
                { new AllowedCourseType(courseTypeId, "TestCourseTypeName")}
        };

        // Act
        ProviderModel result = model;

        // Assert
        Assert.AreEqual(canDeliverApprenticeships, result.CanDeliverApprenticeships);
        Assert.AreEqual(canDeliverApprenticeshipUnits, result.CanDeliverApprenticeshipUnits);
    }
}