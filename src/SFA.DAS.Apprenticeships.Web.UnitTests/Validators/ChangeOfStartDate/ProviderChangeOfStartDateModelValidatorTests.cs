using FluentAssertions;
using SFA.DAS.Apprenticeships.Web.Models;
using SFA.DAS.Apprenticeships.Web.Models.ChangeOfStartDate;
using SFA.DAS.Apprenticeships.Web.Validators.ChangeOfStartDate;

namespace SFA.DAS.Apprenticeships.Web.UnitTests.Validators.ChangeOfStartDate
{
    [TestFixture]
    public class ProviderChangeOfStartDateModelValidatorTests
    {
        private const string _noChangeMessage = "You must change the actual training start date";
        private const string _earliestStartDateMessage = "The new start date must be no earlier than ";
        private const string _latestStartDateMessage = "The new start date must be no later than ";
        private const string _lastFridayOfSchoolMessage = "The start date must be after {0} when the learner has reached school leaving age.";

        [TestCase(true)]
        [TestCase(false)]
        public void Validate_StartDate_ReturnsExpectedErrorMessage(bool startDateChanged)
        {
            // Arrange
            var originalStartDate = new DateTime(2023, 6, 1);
            var model = new ProviderChangeOfStartDateModel
            {
                OriginalApprenticeshipActualStartDate = originalStartDate,
                ApprenticeshipActualStartDate = new DateField
                {
                    Year = originalStartDate.Year,
                    Month = originalStartDate.Month,
                    Day = startDateChanged ? originalStartDate.Day + 1 : originalStartDate.Day
                },
                EarliestStartDate = originalStartDate,
                LatestStartDate = originalStartDate.AddDays(1),
                LastFridayOfSchool = originalStartDate,
                ReasonForChangeOfStartDate = "Reason"
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
            var model = new ProviderChangeOfStartDateModel
            {
                OriginalApprenticeshipActualStartDate = originalStartDate,
                ApprenticeshipActualStartDate = new DateField
                {
                    Year = originalStartDate.Year,
                    Month = originalStartDate.Month,
                    Day = originalStartDate.Day + 1,
                },
                EarliestStartDate = valid ? originalStartDate : originalStartDate.AddDays(2),
                LatestStartDate = originalStartDate.AddDays(1),
                LastFridayOfSchool = originalStartDate,
                ReasonForChangeOfStartDate = "Reason"
            };
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
            var model = new ProviderChangeOfStartDateModel
            {
                OriginalApprenticeshipActualStartDate = originalStartDate,
                ApprenticeshipActualStartDate = new DateField
                {
                    Year = originalStartDate.Year,
                    Month = originalStartDate.Month,
                    Day = originalStartDate.Day + 1,
                },
                EarliestStartDate = originalStartDate,
                LatestStartDate = valid ? originalStartDate.AddDays(1) : originalStartDate,
                LastFridayOfSchool = originalStartDate,
                ReasonForChangeOfStartDate = "Reason"
            };
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
            var model = new ProviderChangeOfStartDateModel
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
                LastFridayOfSchool = valid ? originalStartDate : originalStartDate.AddDays(2),
                ReasonForChangeOfStartDate = "Reason"
            };
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
    }
}

