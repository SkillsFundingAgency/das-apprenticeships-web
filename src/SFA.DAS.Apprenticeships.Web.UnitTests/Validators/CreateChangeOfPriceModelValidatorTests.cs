﻿using FluentAssertions;
using SFA.DAS.Apprenticeships.Web.Models;
using SFA.DAS.Apprenticeships.Web.Validators;

namespace SFA.DAS.Apprenticeships.Web.UnitTests.Validators
{
    [TestFixture]
    public class CreateChangeOfPriceModelValidatorTests
    {
        private const string _skipPropertyCheck = "";
        private const string _trainingPriceProperty = nameof(CreateChangeOfPriceModel.ApprenticeshipTrainingPrice);
        private const string _endPointAssessmentPriceProperty = nameof(CreateChangeOfPriceModel.ApprenticeshipEndPointAssessmentPrice);
        private const string _totalPriceProperty = nameof(CreateChangeOfPriceModel.ApprenticeshipTotalPrice);
        private const string _noChangeMessage = "You must change the training price and/or the end-point assessment price";
        private const string _trainingPriceMessage = "The training price must be a whole number between 1 - 100,000";
        private const string _endPointAssessmentPriceMessage = "The end-point assessment price must be a whole number between 1 - 100,000";
        private const string _totalPriceExceedsMaximum = "The total price must not be greater than 100,000";

        [TestCase(_noChangeMessage, _skipPropertyCheck, 1000, 1000, 1000, 1000)]
        [TestCase(_trainingPriceMessage, _trainingPriceProperty, 0, 1000, 2000, 2000)]
        [TestCase(_trainingPriceMessage, _trainingPriceProperty, 100001, 1000, 2000, 2000)]
        [TestCase(_endPointAssessmentPriceMessage, _endPointAssessmentPriceProperty, 1000, 0, 2000, 2000)]
        [TestCase(_endPointAssessmentPriceMessage, _endPointAssessmentPriceProperty, 1000, 100001, 2000, 2000)]
        [TestCase(_totalPriceExceedsMaximum, _totalPriceProperty, 95000, 15000, 2000, 2000)]
        public void CreateChangeOfPriceModelValidator_ReturnsExpectedErrorMessage(
            string expectedMessage, string expectedProperty, int trainingPrice, int assessmentPrice, int originalTrainingPrice, int originalAssessmentPrice)
        {
            // Arrange
            var model = new CreateChangeOfPriceModel
            {
                ApprenticeshipTrainingPrice = trainingPrice,
                ApprenticeshipEndPointAssessmentPrice = assessmentPrice,
                OriginalTrainingPrice = originalTrainingPrice,
                OriginalEndPointAssessmentPrice = originalAssessmentPrice,
				EarliestEffectiveDate = new DateTime(2025, 10, 17)
			};
            var validator = new CreateChangeOfPriceModelValidator();

            // Act
            var result = validator.Validate(model);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage == expectedMessage);

            if(expectedProperty != _skipPropertyCheck)
            {
                result.Errors.Should().Contain(x => x.PropertyName == expectedProperty);
            }
        }

        [TestCase("Enter a date in the correct format", 41, 5, 2023)]
        [TestCase("Enter a date in the correct format", 1, 15, 2023)]
        [TestCase("Enter a date in the correct format", 1, 5, -1)]
        [TestCase("Enter a date that is after the training start date", 1, 5, 2023)]
        [TestCase("Enter a date that is after the training start date", 1, 6, 2024)]
        [TestCase("The date entered must be before the planned end date", 1, 5, 2027)]
        [TestCase("The date entered must be before the planned end date", 1, 6, 2026)]
		[TestCase("You cannot enter a date in a previous academic year. The earliest date you can enter is 17/10/2025.", 10, 9, 2025)]
		[TestCase("You cannot enter a date in a previous academic year. The earliest date you can enter is 17/10/2025.", 16, 10, 2025)]
		public void CreateChangeOfPriceModelValidator_EffectiveFromDate_ReturnsExpectedErrorMessage(
            string expectedMessage, int day, int month, int year)
        {
            // Arrange
            var model = new CreateChangeOfPriceModel
            {
                ApprenticeshipTrainingPrice = 5500,
                ApprenticeshipEndPointAssessmentPrice = 500,
                OriginalTrainingPrice = 5000,
                OriginalEndPointAssessmentPrice = 500,
                EffectiveFromDate = new DateField { Day = day, Month = month, Year = year },
                ApprenticeshipActualStartDate = new DateTime(2024, 6, 1),
                ApprenticeshipPlannedEndDate = new DateTime(2026, 6, 1),
				EarliestEffectiveDate = new DateTime(2025, 10, 17)
			};
            var validator = new CreateChangeOfPriceModelValidator();

            // Act
            var result = validator.Validate(model);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage == expectedMessage);
            result.Errors.Should().Contain(x => x.PropertyName == nameof(CreateChangeOfPriceModel.EffectiveFromDate));

        }

        [Test]
        public void CreateChangeOfPriceModelValidator_ReasonForChangeOfPrice_ReturnsExpectedErrorMessage()
        {
            // Arrange
            var model = new CreateChangeOfPriceModel
            {
                ApprenticeshipTrainingPrice = 5500,
                ApprenticeshipEndPointAssessmentPrice = 500,
                OriginalTrainingPrice = 5000,
                OriginalEndPointAssessmentPrice = 500,
                EffectiveFromDate = new DateField { Day = 15, Month = 7, Year = 2025 },
                ApprenticeshipActualStartDate = new DateTime(2024, 6, 1),
                ApprenticeshipPlannedEndDate = new DateTime(2026, 6, 1),
				EarliestEffectiveDate = new DateTime(2025, 10, 17),
                ReasonForChangeOfPrice = null
            };
            var validator = new CreateChangeOfPriceModelValidator();

            // Act
            var result = validator.Validate(model);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage == "You must enter a reason for requesting a price change. This will help the employer when they review your request.");
            result.Errors.Should().Contain(x => x.PropertyName == nameof(CreateChangeOfPriceModel.ReasonForChangeOfPrice));

        }
    }
}
