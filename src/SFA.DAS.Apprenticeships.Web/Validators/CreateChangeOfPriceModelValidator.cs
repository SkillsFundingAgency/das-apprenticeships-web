using FluentValidation;
using SFA.DAS.Apprenticeships.Web.Models;

namespace SFA.DAS.Apprenticeships.Web.Validators
{
    public class CreateChangeOfPriceModelValidator : AbstractValidator<CreateChangeOfPriceModel>
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


        }

        private bool HavePriceChange(CreateChangeOfPriceModel model)
        {
            if(model.OriginalTrainingPrice == model.ApprenticeshipTrainingPrice && 
                model.OriginalEndPointAssessmentPrice == model.ApprenticeshipEndPointAssessmentPrice)
            {
                return false;
            }

            return true;
        }
    }
}
