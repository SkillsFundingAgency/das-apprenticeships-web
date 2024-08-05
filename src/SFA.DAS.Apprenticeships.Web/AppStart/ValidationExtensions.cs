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
        //mvcBuilder.Services.RegisterRadioOptionValidatorsFromAssemblies(Assembly.GetExecutingAssembly());
        //mvcBuilder.Services.RegisterValidatorsFromAssemblies(Assembly.GetAssembly(typeof(ProviderChangeOfPriceModelValidator))!);
        return mvcBuilder;
    }

    public static void RegisterValidatorsFromAssemblies(this IServiceCollection services, params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            var validatorImplementationTypes = assembly.GetTypes()
                .Where(type => type.IsClass && !type.IsAbstract && InheritsFromGenericType(type, typeof(AbstractValidator<>)))
                .ToList();

            foreach (var validatorImplementationType in validatorImplementationTypes)
            {
                var modelType = validatorImplementationType.BaseType?.GetGenericArguments()[0];

                if(modelType?.FullName == null) continue;

                var validatorInterfaceType = typeof(IValidator<>).MakeGenericType(modelType);
                services.AddTransient(validatorInterfaceType, validatorImplementationType);
            }
        }
    }

    //public static void RegisterRadioOptionValidatorsFromAssemblies(this IServiceCollection services, params Assembly[] assemblies)
    //{
    //    foreach (var assembly in assemblies)
    //    {
    //        var types = assembly.GetTypes();
    //        var typesWithAttribute = assembly.GetTypes()
    //            .Where(type => type.GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(RadioOptionAttribute))).Any());

    //        foreach (var type in typesWithAttribute)
    //        {
    //            var validatorType = typeof(RadioOptionValidation<>).MakeGenericType(type);
    //            var validatorInterfaceType = typeof(IValidator<>).MakeGenericType(type);
    //            services.AddTransient(validatorInterfaceType, validatorType);
    //        }
    //    }
    //}

    private static bool InheritsFromGenericType(Type? type, Type genericType)
    {
        while (type != null && type != typeof(object))
        {
            var currentType = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
            if (genericType == currentType)
            {
                return true;
            }
            type = type.BaseType;
        }
        return false;
    }
}
