using FluentValidation.AspNetCore;
using SFA.DAS.Apprenticeships.Web.Validators.ChangeOfPrice;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Apprenticeships.Web.AppStart;

[ExcludeFromCodeCoverage]
public static class ValidationExtensions
{
    public static IMvcBuilder RegisterValidators(this IMvcBuilder mvcBuilder)
    {
        mvcBuilder.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<ProviderChangeOfPriceModelValidator>());
        return mvcBuilder;
    }
}
