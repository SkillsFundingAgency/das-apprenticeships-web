#pragma warning disable CS8618 // Non-nullable - This class follows pattern of other configuration classes
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Apprenticeships.Infrastructure.Configuration
{
	[ExcludeFromCodeCoverage]
	public class CacheConfiguration
	{
		public string DefaultCache { get; set; }
		public string CacheConnection { get; set; }
		public int ExpirationInMinutes { get; set; }
    }
}
#pragma warning restore CS8618 