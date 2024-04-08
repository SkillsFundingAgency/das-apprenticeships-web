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
                }
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
    }
}

