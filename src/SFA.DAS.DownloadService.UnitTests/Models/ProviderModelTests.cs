using NUnit.Framework;
using SFA.DAS.DownloadService.Api.Types.Roatp.Common;
using SFA.DAS.DownloadService.Api.Types.Roatp.Models;
using System;
using System.Collections.Generic;

namespace SFA.DAS.DownloadService.UnitTests.Models;
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
            ProviderType = ProviderType.Employer,
            ProviderTypeName = "Employer provider",
            OrganisationTypeId = 1,
            OrganisationType = "TestOrganisationType",
            Status = OrganisationStatus.Active,
            ApplicationDeterminedDate = DateTime.UtcNow.AddMonths(-1),
            RemovedReasonId = 1,
            RemovedReason = "TestRemovedReason",
            StartDate = DateTime.UtcNow,
            LastUpdatedDate = DateTime.UtcNow.AddDays(-1),
            AllowedCourseTypes = new List<AllowedCourseType>
                { new AllowedCourseType(1, "TestCourseTypeName", LearningType.Standard)}
        };
        var expectedApplicationType = "EmployerProvider";

        // Act
        ProviderModel result = model;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.AreEqual(result.Ukprn, model.Ukprn);
            Assert.AreEqual(result.Name, model.LegalName + " T/A " + model.TradingName);
            Assert.AreEqual(result.ApplicationType, expectedApplicationType);
            Assert.AreEqual(result.StartDate, model.StartDate);
            Assert.AreEqual(result.ApplicationDeterminedDate, model.ApplicationDeterminedDate);
            Assert.AreEqual(result.CurrentlyNotStartingNewApprentices, false);
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
        Assert.AreEqual(result.Name, model.LegalName);
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
        Assert.AreEqual(result.CurrentlyNotStartingNewApprentices, true);
    }
}