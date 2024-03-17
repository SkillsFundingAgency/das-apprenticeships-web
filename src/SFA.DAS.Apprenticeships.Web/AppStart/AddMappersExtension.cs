using SFA.DAS.Apprenticeships.Web.Models;
using SFA.DAS.Apprenticeships.Web.Models.ChangeOfPrice;

namespace SFA.DAS.Apprenticeships.Web.AppStart
{
    public static class AddMappersExtension
    {
        public static void AddMappers(this IServiceCollection services)
        {
            services.AddTransient<IMapper<ProviderChangeOfPriceModel>, ProviderChangeOfPriceModelMapper>();
            services.AddTransient<IMapper<EmployerChangeOfPriceModel>, EmployerChangeOfPriceModelMapper>();
			services.AddTransient<IMapper<EmployerViewPendingPriceChangeModel>, EmployerViewPendingPriceChangeModelMapper>();
			services.AddTransient<IMapper<EmployerCancelPriceChangeModel>, EmployerCancelPriceChangeModelMapper>();
            services.AddTransient<IMapper<ProviderCancelPriceChangeModel>, ProviderCancelPriceChangeModelMapper>();


            services.AddTransient<IMapper>((serviceProvider) =>
            {
                var mapperResolver = new MapperResolver();
                mapperResolver.Register(serviceProvider.GetService<IMapper<ProviderChangeOfPriceModel>>()!);
                mapperResolver.Register(serviceProvider.GetService<IMapper<EmployerChangeOfPriceModel>>()!);
				mapperResolver.Register(serviceProvider.GetService<IMapper<EmployerViewPendingPriceChangeModel>>()!);
				mapperResolver.Register(serviceProvider.GetService<IMapper<EmployerCancelPriceChangeModel>>()!);
                mapperResolver.Register(serviceProvider.GetService<IMapper<ProviderCancelPriceChangeModel>>()!);
                return mapperResolver;
            });
        }
    }
}
