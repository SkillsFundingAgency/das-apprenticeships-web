#pragma warning disable CS8618 // Non-nullable - This class follows pattern of other configuration classes

using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Apprenticeships.Infrastructure.Configuration;

[ExcludeFromCodeCoverage]
public class DataProtection
{
	public string DataProtectionKeysDatabase { get; set; }
	public string RedisConnectionString { get; set; }
}

#pragma warning restore CS8618