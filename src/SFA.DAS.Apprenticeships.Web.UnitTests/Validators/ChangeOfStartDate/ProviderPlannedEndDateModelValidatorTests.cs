using FluentAssertions;
using SFA.DAS.Apprenticeships.Web.Models;
using SFA.DAS.Apprenticeships.Web.Models.ChangeOfStartDate;
using SFA.DAS.Apprenticeships.Web.Validators.ChangeOfStartDate;

namespace SFA.DAS.Apprenticeships.Web.UnitTests.Validators.ChangeOfStartDate;


[TestFixture]
public class ProviderPlannedEndDateModelValidatorTests
{
    private ProviderPlannedEndDateModelValidator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new ProviderPlannedEndDateModelValidator();
    }

    [Test]
    public void Validate_WhenPlannedEndDateNull_And_UsingSuggestedDate_PassesValidation()
    {
        //  Arrange
        var model = new ProviderPlannedEndDateModel
        {
            UseSuggestedDate = true,
            PlannedEndDate = null,
            ApprenticeshipActualStartDate = new DateField(DateTime.Now)
        };

        //  Act
        var validationResult = _validator.Validate(model);

        //  Assert
        validationResult.IsValid.Should().BeTrue();
    }

    [Test]
    public void Validate_WhenPlannedEndDateNull_FailsValidation()
    {
        //  Arrange
        var model = new ProviderPlannedEndDateModel
        {
            UseSuggestedDate = false,
            PlannedEndDate = null,
            ApprenticeshipActualStartDate = new DateField(DateTime.Now)
        };

        //  Act
        var validationResult = _validator.Validate(model);

        //  Assert
        validationResult.IsValid.Should().BeFalse();
    }

    [Test]
    public void Validate_WhenPlannedEndIsLessThanAYearFromStartDate_FailsValidation()
    {
        //  Arrange
        var startDate = DateTime.Now;
        var endDate = startDate.AddDays(364);

        var model = new ProviderPlannedEndDateModel
        {
            UseSuggestedDate = false,
            PlannedEndDate = new DateField(endDate),
            ApprenticeshipActualStartDate = new DateField(startDate)
        };

        //  Act
        var validationResult = _validator.Validate(model);

        //  Assert
        validationResult.IsValid.Should().BeFalse();
    }

    [Test]
    public void Validate_WhenPlannedEndIsMoreThanAYearFromStartDate_PassesValidation()
    {
        //  Arrange
        var startDate = DateTime.Now;
        var endDate = startDate.AddDays(365);

        var model = new ProviderPlannedEndDateModel
        {
            UseSuggestedDate = false,
            PlannedEndDate = new DateField(endDate),
            ApprenticeshipActualStartDate = new DateField(startDate)
        };

        //  Act
        var validationResult = _validator.Validate(model);

        //  Assert
        validationResult.IsValid.Should().BeTrue();
    }
}
