using FluentValidation;
using SFA.DAS.Apprenticeships.Web.Models.ChangeOfStartDate;

namespace SFA.DAS.Apprenticeships.Web.Validators.ChangeOfStartDate
{
    public class ProviderChangeOfStartDateModelValidator : AbstractValidator<ProviderChangeOfStartDateModel>
    {
        public ProviderChangeOfStartDateModelValidator()
        {
            RuleFor(x => x)
                .Must(HaveStartDateChange)
                .WithName(nameof(ProviderChangeOfStartDateModel.ApprenticeshipActualStartDate))
                .WithMessage("You must change the actual training start date");
        }

        private static bool HaveStartDateChange(ProviderChangeOfStartDateModel model)
        {
            return model.ApprenticeshipActualStartDate?.Date != model.OriginalApprenticeshipActualStartDate;
        }

    }
}
