using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.Apprenticeships.Web.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        [Route("signout", Name = "provider-signout")]
        public async Task<IActionResult> BeginSigningOut()
        {
            var idToken = await HttpContext.GetTokenAsync("id_token");

            var authenticationProperties = new AuthenticationProperties();
            authenticationProperties.Parameters.Clear();
            authenticationProperties.Parameters.Add("id_token", idToken);
            return SignOut(
                authenticationProperties,
                authenticationSchemes: new[] 
                {
	                CookieAuthenticationDefaults.AuthenticationScheme,
					OpenIdConnectDefaults.AuthenticationScheme
                }
            );
        }
    }
}
