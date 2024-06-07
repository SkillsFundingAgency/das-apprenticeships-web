using FluentValidation;
using SFA.DAS.Apprenticeships.Web.Models.ChangeOfPaymentStatus;

namespace SFA.DAS.Apprenticeships.Web.Validators.ChangeOfPaymentStatus;

public class UnfreezeProviderPaymentsModelValidator : AbstractValidator<UnfreezeProviderPaymentsModel>
{
    public UnfreezeProviderPaymentsModelValidator()
    {
        RuleFor(x => x.UnfreezePayments)
            .NotNull()
            .WithMessage("You must select an option");
    }


}
