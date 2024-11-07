using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Azure.KeyVault.Models;

namespace SFA.DAS.Apprenticeships.Web.Extensions;

public static class ModelStateExtensions
{
    public static string GetErrorSummary(this ModelStateDictionary modelState)
    {
        return modelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).Aggregate((current, next) => current + " " + next);
    }
}
