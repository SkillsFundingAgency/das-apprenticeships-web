using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Apprenticeships.Web.Models;
using SFA.DAS.Apprenticeships.Web.Models.ChangeOfStartDate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Apprenticeships.Web.UnitTests.Models.ChangeOfStartDate;

public class ProviderPlannedEndDateModelTests
{

    [TestCase(true, true, false)]
    [TestCase(true, false, true)]
    [TestCase(false, true, true)]
    public void GetSuggestedEndDate_WhenDatePropertiesNull_ThenReturnsNull(bool hasOriginalStartDate, bool hasOriginalEndDate, bool hasnewStartDate)
    {
        // Arrange
        var model = new ProviderPlannedEndDateModel
        {
            OriginalApprenticeshipActualStartDate = hasOriginalStartDate ? new DateTime(2021, 1, 1) : null,
            OriginalPlannedEndDate = hasOriginalEndDate ? new DateTime(2022, 1, 1) : null,
            ApprenticeshipActualStartDate = hasnewStartDate ? new DateField(new DateTime(2021, 1, 2)) : null,
        };

        // Act
        var result = model.SuggestedEndDate;

        // Assert
        Assert.Null(result);
    }

    [Test]
    public void GetSuggestedEndDate_WhenAllValuesAreSet_ThenReturnsCorrectDate()
    {
        // Arrange
        var model = new ProviderPlannedEndDateModel
        {
            OriginalApprenticeshipActualStartDate = new DateTime(2021, 1, 1),
            OriginalPlannedEndDate = new DateTime(2022, 1, 1),
            ApprenticeshipActualStartDate = new DateField(new DateTime(2021, 1, 2))
        };

        // Act
        var result = model.SuggestedEndDate;

        // Assert
        result.Should().Be(new DateTime(2022, 1, 2));

    }
}
