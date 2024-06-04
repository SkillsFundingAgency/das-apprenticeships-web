using FluentAssertions;
using SFA.DAS.Apprenticeships.Web.Models;
using SFA.DAS.Apprenticeships.Web.Models.ChangeOfStartDate;
using SFA.DAS.Apprenticeships.Web.Validators.ChangeOfStartDate;

namespace SFA.DAS.Apprenticeships.Web.UnitTests.Validators.ChangeOfStartDate;

[TestFixture]
public class ProviderChangeOfStartDateModelValidatorTests
{
    private const string NoChangeMessage = "You must change the actual training start date";
    private const string EarliestStartDateMessage = "The new start date must be no earlier than ";
    private const string LatestStartDateMessage = "The new start date must be no later than ";
    private const string LastFridayOfSchoolMessage = "The start date must be after {0} when the learner has reached school leaving age.";
    private const string StandardEarliestDateMessage = "This training course is only available to apprentices with a start date after ";
    private const string StandardLatestDateMessage = "This training course is only available to apprentices with a start date before ";
    private const string StandardVersionEarliestDateMessage = "This version of the training course is only available to apprentices with a start date after ";
    private const string StandardVersionLatestDateMessage = "This version of the training course is only available to apprentices with a start date before ";

    [Test]
    public void Validate_Valid_DoesNotReturnMessage()
    {
        // Arrange
        var originalStartDate = new DateTime(2023, 6, 1);
        var model = BuildValidTestModel(originalStartDate);

        var validator = new ProviderChangeOfStartDateModelValidator();

        // Act
        var result = validator.Validate(model);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Test]
    public void Validate_StartDate_ReturnsExpectedErrorMessage()
    {
        // Arrange
        var originalStartDate = new DateTime(2023, 6, 1);
        var model = BuildValidTestModel(originalStartDate);
        model.ApprenticeshipActualStartDate = new DateField
        {
            Year = originalStartDate.Year,
            Month = originalStartDate.Month,
            Day = originalStartDate.Day

        };

        var validator = new ProviderChangeOfStartDateModelValidator();

        // Act
        var result = validator.Validate(model);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.ErrorMessage == NoChangeMessage);
    }

    [Test]
    public void Validate_EarliestStartDate_ReturnsExpectedErrorMessage()
    {
        // Arrange
        var originalStartDate = new DateTime(2023, 6, 1);
        var model = BuildValidTestModel(originalStartDate);
        model.EarliestStartDate = originalStartDate.AddDays(2);

        var validator = new ProviderChangeOfStartDateModelValidator();

        // Act
        var result = validator.Validate(model);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.ErrorMessage == $"{EarliestStartDateMessage}{model.EarliestStartDate.Value:dd MM yyyy}.");
    }

    [Test]
    public void Validate_LatestStartDate_ReturnsExpectedErrorMessage()
    {
        // Arrange
        var originalStartDate = new DateTime(2023, 6, 1);
        var model = BuildValidTestModel(originalStartDate);
        model.LatestStartDate = originalStartDate;
        var validator = new ProviderChangeOfStartDateModelValidator();

        // Act
        var result = validator.Validate(model);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.ErrorMessage == $"{LatestStartDateMessage}{model.LatestStartDate.Value:dd MM yyyy}.");
    }

    [Test]
    public void Validate_LastFridayOfSchool_ReturnsExpectedErrorMessage()
    {
        // Arrange
        var originalStartDate = new DateTime(2023, 6, 1);
        var model = BuildValidTestModel(originalStartDate);
        model.LastFridayOfSchool = originalStartDate.AddDays(2);

        var validator = new ProviderChangeOfStartDateModelValidator();

        // Act
        var result = validator.Validate(model);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.ErrorMessage == String.Format(LastFridayOfSchoolMessage, model.LastFridayOfSchool.ToString("dd MM yyyy")));
    }

    [Test]
    public void Validate_StandardEarliestDate_ReturnsExpectedErrorMessage()
    {
        // Arrange
        var originalStartDate = new DateTime(2023, 6, 1);
        var model = BuildValidTestModel(originalStartDate);
        model.StandardEarliestDate = originalStartDate.AddDays(2);

        var validator = new ProviderChangeOfStartDateModelValidator();

        // Act
        var result = validator.Validate(model);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.ErrorMessage == $"{StandardEarliestDateMessage}{model.StandardEarliestDate.Value:dd MM yyyy}.");
    }

    [Test]
    public void Validate_StandardLatestDate_ReturnsExpectedErrorMessage()
    {
        // Arrange
        var originalStartDate = new DateTime(2023, 6, 1);
        var model = BuildValidTestModel(originalStartDate);
        model.StandardLatestDate = originalStartDate;


        var validator = new ProviderChangeOfStartDateModelValidator();

        // Act
        var result = validator.Validate(model);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.ErrorMessage == $"{StandardLatestDateMessage}{model.StandardLatestDate.Value:dd MM yyyy}.");
    }

    [Test]
    public void Validate_StandardVersionEarliestDate_ReturnsExpectedErrorMessage()
    {
        // Arrange
        var originalStartDate = new DateTime(2023, 6, 1);
        var model = BuildValidTestModel(originalStartDate);
        model.StandardVersionEarliestDate = originalStartDate.AddDays(2);

        var validator = new ProviderChangeOfStartDateModelValidator();

        // Act
        var result = validator.Validate(model);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.ErrorMessage == $"{StandardVersionEarliestDateMessage}{model.StandardVersionEarliestDate.Value:dd MM yyyy}.");
    }

    [Test]
    public void Validate_StandardVersionLatestDate_ReturnsExpectedErrorMessage()
    {
        // Arrange
        var originalStartDate = new DateTime(2023, 6, 1);
        var model = BuildValidTestModel(originalStartDate);
        model.StandardVersionLatestDate = originalStartDate;

        var validator = new ProviderChangeOfStartDateModelValidator();

        // Act
        var result = validator.Validate(model);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.ErrorMessage == $"{StandardVersionLatestDateMessage}{model.StandardVersionLatestDate.Value:dd MM yyyy}.");
    }

    [Test]
    public void Validate_StandardVersionEarliestDate_OnlyIfStandardEarliestDateIsValid_ReturnsExpectedErrorMessage()
    {
        // Arrange
        var originalStartDate = new DateTime(2023, 6, 1);
        var model = BuildValidTestModel(originalStartDate);
        model.StandardEarliestDate = originalStartDate.AddDays(2);
        model.StandardVersionEarliestDate = originalStartDate.AddDays(2);

        var validator = new ProviderChangeOfStartDateModelValidator();

        // Act
        var result = validator.Validate(model);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotContain(x => x.ErrorMessage == $"{StandardVersionEarliestDateMessage}{model.StandardVersionEarliestDate.Value:dd MM yyyy}.");
    }

    [Test]
    public void Validate_StandardVersionLatestDate_OnlyIfStandardLatestDateIsValid_ReturnsExpectedErrorMessage()
    {
        // Arrange
        var originalStartDate = new DateTime(2023, 6, 1);
        var model = BuildValidTestModel(originalStartDate);
        model.StandardLatestDate = originalStartDate;
        model.StandardVersionLatestDate = originalStartDate;

        var validator = new ProviderChangeOfStartDateModelValidator();

        // Act
        var result = validator.Validate(model);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotContain(x => x.ErrorMessage == $"{StandardVersionLatestDateMessage}{model.StandardVersionLatestDate.Value:dd MM yyyy}.");
    }

    private ProviderChangeOfStartDateModel BuildValidTestModel(DateTime originalStartDate)
    {
        return new ProviderChangeOfStartDateModel
        {
            OriginalApprenticeshipActualStartDate = originalStartDate,
            OriginalPlannedEndDate = originalStartDate.AddMonths(22),
            ApprenticeshipActualStartDate = new DateField
            {
                Year = originalStartDate.Year,
                Month = originalStartDate.Month,
                Day = originalStartDate.Day + 1,
            },
            EarliestStartDate = originalStartDate,
            LatestStartDate = originalStartDate.AddDays(1),
            LastFridayOfSchool = originalStartDate,
            StandardEarliestDate = originalStartDate,
            ReasonForChangeOfStartDate = "Reason",
            StandardLatestDate = originalStartDate.AddDays(1),
            StandardVersionEarliestDate = originalStartDate,
            StandardVersionLatestDate = originalStartDate.AddDays(1)
        };
    }
}