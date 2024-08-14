using FluentValidation;
using SFA.DAS.Apprenticeships.Web.Models.ChangeOfPaymentStatus;

namespace SFA.DAS.Apprenticeships.Web.Validators.PaymentsFreeze;

public class FreezeProviderPaymentsModelValidator : BaseApprenticeshipsModelValidator<FreezeProviderPaymentsModel>
{
    public FreezeProviderPaymentsModelValidator()
    {
        RuleFor(x => x.FreezePayments)
            .NotNull()
            .WithMessage("You must select an option");
    }
}
