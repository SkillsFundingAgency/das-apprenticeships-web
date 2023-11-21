using FluentAssertions;
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
                OriginalEndPointAssessmentPrice = originalAssessmentPrice
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
    }
}
