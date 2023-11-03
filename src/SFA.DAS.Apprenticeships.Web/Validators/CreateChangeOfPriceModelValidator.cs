using FluentValidation;
using SFA.DAS.Apprenticeships.Web.Models;

namespace SFA.DAS.Apprenticeships.Web.Validators
{
    public class CreateChangeOfPriceModelValidator : AbstractValidator<CreateChangeOfPriceModel>
    {
        private const string _apprenticeshipTrainingPriceErrorMessage = "The training price must be a whole number between 1 - 100,000";
        private const string _apprenticeshipEndPointAssessmentPriceErrorMessage = "The end-point assessment price must be a whole number between 1 - 100,000";

        public CreateChangeOfPriceModelValidator()
        {

            RuleFor(x => x.ApprenticeshipTrainingPrice)
                .NotEmpty().WithMessage(_apprenticeshipTrainingPriceErrorMessage)
                .GreaterThan(0).WithMessage(_apprenticeshipTrainingPriceErrorMessage)
                .LessThanOrEqualTo(x => 100000).WithMessage(_apprenticeshipTrainingPriceErrorMessage);

            RuleFor(x => x.ApprenticeshipEndPointAssessmentPrice)
                .NotEmpty().WithMessage(_apprenticeshipEndPointAssessmentPriceErrorMessage)
                .GreaterThan(0).WithMessage(_apprenticeshipEndPointAssessmentPriceErrorMessage)
                .LessThanOrEqualTo(x => 100000).WithMessage(_apprenticeshipEndPointAssessmentPriceErrorMessage);
        }
    }
}
