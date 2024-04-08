using AutoFixture;
using FluentAssertions;
using Moq;
using SFA.DAS.Apprenticeships.Application.Exceptions;
using SFA.DAS.Apprenticeships.Application.Services;
using SFA.DAS.Apprenticeships.Domain.Api;
using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;
using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api.Requests;
using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api.Responses;
using SFA.DAS.Apprenticeships.Domain.Interfaces;

namespace SFA.DAS.Apprenticeships.Application.UnitTests.Services
{
    [TestFixture]
    public class ApprenticeshipServiceTests
    {
        private Mock<IApiClient> _apiClientMock;
        private ApprenticeshipService _apprenticeshipService;
        private Fixture _fixture;

        public ApprenticeshipServiceTests()
        {
            _fixture = new Fixture();
        }

        [SetUp]
        public void SetUp()
        {
            _apiClientMock = new Mock<IApiClient>();
            _apprenticeshipService = new ApprenticeshipService(_apiClientMock.Object);
        }

        [Test]
        public async Task GetApprenticeshipKey_WhenCalled_ReturnsApprenticeshipKey()
        {
            // Arrange
            var apprenticeshipHashId = "hashId";
            var expectedKey = _fixture.Create<Guid>();
            var response = new ApiResponse<Guid>(expectedKey, System.Net.HttpStatusCode.Accepted, string.Empty);
            _apiClientMock.Setup(x => x.Get<Guid>(It.IsAny<GetApprenticeshipKeyRequest>())).ReturnsAsync(response);

            // Act
            var result = await _apprenticeshipService.GetApprenticeshipKey(apprenticeshipHashId);

            // Assert
            result.Should().Be(expectedKey);
        }

        [Test]
        public async Task GetApprenticeshipPrice_WhenCalled_ReturnsApprenticeshipPrice()
        {
            // Arrange
            var apprenticeshipKey = _fixture.Create<Guid>();
            var expectedPrice = _fixture.Create<ApprenticeshipPrice>();
            var response = new ApiResponse<ApprenticeshipPrice>( expectedPrice, System.Net.HttpStatusCode.Accepted, string.Empty);
            _apiClientMock.Setup(x => x.Get<ApprenticeshipPrice>(It.IsAny<GetApprenticeshipPriceRequest>())).ReturnsAsync(response);

            // Act
            var result = await _apprenticeshipService.GetApprenticeshipPrice(apprenticeshipKey);

            // Assert
            result.Should().Be(expectedPrice);
        }

        [Test]
        public async Task GetPendingPriceChange_WhenCalled_ReturnsPendingPriceChange()
        {
            // Arrange
            var apprenticeshipKey = _fixture.Create<Guid>();
            var expectedPrice = _fixture.Create<GetPendingPriceChangeResponse>();
            var response = new ApiResponse<GetPendingPriceChangeResponse>(expectedPrice, System.Net.HttpStatusCode.Accepted, string.Empty);
            _apiClientMock.Setup(x => x.Get<GetPendingPriceChangeResponse>(It.IsAny<GetPendingPriceChangeRequest>())).ReturnsAsync(response);

            // Act
            var result = await _apprenticeshipService.GetPendingPriceChange(apprenticeshipKey);

            // Assert
            result.Should().Be(expectedPrice);
        }

        [Test]
        public async Task CancelPendingPriceChange_WhenCalled_DeletesPendingPriceChange()
        {
            // Arrange
            var apprenticeshipKey = _fixture.Create<Guid>();
            var response = new ApiResponse<object>(_fixture.Create<string>(), System.Net.HttpStatusCode.OK, "");
            _apiClientMock.Setup(x => x.Delete<object>(It.IsAny<CancelPendingPriceChangeRequest>())).ReturnsAsync(response);

            // Act
            await _apprenticeshipService.CancelPendingPriceChange(apprenticeshipKey);

            // Assert
            _apiClientMock.Verify(x => x.Delete<object>(It.IsAny<CancelPendingPriceChangeRequest>()), Times.Once);
        }

        [Test]
        public async Task RejectPendingPriceChange_WhenCalled_PatchesPendingPriceChange()
        {
            // Arrange
            var apprenticeshipKey = _fixture.Create<Guid>();

            // Act
            await _apprenticeshipService.RejectPendingPriceChange(apprenticeshipKey, _fixture.Create<string>());

            // Assert
            _apiClientMock.Verify(x => x.Patch<object>(It.IsAny<RejectPendingPriceChangeRequest>()), Times.Once);
        }

		[Test]
		public async Task ApprovePendingPriceChange_WhenCalled_PatchesPendingPriceChange()
		{
			// Arrange
			var apprenticeshipKey = _fixture.Create<Guid>();

			// Act
			await _apprenticeshipService.ApprovePendingPriceChange(apprenticeshipKey, _fixture.Create<string>());

			// Assert
			_apiClientMock.Verify(x => x.Patch<object>(It.IsAny<ApprovePendingPriceChangeRequest>()), Times.Once);
		}

        [Test]
        public async Task ApprovePendingPriceChange_Provider_WhenCalled_PatchesPendingPriceChange()
        {
            // Arrange
            var apprenticeshipKey = _fixture.Create<Guid>();

            // Act
            await _apprenticeshipService.ApprovePendingPriceChange(apprenticeshipKey, _fixture.Create<string>(), _fixture.Create<decimal>(), _fixture.Create<decimal>());

            // Assert
            _apiClientMock.Verify(x => x.Patch<object>(It.IsAny<ApprovePendingPriceChangeRequest>()), Times.Once);
        }

        [Test]
        public async Task CreateStartDateChange_WhenCalled_PostsRequest()
        {
            // Arrange
            var apprenticeshipKey = _fixture.Create<Guid>();
            var data = _fixture.Create<CreateChangeOfStartDateData>();
            _apiClientMock.Setup(x => x.Post<object>(It.IsAny<CreateChangeOfStartDateRequest>())).ReturnsAsync(new ApiResponse<object>(string.Empty, System.Net.HttpStatusCode.OK, ""));

            // Act
            await _apprenticeshipService.CreateStartDateChange(
                apprenticeshipKey, data.Initiator, data.UserId, data.Reason, data.ActualStartDate);

            // Assert
            _apiClientMock.Verify(x => x.Post<object>(It.IsAny<CreateChangeOfStartDateRequest>()), Times.Once);
        }

        [Test]
        public void CreateStartDateChange_WhenCalled_ThrowsException()
        {
            // Arrange
            var apprenticeshipKey = _fixture.Create<Guid>();
            var data = _fixture.Create<CreateChangeOfStartDateData>();
            _apiClientMock.Setup(x => x.Post<object>(It.IsAny<CreateChangeOfStartDateRequest>())).ReturnsAsync(new ApiResponse<object>(string.Empty, System.Net.HttpStatusCode.OK, "Error"));

            // Act / Assert
            Assert.ThrowsAsync<ServiceException>(() => _apprenticeshipService.CreateStartDateChange(
                               apprenticeshipKey, data.Initiator, data.UserId, data.Reason, data.ActualStartDate));
        }

    }
}
