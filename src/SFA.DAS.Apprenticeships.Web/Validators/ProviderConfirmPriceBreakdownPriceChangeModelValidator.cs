using FluentValidation;
using SFA.DAS.Apprenticeships.Web.Models.ChangeOfPrice;

namespace SFA.DAS.Apprenticeships.Web.Validators;

public class ProviderConfirmPriceBreakdownPriceChangeModelValidator : AbstractValidator<ProviderConfirmPriceBreakdownPriceChangeModel>
{
    public ProviderConfirmPriceBreakdownPriceChangeModelValidator()
    {
        RuleFor(x => x)
            .Must(BreakdownMatchRequestedTotalPrice)
            .WithName(nameof(ProviderConfirmPriceBreakdownPriceChangeModel.ApprenticeshipTrainingPrice))
            .WithMessage("The new training price and end-point assessment must match the total price requested by the employer.");
    }

    private bool BreakdownMatchRequestedTotalPrice(ProviderConfirmPriceBreakdownPriceChangeModel model)
    {
        if (model.ApprenticeshipTrainingPrice + model.ApprenticeshipEndPointAssessmentPrice != model.ApprenticeshipTotalPrice)
        {
            return false;
        }

        return true;
    }
}