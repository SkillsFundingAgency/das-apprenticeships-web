using Microsoft.AspNetCore.Mvc.Rendering;

namespace SFA.DAS.Apprenticeships.Web.Extensions
{
    public static class UiExtensions
    {
        public static string FormatCurrency(this int value)
        {
            return value.ToString("C0");
        }

        public static string DisplayFormGroupError(this ViewContext viewContext, string key)
        {
            if (viewContext.ModelState[key]?.Errors.Any() == true)
            {
                return "govuk-form-group--error";
            }

            return string.Empty;
        }

        public static string DisplayInputError(this ViewContext viewContext, string key)
        {
            if (viewContext.ModelState[key]?.Errors.Any() == true)
            {
                return "govuk-input--error";
            }

            return string.Empty;
        }

        public static string StyleAsError(this ViewContext viewContext, string key)
        {
            if (viewContext.ModelState[key]?.Errors.Any() == true)
            {
                return "govuk-error-message field-validation-error";
            }

            return string.Empty;
        }

    }
}
