using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;
using SFA.DAS.Apprenticeships.Domain.Interfaces;
using SFA.DAS.Apprenticeships.Web.Controllers.ChangeOfStartDate;
using SFA.DAS.Apprenticeships.Web.Models;
using SFA.DAS.Apprenticeships.Web.Models.ChangeOfStartDate;
using SFA.DAS.Apprenticeships.Web.Services;
using SFA.DAS.Apprenticeships.Web.UnitTests.TestHelpers;
using SFA.DAS.Provider.Shared.UI.Extensions;
using SFA.DAS.Provider.Shared.UI.Models;

namespace SFA.DAS.Apprenticeships.Web.UnitTests.Controllers.ChangeOfStartDate
{
    [TestFixture]
    public class ChangeOfStartDateProviderControllerTests
    {
        #pragma warning disable CS8618
        private Fixture _fixture;
        private Mock<ILogger<ChangeOfStartDateProviderController>> _mockLogger;
        private Mock<IApprenticeshipService> _mockApprenticeshipService;
        private Mock<IMapper> _mockMapper;
        private Mock<IExternalUrlHelper> _mockExternalUrlHelper;
        private Mock<ICacheService> _mockCacheService;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _mockLogger = new Mock<ILogger<ChangeOfStartDateProviderController>>();
            _mockApprenticeshipService = new Mock<IApprenticeshipService>();
            _mockMapper = new Mock<IMapper>();
            _mockExternalUrlHelper = new Mock<IExternalUrlHelper>();
            _mockCacheService = new Mock<ICacheService>();
        }

