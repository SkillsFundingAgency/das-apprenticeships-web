using FluentValidation;
using SFA.DAS.Apprenticeships.Web.Models;

namespace SFA.DAS.Apprenticeships.Web.Validators
{
    public class CreateChangeOfPriceModelValidator : AbstractValidator<CreateChangeOfPriceModel>
    {
        private const string _trainingPriceErrorMessage = "The training price must be a whole number between 1 - 100,000";
        private const string _endPointAssessmentPriceErrorMessage = "The end-point assessment price must be a whole number between 1 - 100,000";
        private const string _totalPriceErrorMessage = "The total price must not be greater than 100,000";

        public CreateChangeOfPriceModelValidator()
        {

            RuleFor(x => x.ApprenticeshipTrainingPrice)
                .NotEmpty().WithMessage(_trainingPriceErrorMessage)
                .GreaterThan(0).WithMessage(_trainingPriceErrorMessage)
                .LessThanOrEqualTo(x => 100000).WithMessage(_trainingPriceErrorMessage);

            RuleFor(x => x.ApprenticeshipEndPointAssessmentPrice)
                .NotEmpty().WithMessage(_endPointAssessmentPriceErrorMessage)
                .GreaterThan(0).WithMessage(_endPointAssessmentPriceErrorMessage)
                .LessThanOrEqualTo(x => 100000).WithMessage(_endPointAssessmentPriceErrorMessage);

            RuleFor(x => x.ApprenticeshipTotalPrice)
                .LessThanOrEqualTo(x => 100000).WithMessage(_totalPriceErrorMessage);
        }
    }
}
