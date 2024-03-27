using FluentAssertions;
using SFA.DAS.Apprenticeships.Web.Models.ChangeOfPrice;
using SFA.DAS.Apprenticeships.Web.Validators.ChangeOfPrice;

namespace SFA.DAS.Apprenticeships.Web.UnitTests.Validators.ChangeOfPrice;

[TestFixture]
public class ProviderConfirmPriceBreakdownPriceChangeModelValidatorTests
{
    private ProviderConfirmPriceBreakdownPriceChangeModelValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _validator = new ProviderConfirmPriceBreakdownPriceChangeModelValidator();
    }

    [TestCase(5000, 2000, 7000, true)]
    [TestCase(9000, 1000, 8500, false)]
    public void BreakdownMatchRequestedTotalPriceValidation(decimal trainingPrice, decimal assessmentPrice, decimal totalPrice, bool expectedToBeValid)
    {
        // Arrange
        var model = new ProviderConfirmPriceBreakdownPriceChangeModel
        {
            ApprenticeshipTrainingPrice = trainingPrice,
            ApprenticeshipEndPointAssessmentPrice = assessmentPrice,
            ApprenticeshipTotalPrice = totalPrice
        };

        // Act
        var result = _validator.Validate(model);

        // Assert
        result.IsValid.Should().Be(expectedToBeValid);
        if(!expectedToBeValid ) result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(ProviderConfirmPriceBreakdownPriceChangeModel.ApprenticeshipTrainingPrice));
    }
}