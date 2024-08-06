using FluentValidation;
using SFA.DAS.Apprenticeships.Web.Models.ChangeOfStartDate;

namespace SFA.DAS.Apprenticeships.Web.Validators.ChangeOfStartDate;

public class ProviderPlannedEndDateModelValidator : BaseApprenticeshipsModelValidator<ProviderPlannedEndDateModel>
{
    public ProviderPlannedEndDateModelValidator()
    {
        RuleFor(x => x)
            .Must(BeMoreThanOneYear)
            .WithName(nameof(ProviderChangeOfStartDateModel.PlannedEndDate))
            .When(x => x.UseSuggestedDate == false)
            .WithMessage("The duration of an apprenticeship must be at least 365 days");

    }

    private static bool BeMoreThanOneYear(ProviderPlannedEndDateModel model)
    {
        var timeSpan = model.PlannedEndDate?.Date - model.ApprenticeshipActualStartDate?.Date;

        if(timeSpan == null)
            return false;

        return timeSpan.Value.Days >= 365;
    }
}