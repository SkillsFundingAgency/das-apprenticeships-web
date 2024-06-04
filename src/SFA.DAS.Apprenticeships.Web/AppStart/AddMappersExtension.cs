using SFA.DAS.Apprenticeships.Web.Models;
using SFA.DAS.Apprenticeships.Web.Models.ChangeOfPrice;
using SFA.DAS.Apprenticeships.Web.Models.ChangeOfStartDate;

namespace SFA.DAS.Apprenticeships.Web.AppStart;

public static class AddMappersExtension
{
    public static void AddMappers(this IServiceCollection services)
    {
        services.AddTransient<IMapper<ProviderChangeOfPriceModel>, ProviderChangeOfPriceModelMapper>();
        services.AddTransient<IMapper<EmployerChangeOfPriceModel>, EmployerChangeOfPriceModelMapper>();
        services.AddTransient<IMapper<EmployerViewPendingPriceChangeModel>, EmployerViewPendingPriceChangeModelMapper>();
        services.AddTransient<IMapper<EmployerCancelPriceChangeModel>, EmployerCancelPriceChangeModelMapper>();
        services.AddTransient<IMapper<ProviderCancelPriceChangeModel>, ProviderCancelPriceChangeModelMapper>();
        services.AddTransient<IMapper<ProviderViewPendingPriceChangeModel>, ProviderViewPendingPriceChangeModelMapper>();
        services.AddTransient<IMapper<ProviderConfirmPriceBreakdownPriceChangeModel>, ProviderConfirmPriceBreakdownPriceChangeModelMapper>();

        services.AddTransient<IMapper<ProviderChangeOfStartDateModel>, ProviderChangeOfStartDateModelMapper>();
        services.AddTransient<IMapper<EmployerViewPendingStartDateChangeModel>, EmployerViewPendingStartDateChangeModelMapper>();
        services.AddTransient<IMapper<ProviderCancelStartDateModel>, ProviderCancelStartDateModelMapper>();
        services.AddTransient<IMapper<ProviderPlannedEndDateModel>, ProviderPlannedEndDateModelMapper>();


        services.AddTransient<IMapper>((serviceProvider) =>
        {
            var mapperResolver = new MapperResolver();
            mapperResolver.Register(serviceProvider.GetService<IMapper<ProviderChangeOfPriceModel>>()!);
            mapperResolver.Register(serviceProvider.GetService<IMapper<EmployerChangeOfPriceModel>>()!);
            mapperResolver.Register(serviceProvider.GetService<IMapper<EmployerViewPendingPriceChangeModel>>()!);
            mapperResolver.Register(serviceProvider.GetService<IMapper<EmployerCancelPriceChangeModel>>()!);
            mapperResolver.Register(serviceProvider.GetService<IMapper<ProviderCancelPriceChangeModel>>()!);
            mapperResolver.Register(serviceProvider.GetService<IMapper<ProviderViewPendingPriceChangeModel>>()!);
            mapperResolver.Register(serviceProvider.GetService<IMapper<ProviderConfirmPriceBreakdownPriceChangeModel>>()!);

            mapperResolver.Register(serviceProvider.GetService<IMapper<ProviderChangeOfStartDateModel>>()!);
            mapperResolver.Register(serviceProvider.GetService<IMapper<EmployerViewPendingStartDateChangeModel>>()!);
            mapperResolver.Register(serviceProvider.GetService<IMapper<ProviderCancelStartDateModel>>()!);
            mapperResolver.Register(serviceProvider.GetService<IMapper<ProviderPlannedEndDateModel>>()!);
            return mapperResolver;
        });
    }
}
