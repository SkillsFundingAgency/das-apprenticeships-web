using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using SFA.DAS.Provider.Shared.UI.Models;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Apprenticeships.Web.Infrastructure;

/// <summary>
/// Authorization handler that evaluates if the provider is authorized to use the service
/// </summary>
/// <remarks>
/// If not authorized, then the user will be redirected to the PAS 401 page.
/// This requirement is overriden if the stub for provider validation is enabled.
/// </remarks>
[ExcludeFromCodeCoverage]
public class TrainingProviderAllRolesAuthorizationHandler : AuthorizationHandler<TrainingProviderAllRolesRequirement>
{
    private readonly ITrainingProviderAuthorizationHandler _handler;
    private readonly IConfiguration _configuration;
    private readonly ProviderSharedUIConfiguration _providerSharedUiConfiguration;

    public TrainingProviderAllRolesAuthorizationHandler(
        ITrainingProviderAuthorizationHandler handler,
        IConfiguration configuration,
        ProviderSharedUIConfiguration providerSharedUiConfiguration)
    {
        _handler = handler;
        _providerSharedUiConfiguration = providerSharedUiConfiguration;
        _configuration = configuration;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, TrainingProviderAllRolesRequirement requirement)
    {
        if (!context.User.HasClaim(c => c.Type.Equals(ProviderClaims.ProviderUkprn)))
        {
            context.Fail();
            return;
        }

        var claimValue = context.User.FindFirst(c => c.Type.Equals(ProviderClaims.ProviderUkprn))?.Value;

        if (!int.TryParse(claimValue, out _))
        {
            context.Fail();
            return;
        }

        var isStubProviderValidationEnabled = GetUseStubProviderValidationSetting();
        var currentContext = context.Resource switch
        {
            HttpContext resource => resource,
            AuthorizationFilterContext authorizationFilterContext => authorizationFilterContext.HttpContext,
            _ => null
        };

            
        if (!isStubProviderValidationEnabled && !(await _handler.IsProviderAuthorized(context)))
        {
            currentContext?.Response.Redirect($"{_providerSharedUiConfiguration.DashboardUrl}/error/403/invalid-status");
        }

        context.Succeed(requirement);
    }

    private bool GetUseStubProviderValidationSetting()
    {
        var value = _configuration.GetSection("UseStubProviderValidation").Value;

        return value != null && bool.TryParse(value, out var result) && result;
    }
}