using FluentAssertions;
using SFA.DAS.Apprenticeships.Web.Models;
using SFA.DAS.Apprenticeships.Web.Validators;

namespace SFA.DAS.Apprenticeships.Web.UnitTests.Validators
{
    [TestFixture]
    public class EmployerChangeOfPriceModelValidatorTests
    {

        private const string _noChangeMessage = "You must change the total price";
        private const string _totalPriceMessage = "The total price must be a whole number between 1 - 100,000";
        private const string _totalPriceExceedsMaximum = "The total price must not be greater than 100,000";

        [TestCase(_noChangeMessage, 1000, 1000)]
        [TestCase(_totalPriceMessage, 0, 1000)]
        [TestCase(_totalPriceExceedsMaximum, 100001, 1000)]
        public void CreateChangeOfPriceModelValidator_ReturnsExpectedErrorMessage(
            string expectedMessage, int totalPrice, int originalTotalPrice)
        {
            // Arrange
            var model = new EmployerChangeOfPriceModel
            {
                ApprenticeshipTotalPrice = totalPrice,
                OriginalApprenticeshipTotalPrice = originalTotalPrice,
                EarliestEffectiveDate = new DateTime(2025, 10, 17)
            };
            var validator = new EmployerChangeOfPriceModelValidator();

            // Act
            var result = validator.Validate(model);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage == expectedMessage);
            result.Errors.Should().Contain(x => x.PropertyName == nameof(EmployerChangeOfPriceModel.ApprenticeshipTotalPrice));

        }

        [TestCase("Enter a date in the correct format", 41, 5, 2023)]
        [TestCase("Enter a date in the correct format", 1, 15, 2023)]
        [TestCase("Enter a date in the correct format", 1, 5, -1)]
        [TestCase("Enter a date that is after the training start date", 1, 5, 2023)]
        [TestCase("Enter a date that is after the training start date", 1, 6, 2024)]
        [TestCase("The date entered must be before the planned end date", 1, 5, 2027)]
        [TestCase("The date entered must be before the planned end date", 1, 6, 2026)]
        public void CreateChangeOfPriceModelValidator_EffectiveFromDate_ReturnsExpectedErrorMessage(
            string expectedMessage, int day, int month, int year)
        {
            // Arrange
            var model = new EmployerChangeOfPriceModel
            {
                ApprenticeshipTotalPrice = 6000,
                OriginalApprenticeshipTotalPrice = 5500,
                EffectiveFromDate = new DateField { Day = day, Month = month, Year = year },
                ApprenticeshipActualStartDate = new DateTime(2024, 6, 1),
                ApprenticeshipPlannedEndDate = new DateTime(2026, 6, 1),
                EarliestEffectiveDate = new DateTime(2025, 10, 17)
            };
            var validator = new EmployerChangeOfPriceModelValidator();

            // Act
            var result = validator.Validate(model);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage == expectedMessage);
            result.Errors.Should().Contain(x => x.PropertyName == nameof(EmployerChangeOfPriceModel.EffectiveFromDate));

        }

        [Test]
        public void CreateChangeOfPriceModelValidator_ReasonForChangeOfPrice_ReturnsExpectedErrorMessage()
        {
            // Arrange
            var model = new EmployerChangeOfPriceModel
            {
                ApprenticeshipTotalPrice = 6000,
                OriginalApprenticeshipTotalPrice = 5500,
                EffectiveFromDate = new DateField { Day = 15, Month = 7, Year = 2025 },
                ApprenticeshipActualStartDate = new DateTime(2024, 6, 1),
                ApprenticeshipPlannedEndDate = new DateTime(2026, 6, 1),
                ReasonForChangeOfPrice = null,
                EarliestEffectiveDate = new DateTime(2025, 10, 17)
            };
            var validator = new EmployerChangeOfPriceModelValidator();

            // Act
            var result = validator.Validate(model);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage == "You must enter a reason for requesting a price change. This will help the employer when they review your request.");
            result.Errors.Should().Contain(x => x.PropertyName == nameof(EmployerChangeOfPriceModel.ReasonForChangeOfPrice));

        }
    }
}
