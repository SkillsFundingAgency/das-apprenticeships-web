using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Apprenticeships.Web.Controllers;

namespace SFA.DAS.Apprenticeships.Web.UnitTests.Controllers
{
    public class HomeControllerTests
    {
        [Test]
        public void WhenCreatePriceChangeRequestThen()
        {
            // Arrange
            var sut = CreateController();

            // Act
            var result = (ViewResult) sut.CreatePriceChangeRequest();

            // Assert
            //
        }

        private static ChangeOfPriceController CreateController()
        {
            var loggerMock = new Mock<ILogger<ChangeOfPriceController>>();
            return new ChangeOfPriceController(loggerMock.Object);
        }
    }
}