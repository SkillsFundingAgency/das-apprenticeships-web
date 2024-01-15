using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Apprenticeships.Web.Infrastructure;
using SFA.DAS.Apprenticeships.Web.Models;
using SFA.DAS.GovUK.Auth.Models;
using SFA.DAS.GovUK.Auth.Services;

namespace SFA.DAS.Apprenticeships.Web.Controllers;

[Route("[controller]")]
public class ServiceController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly IStubAuthenticationService _stubAuthenticationService;

    public ServiceController(IConfiguration configuration, IStubAuthenticationService stubAuthenticationService)
    {
        _configuration = configuration;
        _stubAuthenticationService = stubAuthenticationService;
    }
    
    [Route("signout", Name = RouteNames.SignOut)]
    public async Task<IActionResult> SignOut()
    {
        var idToken = await HttpContext.GetTokenAsync("id_token");

        var authenticationProperties = new AuthenticationProperties();
        authenticationProperties.Parameters.Clear();
        authenticationProperties.Parameters.Add("id_token",idToken);

        var schemes = new List<string>
        {
            CookieAuthenticationDefaults.AuthenticationScheme
        };
        _ = bool.TryParse(_configuration["StubAuth"], out var stubAuth);
        if (!stubAuth)
        {
            schemes.Add(OpenIdConnectDefaults.AuthenticationScheme);
        }
        
        return SignOut(
            authenticationProperties, 
            schemes.ToArray());
    }

    [AllowAnonymous]
    [Route("user-signed-out", Name = RouteNames.SignedOut)]
    [HttpGet]
    public IActionResult SignedOut()
    {
        return View("SignedOut", new SignedOutViewModel(_configuration["ResourceEnvironmentName"]));
    }

#if DEBUG
    [AllowAnonymous]
    [HttpGet]
    [Route("SignIn-Stub")]
    public IActionResult SigninStub()
    {
        return View("SigninStub", new List<string> { _configuration["ApprenticeshipsWeb:StubId"], _configuration["ApprenticeshipsWeb:StubEmail"] });
    }
    [AllowAnonymous]
    [HttpPost]
    [Route("SignIn-Stub")]
    public async Task<IActionResult> SigninStubPost()
    {
        var claims = await _stubAuthenticationService.GetStubSignInClaims(new StubAuthUserDetails
        {
            Email = _configuration["ApprenticeshipsWeb:StubEmail"],
            Id = _configuration["ApprenticeshipsWeb:StubId"]
        });

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claims,
            new AuthenticationProperties());

        return RedirectToRoute("Signed-in-stub");
    }

    [Authorize]
    [HttpGet]
    [Route("signed-in-stub", Name = "Signed-in-stub")]
    public IActionResult SignedInStub()
    {
        return View();
    }
#endif
}