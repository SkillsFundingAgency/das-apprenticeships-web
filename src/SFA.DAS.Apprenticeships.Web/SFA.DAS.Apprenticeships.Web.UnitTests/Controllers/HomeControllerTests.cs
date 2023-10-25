using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Apprenticeships.Web.Controllers;

namespace SFA.DAS.Apprenticeships.Web.UnitTests.Controllers
{
    public class HomeControllerTests
    {
        private Fixture _fixture;
        private Mock<ILogger<ChangeOfPriceController>> _loggerMock;
        private ChangeOfPriceController _sut;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
            _loggerMock = new Mock<ILogger<ChangeOfPriceController>>();

            _sut = new ChangeOfPriceController(_loggerMock.Object);
        }

        [Test]
        public async Task WhenCreatePriceChangeRequestThen()
        {
            // Arrange
            //

            // Act
            var result = (ViewResult) _sut.CreatePriceChangeRequest();

            // Assert
            //
        }
    }
}