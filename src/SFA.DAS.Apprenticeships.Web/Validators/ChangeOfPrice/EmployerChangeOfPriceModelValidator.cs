using FluentValidation;
using SFA.DAS.Apprenticeships.Web.Models.ChangeOfPrice;

namespace SFA.DAS.Apprenticeships.Web.Validators.ChangeOfPrice
{
    public class EmployerChangeOfPriceModelValidator : BaseChangeOfPriceValidator<EmployerChangeOfPriceModel>
    {
        public EmployerChangeOfPriceModelValidator()
        {
            RuleFor(x => x)
                .Must(HavePriceChange)
                .WithName(nameof(EmployerChangeOfPriceModel.ApprenticeshipTotalPrice))
                .WithMessage("You must change the total price");

            const string apprenticeshipTotalPriceErrorMessage = "The total price must be a whole number between 1 - 100,000";
            RuleFor(x => x.ApprenticeshipTotalPrice)
                .NotEmpty().WithMessage(apprenticeshipTotalPriceErrorMessage)
                .GreaterThan(0).WithMessage(apprenticeshipTotalPriceErrorMessage);

            RuleFor(x => x.ApprenticeshipTotalPrice)
                .LessThanOrEqualTo(x => 100000).WithMessage("The total price must not be greater than 100,000");

            RuleFor(x => x.EffectiveFromDate)
                .Must(IsValidDate)
                .WithMessage("Enter a date in the correct format");

            RuleFor(x => x)
                .Must(MustBeAfterTrainingStartDate)
                .WithName(nameof(EmployerChangeOfPriceModel.EffectiveFromDate))
                .WithMessage("Enter a date that is after the training start date");

            RuleFor(x => x)
                .Must(MustBeBeforePlannedEndDate)
                .WithName(nameof(EmployerChangeOfPriceModel.EffectiveFromDate))
                .WithMessage("The date entered must be before the planned end date");

            RuleFor(x => x)
                .Must(MustBeAfterEarliestValidDate)
                .WithName(nameof(EmployerChangeOfPriceModel.EffectiveFromDate))
                .WithMessage(x => $"You cannot enter a date in a previous academic year. The earliest date you can enter is {x.EarliestEffectiveDate!.Value.ToString("dd/MM/yyyy")}.");

            RuleFor(x => x)
                .Must(MustNotBeInTheFuture)
                .WithName(nameof(EmployerChangeOfPriceModel.EffectiveFromDate))
                .WithMessage(x => "The date must not be in the future");

            RuleFor(x => x.ReasonForChangeOfPrice)
                .NotEmpty()
                .WithMessage("You must enter a reason for requesting a price change. This will help the training provider when they review your request.");
        }

        private bool HavePriceChange(EmployerChangeOfPriceModel model)
        {
            if (model.OriginalApprenticeshipTotalPrice == model.ApprenticeshipTotalPrice)
            {
                return false;
            }

            return true;
        }
    }
}
