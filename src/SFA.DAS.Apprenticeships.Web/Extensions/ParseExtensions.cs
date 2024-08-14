namespace SFA.DAS.Apprenticeships.Web.Extensions;

public static class ParseExtensions
{
    public static string ValueOrSubstitute(this string? property, string substituteValue)
    {
        if (string.IsNullOrEmpty(property))
        {
            return substituteValue;
        }

        return property;

    }

    public static T ValueOrSubstitute<T>(this T? property, T substituteValue) where T : struct
    {
        if (!property.HasValue)
        {
            return substituteValue;
        }

        return property.Value;

    }

    public static T ValueOrThrow<T>(this T? property, string propertyName) where T : struct
    {
        if (!property.HasValue)
        {
            throw new ArgumentNullException($"The value of {propertyName} is null");
        }

        return property.Value;

    }
}