﻿using SFA.DAS.Apprenticeships.Web.Infrastructure;
using SFA.DAS.Apprenticeships.Web.Models;

namespace SFA.DAS.Apprenticeships.Web.Extensions;

public static class HttpContextExtension
{
    public static string GetRouteValueString(this HttpContext context, string routeValueName)
    {
        var routeValue = context.Request.RouteValues[routeValueName] as string;

        if(string.IsNullOrEmpty(routeValue))
        {
            throw new ArgumentException($"Route value {routeValueName} is null or empty");
        }

        return routeValue;
    }

    public static void PopulateEmployerInitiatedRouteValues(this HttpContext context, IRouteValuesEmployer model)
    {
        model.ApprenticeshipHashedId = context.GetRouteValueString(RouteValues.ApprenticeshipHashedId);
        model.EmployerAccountId = context.GetRouteValueString(RouteValues.EmployerAccountId);
    }
}
