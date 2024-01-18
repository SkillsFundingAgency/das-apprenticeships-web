using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SFA.DAS.Apprenticeships.Infrastructure
{
	//  Its not intended for this class to remain in this project, it will be moved to a shared project
	public static class BearerTokenProvider
	{
		private static string _signingKey = string.Empty;
		private static int _expiryTime = 5;

		/// <summary>
		/// Set the bearer token signing key, must exceed 128bits in length
		/// </summary>
		public static void SetSigningKey(string? signingKey)
		{
			if (string.IsNullOrEmpty(signingKey))
			{
				throw new BearTokenException("Signing key cannot be null or empty");
			}

			const int minimumKeySize = 128;
			if (signingKey.Length * 8 < minimumKeySize)
			{
				// This checks the key is at least 128 bits long, otherwise the token will fail to be generated
				throw new BearTokenException("Signing key must exceed 128bits in length");
			}

			_signingKey = signingKey;
		}

		/// <summary>
		/// (Optional) Set bearer token expiry time, default is 5 minutes 
		/// </summary>
		public static void SetExpiryTime(int expiryTime)
		{
			_expiryTime = expiryTime;
		}

		public static string GetBearerToken(this HttpContext httpContext)
		{
			if (string.IsNullOrEmpty(_signingKey))
			{
				throw new BearTokenException("Signing key must be set before a token can be retrieved. This should ideally be done in startup");
			}

			var user = httpContext.User;
			if (!IsUserAuthenticated(user))
				throw new BearTokenException("User is not authenticated, cannot create token");

			var key = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(_signingKey));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

			var token = new JwtSecurityToken(
				claims: user.Claims,
				signingCredentials: creds,
				expires: DateTime.UtcNow.AddMinutes(_expiryTime)
			);

			return new JwtSecurityTokenHandler().WriteToken(token);

		}

		private static bool IsUserAuthenticated(ClaimsPrincipal? user)
		{
			if (user == null) return false;

			if (user.Identity == null) return false;

			return user.Identity.IsAuthenticated;
		}
	}

	// Although this could be in a seperate file, all the code used here will get moved to a seperate shared project
	public class BearTokenException : Exception
	{
		public BearTokenException(string message) : base(message)
		{
		}
	}
}
