using FluentValidation;
using SFA.DAS.Apprenticeships.Web.Models.ChangeOfStartDate;

namespace SFA.DAS.Apprenticeships.Web.Validators.ChangeOfStartDate;

public class ProviderChangeOfStartDateModelValidator : BaseApprenticeshipsModelValidator<ProviderChangeOfStartDateModel>
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
                
        RuleFor(x => x)
            .Must(NotBeEarlierThanStandardEarliestDate)
            .WithName(nameof(ProviderChangeOfStartDateModel.ApprenticeshipActualStartDate))
            .WithMessage(x => $"This training course is only available to apprentices with a start date after {x.StandardEarliestDate.GetValueOrDefault():dd MM yyyy}.");

        RuleFor(x => x)
            .Must(NotBeLaterThanStandardLatestDate)
            .WithName(nameof(ProviderChangeOfStartDateModel.ApprenticeshipActualStartDate))
            .WithMessage(x => $"This training course is only available to apprentices with a start date before {x.StandardLatestDate.GetValueOrDefault():dd MM yyyy}.");

        RuleFor(x => x)
            .Must(NotBeEarlierThanStandardVersionEarliestDate)
            .WithName(nameof(ProviderChangeOfStartDateModel.ApprenticeshipActualStartDate))
            .WithMessage(x => $"This version of the training course is only available to apprentices with a start date after {x.StandardVersionEarliestDate.GetValueOrDefault():dd MM yyyy}.");

        RuleFor(x => x)
            .Must(NotBeLaterThanStandardVersionLatestDate)
            .WithName(nameof(ProviderChangeOfStartDateModel.ApprenticeshipActualStartDate))
            .WithMessage(x => $"This version of the training course is only available to apprentices with a start date before {x.StandardVersionLatestDate.GetValueOrDefault():dd MM yyyy}.");
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

    private static bool NotBeEarlierThanStandardEarliestDate(ProviderChangeOfStartDateModel model)
    {
        return model.StandardEarliestDate == null || model.ApprenticeshipActualStartDate?.Date >= model.StandardEarliestDate;
    }

    private static bool NotBeLaterThanStandardLatestDate(ProviderChangeOfStartDateModel model)
    {
        return model.StandardLatestDate == null || model.ApprenticeshipActualStartDate?.Date <= model.StandardLatestDate;
    }

    private static bool NotBeEarlierThanStandardVersionEarliestDate(ProviderChangeOfStartDateModel model)
    {
        if(NotBeEarlierThanStandardEarliestDate(model))
            return model.StandardVersionEarliestDate == null || model.ApprenticeshipActualStartDate?.Date >= model.StandardVersionEarliestDate;
        else
            return true;
    }

    private static bool NotBeLaterThanStandardVersionLatestDate(ProviderChangeOfStartDateModel model)
    {
        if (NotBeLaterThanStandardLatestDate(model))
            return model.StandardVersionLatestDate == null || model.ApprenticeshipActualStartDate?.Date <= model.StandardVersionLatestDate;
        else
            return true;
    }
}