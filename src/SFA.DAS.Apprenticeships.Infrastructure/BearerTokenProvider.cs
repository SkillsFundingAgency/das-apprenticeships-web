﻿using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SFA.DAS.Apprenticeships.Infrastructure;

public static class BearerTokenProvider
{
    private static string _signingKey = string.Empty;
    private const int _expiryTimeMinutes = 5;

    /// <summary>
    /// Set the bearer token signing key, must exceed 128bits in length
    /// </summary>
    public static void SetSigningKey(string? signingKey)
    {
        if (string.IsNullOrEmpty(signingKey))
        {
            throw new BearerTokenException("Signing key cannot be null or empty");
        }

        const int minimumKeySize = 128;
        if (signingKey.Length * 8 < minimumKeySize)
        {
            // This checks the key is at least 128 bits long, otherwise the token will fail to be generated
            throw new BearerTokenException("Signing key must exceed 128bits in length");
        }

        _signingKey = signingKey;
    }

    public static string GetBearerToken(this HttpContext httpContext)
    {
        if (string.IsNullOrEmpty(_signingKey))
        {
            throw new BearerTokenException("Signing key must be set before a token can be retrieved. This should ideally be done in startup");
        }

        var user = httpContext.User;
        if (!IsUserAuthenticated(user))
            throw new BearerTokenException("User is not authenticated, cannot create token");

        var key = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(_signingKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

        var token = new JwtSecurityToken(
            claims: user.Claims,
            signingCredentials: creds,
            expires: DateTime.UtcNow.AddMinutes(_expiryTimeMinutes)
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

public class BearerTokenException : Exception
{
    public BearerTokenException(string message) : base(message)
    {
    }
}