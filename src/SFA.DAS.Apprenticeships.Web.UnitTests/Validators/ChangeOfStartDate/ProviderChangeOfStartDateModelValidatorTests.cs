using FluentAssertions;
using SFA.DAS.Apprenticeships.Web.Models;
using SFA.DAS.Apprenticeships.Web.Models.ChangeOfStartDate;
using SFA.DAS.Apprenticeships.Web.Validators.ChangeOfStartDate;
using System;

namespace SFA.DAS.Apprenticeships.Web.UnitTests.Validators.ChangeOfStartDate;

[TestFixture]
public class ProviderChangeOfStartDateModelValidatorTests
{
    private const string _noChangeMessage = "You must change the actual training start date";
    private const string _earliestStartDateMessage = "The new start date must be no earlier than ";
    private const string _latestStartDateMessage = "The new start date must be no later than ";
    private const string _lastFridayOfSchoolMessage = "The start date must be after {0} when the learner has reached school leaving age.";
    private const string _standardEarliestDateMessage = "This training course is only available to apprentices with a start date after ";
    private const string _standardLatestDateMessage = "This training course is only available to apprentices with a start date before ";
    private const string _standardVersionEarliestDateMessage = "This version of the training course is only available to apprentices with a start date after ";
    private const string _standardVersionLatestDateMessage = "This version of the training course is only available to apprentices with a start date before ";

