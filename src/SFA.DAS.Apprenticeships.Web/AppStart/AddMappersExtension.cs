using SFA.DAS.Apprenticeships.Web.Models;

namespace SFA.DAS.Apprenticeships.Web.AppStart
{
    public static class AddMappersExtension
    {
        public static void AddMappers(this IServiceCollection services)
        {
            services.AddTransient<IMapper<CreateChangeOfPriceModel>, CreateChangeOfPriceModelMapper>();
        }
    }
}
