using FluentValidation;
using FluentValidation.AspNetCore;
using SFA.DAS.Apprenticeships.Web.Attributes;
using SFA.DAS.Apprenticeships.Web.Validators;
using SFA.DAS.Apprenticeships.Web.Validators.ChangeOfPrice;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace SFA.DAS.Apprenticeships.Web.AppStart;

[ExcludeFromCodeCoverage]
public static class ValidationExtensions
{
    public static IMvcBuilder RegisterValidators(this IMvcBuilder mvcBuilder)
    {

        mvcBuilder.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<ProviderChangeOfPriceModelValidator>());
        mvcBuilder.Services.RegisterValidatorsFromAssembliesWithAttribute(Assembly.GetExecutingAssembly());
        return mvcBuilder;
    }

    public static void RegisterValidatorsFromAssembliesWithAttribute(this IServiceCollection services, params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes();
            var typesWithAttribute = assembly.GetTypes()
                .Where(type => type.GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(RadioOptionAttribute))).Any());

            foreach (var type in typesWithAttribute)
            {
                var validatorType = typeof(RadioOptionValidation<>).MakeGenericType(type);
                var validatorInterfaceType = typeof(IValidator<>).MakeGenericType(type);
                services.AddTransient(validatorInterfaceType, validatorType);
            }
        }
    }
}
