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

            RuleFor(x => x)
                .Must(NotBeEarlierThanEarliestDate)
                .WithName(nameof(ProviderChangeOfStartDateModel.ApprenticeshipActualStartDate))
                .WithMessage(x => $"The new start date must be no earlier than {x.EarliestStartDate.GetValueOrDefault():dd MM yyyy}.");

            RuleFor(x => x)
                .Must(NotBeLaterThanLatestDate)
                .WithName(nameof(ProviderChangeOfStartDateModel.ApprenticeshipActualStartDate))
                .WithMessage(x => $"The new start date must be no later than {x.LatestStartDate.GetValueOrDefault():dd MM yyyy}.");

            RuleFor(x => x)
                .Must(NotBeEarlierThanLastFridayOfSchool)
                .WithName(nameof(ProviderChangeOfStartDateModel.ApprenticeshipActualStartDate))
                .WithMessage(x => $"The start date must be after {x.LastFridayOfSchool:dd MM yyyy} when the learner has reached school leaving age.");

            RuleFor(x => x.ReasonForChangeOfStartDate)
                .NotEmpty()
                .WithMessage("You must enter a reason for requesting a change of start date. This will help the employer when they review your request.");

            RuleFor(x => x.ReasonForChangeOfStartDate)
                .Length(0, 200)
                .WithMessage("Reason cannot exceed 200 characters");
        }

        private static bool HaveStartDateChange(ProviderChangeOfStartDateModel model)
        {
            return model.ApprenticeshipActualStartDate?.Date != model.OriginalApprenticeshipActualStartDate;
        }

        private static bool NotBeEarlierThanEarliestDate(ProviderChangeOfStartDateModel model)
        {
            return model.ApprenticeshipActualStartDate?.Date >= model.EarliestStartDate;
        }

        private static bool NotBeLaterThanLatestDate(ProviderChangeOfStartDateModel model)
        {
            return model.ApprenticeshipActualStartDate?.Date <= model.LatestStartDate;
        }

        private static bool NotBeEarlierThanLastFridayOfSchool(ProviderChangeOfStartDateModel model)
        {
            return model.ApprenticeshipActualStartDate?.Date >= model.LastFridayOfSchool;
        }
    }
}
