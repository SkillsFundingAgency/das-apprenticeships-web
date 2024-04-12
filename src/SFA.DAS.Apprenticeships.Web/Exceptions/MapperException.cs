using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Apprenticeships.Web.Exceptions;

[ExcludeFromCodeCoverage]
public class MapperException : Exception
{
    public MapperException(string message) : base(message)
    {
    }
}
