using SFA.DAS.Apprenticeships.Web.Extensions;
using SFA.DAS.Apprenticeships.Web.Infrastructure;
using SFA.DAS.Apprenticeships.Web.Models;

namespace SFA.DAS.Apprenticeships.Web.Helpers
{
    public static class RouteValuesHelper
    {
        public static void PopulateRouteValues(IRouteValuesProvider model, HttpContext context)
        {
            model.ApprenticeshipHashedId = context.GetRouteValueString(RouteValues.ApprenticeshipHashedId);
            model.ProviderReferenceNumber = long.Parse(context.GetRouteValueString(RouteValues.Ukprn));
        }
    }
}
