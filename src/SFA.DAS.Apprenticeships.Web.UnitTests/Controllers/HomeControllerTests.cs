using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Apprenticeships.Web.Controllers;
using System.Security.Claims;

namespace SFA.DAS.Apprenticeships.Web.UnitTests.Controllers
{
	public class HomeControllerTests
	{

		[Test]
		public async Task BeginSigningOut_ReturnsSignOutResult()
		{
			// Arrange
			var idToken = "test_id_token";
			var homeController = new HomeController();
			MockTokenInControllerContext(homeController, idToken);

			// Act
			var result = await homeController.BeginSigningOut();

			// Assert
			Assert.IsInstanceOf<SignOutResult>(result);
			var signOutResult = result as SignOutResult;
			Assert.That(signOutResult!.Properties!.Parameters["id_token"], Is.EqualTo(idToken));
		}

		private void MockTokenInControllerContext(HomeController controller, string token)
		{
			var mockHttpContext = new Mock<HttpContext>();
			var mockServiceProvider = new Mock<IServiceProvider>();
			var mockAuthenticationService = new Mock<IAuthenticationService>();
			var authenticationResult = AuthenticateResult.Success(
				new AuthenticationTicket(new ClaimsPrincipal(), 
				new AuthenticationProperties(new Dictionary<string, string?>{{ ".Token.id_token", token } }), 
				CookieAuthenticationDefaults.AuthenticationScheme));

			mockAuthenticationService.Setup(m=>m.AuthenticateAsync(It.IsAny<HttpContext>(), It.IsAny<string>())).ReturnsAsync(authenticationResult);
			mockServiceProvider.Setup(m => m.GetService(It.IsAny<Type>())).Returns(mockAuthenticationService.Object);
			mockHttpContext.Setup(m => m.RequestServices).Returns(mockServiceProvider.Object);

			controller.ControllerContext.HttpContext = mockHttpContext.Object;
		}
	}
}
