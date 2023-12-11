using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace SFA.DAS.Apprenticeships.Web.UnitTests.Controllers
{
	public class AllControllersTests
	{
		/// <summary>
		/// This Test ensures that all controllers have the ValidateAntiForgeryToken attribute on all HttpPost, HttpPut and HttpDelete methods
		/// </summary>
		[Test]
		public void VerifyAntiForgeryTokenAttributeIsPresent()
		{
			var controllers = Assembly.GetAssembly(typeof(SFA.DAS.Apprenticeships.Web.Controllers.ChangeOfPriceController))!
				.GetTypes()
				.Where(type => type.IsSubclassOf(typeof(Controller)));

			foreach (var controller in controllers)
			{
				var methods = controller.GetMethods(BindingFlags.Public | BindingFlags.Instance)
					.Where(m => m.GetCustomAttributes(typeof(HttpPostAttribute), false).Any() ||
								m.GetCustomAttributes(typeof(HttpPutAttribute), false).Any() ||
								m.GetCustomAttributes(typeof(HttpDeleteAttribute), false).Any());

				foreach (var method in methods)
				{
					Assert.IsTrue(method.GetCustomAttributes(typeof(ValidateAntiForgeryTokenAttribute), false).Any(),
						$"Method {method.Name} in {controller.Name} does not have ValidateAntiForgeryToken attribute");
				}
			}
		}
	}
}
