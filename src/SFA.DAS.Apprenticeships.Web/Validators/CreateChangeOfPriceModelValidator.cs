using FluentValidation;
using SFA.DAS.Apprenticeships.Web.Models;

namespace SFA.DAS.Apprenticeships.Web.Validators
{
    public class CreateChangeOfPriceModelValidator : AbstractValidator<ProviderChangeOfPriceModel>
    {
        public CreateChangeOfPriceModelValidator()
        {
            RuleFor(x => x)
                .Must(HavePriceChange)
                .WithMessage("You must change the training price and/or the end-point assessment price");

            const string trainingPriceErrorMessage = "The training price must be a whole number between 1 - 100,000";
            RuleFor(x => x.ApprenticeshipTrainingPrice)
                .NotEmpty().WithMessage(trainingPriceErrorMessage)
                .GreaterThan(0).WithMessage(trainingPriceErrorMessage)
                .LessThanOrEqualTo(x => 100000).WithMessage(trainingPriceErrorMessage);

            const string endPointAssessmentPriceErrorMessage = "The end-point assessment price must be a whole number between 1 - 100,000";
            RuleFor(x => x.ApprenticeshipEndPointAssessmentPrice)
                .NotEmpty().WithMessage(endPointAssessmentPriceErrorMessage)
                .GreaterThan(0).WithMessage(endPointAssessmentPriceErrorMessage)
                .LessThanOrEqualTo(x => 100000).WithMessage(endPointAssessmentPriceErrorMessage);

            RuleFor(x => x.ApprenticeshipTotalPrice)
                .LessThanOrEqualTo(x => 100000).WithMessage("The total price must not be greater than 100,000");

            RuleFor(x => x.EffectiveFromDate)
                .Must(IsValidDate)
                .WithMessage("Enter a date in the correct format");

            RuleFor(x => x)
                .Must(MustBeAfterTrainingStartDate)
                .WithName(nameof(ProviderChangeOfPriceModel.EffectiveFromDate))
                .WithMessage("Enter a date that is after the training start date");

            RuleFor(x => x)
                .Must(MustBeBeforePlannedEndDate)
                .WithName(nameof(ProviderChangeOfPriceModel.EffectiveFromDate))
                .WithMessage("The date entered must be before the planned end date");

			RuleFor(x => x)
	            .Must(MustBeAfterEarliestValidDate)
	            .WithName(nameof(ProviderChangeOfPriceModel.EffectiveFromDate))
	            .WithMessage(x => $"You cannot enter a date in a previous academic year. The earliest date you can enter is {x.EarliestEffectiveDate!.Value.ToString("dd/MM/yyyy")}.");

			RuleFor(x => x.ReasonForChangeOfPrice)
                .NotEmpty()
                .WithMessage("You must enter a reason for requesting a price change. This will help the employer when they review your request.");
        }

        private bool HavePriceChange(ProviderChangeOfPriceModel model)
        {
            if(model.OriginalTrainingPrice == model.ApprenticeshipTrainingPrice && 
                model.OriginalEndPointAssessmentPrice == model.ApprenticeshipEndPointAssessmentPrice)
            {
                return false;
            }

            return true;
        }

        private bool IsValidDate(DateField dateField)
        {
            if(dateField.Date == null)
            {
                return false;
            }

            return true;
        }

        private bool MustBeAfterTrainingStartDate(ProviderChangeOfPriceModel model)
        {
            if(model.ApprenticeshipActualStartDate.HasValue && model.EffectiveFromDate.Date <= model.ApprenticeshipActualStartDate)
            {
                return false;
            }

            return true;
        }

        private bool MustBeBeforePlannedEndDate(ProviderChangeOfPriceModel model)
        {
            if (model.ApprenticeshipPlannedEndDate.HasValue && model.EffectiveFromDate.Date >= model.ApprenticeshipPlannedEndDate)
            {
                return false;
            }

            return true;
        }

		private bool MustBeAfterEarliestValidDate(ProviderChangeOfPriceModel model)
		{
            if (!model.EarliestEffectiveDate.HasValue)
            {
                throw new InvalidOperationException("EarliestEffectiveDate must be set");//This should come from api call to get Apprenticeship Price
            }

			if (model.EffectiveFromDate.Date < model.EarliestEffectiveDate)
			{
				return false;
			}

			return true;
		}
	}
}
