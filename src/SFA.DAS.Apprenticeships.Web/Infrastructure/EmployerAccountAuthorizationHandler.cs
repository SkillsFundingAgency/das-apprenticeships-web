using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using SFA.DAS.Apprenticeships.Domain.Employers;
using SFA.DAS.Apprenticeships.Domain.Interfaces;

namespace SFA.DAS.Apprenticeships.Web.Infrastructure;

/// <summary>
/// Authorization handler that evaluates whether the EmployerAccountId in the claim of the authenticated Provider matches that of any incoming requests.
/// </summary>
[ExcludeFromCodeCoverage]
public class EmployerAccountAuthorizationHandler: AuthorizationHandler<EmployerAccountRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IEmployerAccountService _accountsService;
    private readonly ILogger<EmployerAccountAuthorizationHandler> _logger;

    public EmployerAccountAuthorizationHandler(IHttpContextAccessor httpContextAccessor, IEmployerAccountService accountsService, ILogger<EmployerAccountAuthorizationHandler> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _accountsService = accountsService;
        _logger = logger;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, EmployerAccountRequirement requirement)
    {
        if (!IsEmployerAuthorized(context, false))
        {
            return Task.CompletedTask;
        }

        context.Succeed(requirement);

        return Task.CompletedTask;
    }

    private bool IsEmployerAuthorized(AuthorizationHandlerContext context, bool allowAllUserRoles)
    {
        if (_httpContextAccessor.HttpContext != null && !_httpContextAccessor.HttpContext.Request.RouteValues.ContainsKey(RouteValues.EmployerAccountId))
        {
            return false;
        }
        var accountIdFromUrl = _httpContextAccessor.HttpContext?.Request.RouteValues[RouteValues.EmployerAccountId]?.ToString()?.ToUpper();
        var employerAccountClaim = context.User.FindFirst(c=>c.Type.Equals(EmployerClaims.AccountsClaimsTypeIdentifier));

        if(employerAccountClaim?.Value == null)
            return false;

        Dictionary<string, EmployerUserAccountItem> employerAccounts;

        try
        {
            employerAccounts = JsonConvert.DeserializeObject<Dictionary<string, EmployerUserAccountItem>>(employerAccountClaim.Value)!;
        }
        catch (JsonSerializationException e)
        {
            _logger.LogError(e, "Could not deserialize employer account claim for user");
            return false;
        }

        EmployerUserAccountItem employerIdentifier;

        if (employerAccounts != null && employerAccounts.ContainsKey(accountIdFromUrl!))
        {
            employerIdentifier =  employerAccounts[accountIdFromUrl!];
        }
        else
        {
            const string requiredIdClaim = ClaimTypes.NameIdentifier;
                
            if (!context.User.HasClaim(c => c.Type.Equals(requiredIdClaim)))
                return false;
                
            var userClaim = context.User.Claims.First(c => c.Type.Equals(requiredIdClaim));
            var userId = userClaim.Value;
            var email = context.User.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Email))?.Value;

            var accounts = _accountsService.GetUserAccounts(userId, email!).Result;
            var accountsAsJson = JsonConvert.SerializeObject(accounts.EmployerAccounts.ToDictionary(k => k.AccountId));
            var associatedAccountsClaim = new Claim(EmployerClaims.AccountsClaimsTypeIdentifier, accountsAsJson, JsonClaimValueTypes.Json);
            var updatedEmployerAccounts = JsonConvert.DeserializeObject<Dictionary<string, EmployerUserAccountItem>>(associatedAccountsClaim.Value);
            userClaim.Subject?.AddClaim(associatedAccountsClaim);
                
            if (updatedEmployerAccounts == null || !updatedEmployerAccounts.ContainsKey(accountIdFromUrl!))
            {
                return false;
            }

            employerIdentifier = updatedEmployerAccounts[accountIdFromUrl!];
        }

        if (!_httpContextAccessor.HttpContext!.Items.ContainsKey(ContextItemKeys.EmployerIdentifier))
        {
            _httpContextAccessor.HttpContext.Items.Add(ContextItemKeys.EmployerIdentifier, employerAccounts!.GetValueOrDefault(accountIdFromUrl));
        }

        return CheckUserRoleForAccess(employerIdentifier, allowAllUserRoles);
    }

    private static bool CheckUserRoleForAccess(EmployerUserAccountItem employerIdentifier, bool allowAllUserRoles)
    {
        if (!Enum.TryParse<EmployerUserRole>(employerIdentifier.Role, true, out var userRole))
        {
            return false;
        }

        return allowAllUserRoles || userRole == EmployerUserRole.Owner;
    }
}