        [Test]
        public async Task GetProviderEnterChangeDetails_ReturnsMappedModel()
        {
            // Arrange
            var apprenticeshipHashedId = _fixture.Create<string>();
            var apprenticeshipKey = Guid.NewGuid();
            var expectedModel = _fixture.Create<ProviderChangeOfStartDateModel>();
            var controller = new ChangeOfStartDateProviderController(
                _mockLogger.Object, _mockApprenticeshipService.Object,
                _mockMapper.Object, _mockCacheService.Object, _mockExternalUrlHelper.Object);
            _mockMapper.Setup(m => m.Map<ProviderChangeOfStartDateModel>(It.IsAny<ApprenticeshipStartDate>()))
                .Returns(expectedModel);
            _mockApprenticeshipService.Setup(m => m.GetApprenticeshipKey(apprenticeshipHashedId))
                .ReturnsAsync(apprenticeshipKey);
            _mockApprenticeshipService.Setup(m => m.GetApprenticeshipStartDate(apprenticeshipKey))
                .ReturnsAsync(new ApprenticeshipStartDate { ApprenticeshipKey = apprenticeshipKey });

            controller.SetupHttpContext(_fixture.Create<long>(), apprenticeshipHashedId);

            // Act
            var result = await controller.GetProviderEnterChangeDetails(apprenticeshipHashedId);

            // Assert
            var viewResult = result.ShouldBeOfType<ViewResult>();
            viewResult.ViewName.Should().Be(ChangeOfStartDateProviderController.EnterChangeDetailsViewName);
            viewResult.Model.Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public async Task GetProviderEnterChangeDetails_ReturnsNotFound_WhenApprenticeshipKeyNotFound()
        {
            // Arrange
            var apprenticeshipHashedId = _fixture.Create<string>();
            var controller = new ChangeOfStartDateProviderController(
                _mockLogger.Object, _mockApprenticeshipService.Object,
                _mockMapper.Object, _mockCacheService.Object, _mockExternalUrlHelper.Object);
            _mockApprenticeshipService.Setup(m => m.GetApprenticeshipKey(apprenticeshipHashedId))
                .ReturnsAsync(Guid.Empty);

            // Act
            var result = await controller.GetProviderEnterChangeDetails(apprenticeshipHashedId);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Test]
        public async Task GetProviderEnterChangeDetails_ReturnsNotFound_WhenApprenticeshipStartDateNotFound()
        {
            // Arrange
            var apprenticeshipHashedId = _fixture.Create<string>();
            var apprenticeshipKey = Guid.NewGuid();
            var controller = new ChangeOfStartDateProviderController(
                _mockLogger.Object, _mockApprenticeshipService.Object,
                _mockMapper.Object, _mockCacheService.Object, _mockExternalUrlHelper.Object);
            _mockApprenticeshipService.Setup(m => m.GetApprenticeshipKey(apprenticeshipHashedId))
                .ReturnsAsync(apprenticeshipKey);
            _mockApprenticeshipService.Setup(m => m.GetApprenticeshipStartDate(apprenticeshipKey))!
                .ReturnsAsync((ApprenticeshipStartDate)null!);

            // Act
            var result = await controller.GetProviderEnterChangeDetails(apprenticeshipHashedId);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Test]
        public async Task ProviderCheckDetails_ValidModel_ReturnsCheckDetailsView()
        {
            // Arrange
            var controller = new ChangeOfStartDateProviderController(
                _mockLogger.Object, 
                _mockApprenticeshipService.Object,
                _mockMapper.Object, 
                _mockCacheService.Object, 
                _mockExternalUrlHelper.Object);
                _mockMapper.Setup(m => m.Map<ProviderChangeOfStartDateModel>(It.IsAny<ApprenticeshipStartDate>()));

            var model = _fixture.Create<ProviderChangeOfStartDateModel>();

            controller.SetupHttpContext(_fixture.Create<long>(), _fixture.Create<string>());

            // Act
            var result = await controller.ProviderCheckDetails(model);

            // Assert
            var viewResult = result.ShouldBeOfType<ViewResult>();
            viewResult.ViewName.Should().Be(ChangeOfStartDateProviderController.CheckDetailsViewName);
            viewResult.Model.Should().BeEquivalentTo(model);
        }

        [Test]
        public async Task ProviderCheckDetails_InValidModel_ReturnsEnterChangeDetailsViewName()
        {
            // Arrange
            var controller = new ChangeOfStartDateProviderController(
                _mockLogger.Object,
                _mockApprenticeshipService.Object,
                _mockMapper.Object,
                _mockCacheService.Object,
                _mockExternalUrlHelper.Object);
            _mockMapper.Setup(m => m.Map<ProviderChangeOfStartDateModel>(It.IsAny<ApprenticeshipStartDate>()));

            var model = _fixture.Create<ProviderChangeOfStartDateModel>();

            controller.SetupHttpContext(_fixture.Create<long>(), _fixture.Create<string>());
            controller.ModelState.AddModelError("key", "error message");

            // Act
            var result = await controller.ProviderCheckDetails(model);

            // Assert
            var viewResult = result.ShouldBeOfType<ViewResult>();
            viewResult.ViewName.Should().Be(ChangeOfStartDateProviderController.EnterChangeDetailsViewName);
            viewResult.Model.Should().BeEquivalentTo(model);
        }

        [Test]
        public async Task ProviderSubmitChangeDetails_ValidModel_Redirects()
        {
            // Arrange
            var controller = new ChangeOfStartDateProviderController(
                _mockLogger.Object,
                _mockApprenticeshipService.Object,
                _mockMapper.Object,
                _mockCacheService.Object,
                _mockExternalUrlHelper.Object);
            _mockMapper.Setup(m => m.Map<ProviderChangeOfStartDateModel>(It.IsAny<ApprenticeshipStartDate>()));

            var expectedRedirectUrl = _fixture.Create<string>();
            _mockExternalUrlHelper.Setup(m=>m.GenerateUrl(It.IsAny<UrlParameters>())).Returns(expectedRedirectUrl);
            var model = _fixture.Create<ProviderChangeOfStartDateModel>();

            controller.SetupHttpContext(_fixture.Create<long>(), _fixture.Create<string>(), _fixture.Create<string>());

            // Act
            var result = await controller.ProviderSubmitChangeDetails(model);

            // Assert
            var redirectResult = result.ShouldBeOfType<RedirectResult>();
            redirectResult.Url.Should().Be(expectedRedirectUrl);
        }

    }
}