    [TestCase(true)]
    [TestCase(false)]
    public void Validate_StartDate_ReturnsExpectedErrorMessage(bool startDateChanged)
    {
        // Arrange
        var originalStartDate = new DateTime(2023, 6, 1);
        var model = BuildValidTestModel(originalStartDate);
        model.ApprenticeshipActualStartDate = new DateField
        {
            Year = originalStartDate.Year,
            Month = originalStartDate.Month,
            Day = startDateChanged ? originalStartDate.Day + 1 : originalStartDate.Day
        };

        var validator = new ProviderChangeOfStartDateModelValidator();

        // Act
        var result = validator.Validate(model);

        // Assert
        if (startDateChanged)
        {
            result.IsValid.Should().BeTrue();
        }
        else
        {
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage == _noChangeMessage);
        }
    }

    [TestCase(true)]
    [TestCase(false)]
    public void Validate_EarliestStartDate_ReturnsExpectedErrorMessage(bool valid)
    {
        // Arrange
        var originalStartDate = new DateTime(2023, 6, 1);
        var model = BuildValidTestModel(originalStartDate);
        model.EarliestStartDate = valid ? originalStartDate : originalStartDate.AddDays(2);

        var validator = new ProviderChangeOfStartDateModelValidator();

        // Act
        var result = validator.Validate(model);

        // Assert
        if (valid)
        {
            result.IsValid.Should().BeTrue();
        }
        else
        {
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage == $"{_earliestStartDateMessage}{model.EarliestStartDate.Value:dd MM yyyy}.");
        }
    }

    [TestCase(true)]
    [TestCase(false)]
    public void Validate_LatestStartDate_ReturnsExpectedErrorMessage(bool valid)
    {
        // Arrange
        var originalStartDate = new DateTime(2023, 6, 1);
        var model = BuildValidTestModel(originalStartDate);
        model.LatestStartDate = valid ? originalStartDate.AddDays(1) : originalStartDate;

        var validator = new ProviderChangeOfStartDateModelValidator();

        // Act
        var result = validator.Validate(model);

        // Assert
        if (valid)
        {
            result.IsValid.Should().BeTrue();
        }
        else
        {
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage == $"{_latestStartDateMessage}{model.LatestStartDate.Value:dd MM yyyy}.");
        }
    }

    [TestCase(true)]
    [TestCase(false)]
    public void Validate_LastFridayOfSchool_ReturnsExpectedErrorMessage(bool valid)
    {
        // Arrange
        var originalStartDate = new DateTime(2023, 6, 1);
        var model = BuildValidTestModel(originalStartDate);
        model.LastFridayOfSchool = valid ? originalStartDate : originalStartDate.AddDays(2);

        var validator = new ProviderChangeOfStartDateModelValidator();

        // Act
        var result = validator.Validate(model);

        // Assert
        if (valid)
        {
            result.IsValid.Should().BeTrue();
        }
        else
        {
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage == String.Format(_lastFridayOfSchoolMessage, model.LastFridayOfSchool.ToString("dd MM yyyy")));
        }
    }

    [TestCase(true)]
    [TestCase(false)]
    public void Validate_StandardEarliestDate_ReturnsExpectedErrorMessage(bool valid)
    {
        // Arrange
        var originalStartDate = new DateTime(2023, 6, 1);
        var model = BuildValidTestModel(originalStartDate);
        model.StandardEarliestDate = valid ? originalStartDate : originalStartDate.AddDays(2);

        var validator = new ProviderChangeOfStartDateModelValidator();

        // Act
        var result = validator.Validate(model);

        // Assert
        if (valid)
        {
            result.IsValid.Should().BeTrue();
        }
        else
        {
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage == $"{_standardEarliestDateMessage}{model.StandardEarliestDate.Value:dd MM yyyy}.");
        }
    }

    [TestCase(true)]
    [TestCase(false)]
    public void Validate_StandardLatestDate_ReturnsExpectedErrorMessage(bool valid)
    {
        // Arrange
        var originalStartDate = new DateTime(2023, 6, 1);
        var model = BuildValidTestModel(originalStartDate);
        model.StandardLatestDate = valid ? originalStartDate.AddDays(1) : originalStartDate;

        var validator = new ProviderChangeOfStartDateModelValidator();

        // Act
        var result = validator.Validate(model);

        // Assert
        if (valid)
        {
            result.IsValid.Should().BeTrue();
        }
        else
        {
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage == $"{_standardLatestDateMessage}{model.StandardLatestDate.Value:dd MM yyyy}.");
        }
    }

    [TestCase(true)]
    [TestCase(false)]
    public void Validate_StandardVersionEarliestDate_ReturnsExpectedErrorMessage(bool valid)
    {
        // Arrange
        var originalStartDate = new DateTime(2023, 6, 1);
        var model = BuildValidTestModel(originalStartDate);
        model.StandardVersionEarliestDate = valid ? originalStartDate : originalStartDate.AddDays(2);

        var validator = new ProviderChangeOfStartDateModelValidator();

        // Act
        var result = validator.Validate(model);

        // Assert
        if (valid)
        {
            result.IsValid.Should().BeTrue();
        }
        else
        {
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage == $"{_standardVersionEarliestDateMessage}{model.StandardVersionEarliestDate.Value:dd MM yyyy}.");
        }
    }

    [TestCase(true)]
    [TestCase(false)]
    public void Validate_StandardVersionLatestDate_ReturnsExpectedErrorMessage(bool valid)
    {
        // Arrange
        var originalStartDate = new DateTime(2023, 6, 1);
        var model = BuildValidTestModel(originalStartDate);
        model.StandardVersionLatestDate = valid ? originalStartDate.AddDays(1) : originalStartDate;

        var validator = new ProviderChangeOfStartDateModelValidator();

        // Act
        var result = validator.Validate(model);

        // Assert
        if (valid)
        {
            result.IsValid.Should().BeTrue();
        }
        else
        {
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage == $"{_standardVersionLatestDateMessage}{model.StandardVersionLatestDate.Value:dd MM yyyy}.");
        }
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
        result.Errors.Should().NotContain(x => x.ErrorMessage == $"{_standardVersionEarliestDateMessage}{model.StandardVersionEarliestDate.Value:dd MM yyyy}.");
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
        result.Errors.Should().NotContain(x => x.ErrorMessage == $"{_standardVersionLatestDateMessage}{model.StandardVersionLatestDate.Value:dd MM yyyy}.");
    }

    private ProviderChangeOfStartDateModel BuildValidTestModel(DateTime originalStartDate)
    {
        return new ProviderChangeOfStartDateModel
        {
            OriginalApprenticeshipActualStartDate = originalStartDate,
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
            StandardLatestDate = originalStartDate.AddDays(1),
            StandardVersionEarliestDate = originalStartDate,
            StandardVersionLatestDate = originalStartDate.AddDays(1)
        };
    }
}