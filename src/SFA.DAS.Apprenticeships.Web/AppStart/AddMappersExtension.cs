using SFA.DAS.Apprenticeships.Web.Models;

namespace SFA.DAS.Apprenticeships.Web.AppStart
{
    public static class AddMappersExtension
    {
        public static void AddMappers(this IServiceCollection services)
        {
            services.AddTransient<IMapper<CreateChangeOfPriceModel>, CreateChangeOfPriceModelMapper>();
            services.AddTransient<IMapper<EmployerChangeOfPriceModel>, EmployerChangeOfPriceModelMapper>();

            services.AddTransient<IMapper>((serviceProvider) =>
            {
                var mapperResolver = new MapperResolver();
                mapperResolver.Register(serviceProvider.GetService<IMapper<CreateChangeOfPriceModel>>()!);
                mapperResolver.Register(serviceProvider.GetService<IMapper<EmployerChangeOfPriceModel>>()!);
                return mapperResolver;
            });
        }
    }
}
