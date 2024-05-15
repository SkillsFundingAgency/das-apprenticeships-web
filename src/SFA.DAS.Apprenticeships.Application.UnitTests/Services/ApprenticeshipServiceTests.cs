using System.Net;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Apprenticeships.Application.Exceptions;
using SFA.DAS.Apprenticeships.Application.Services;
using SFA.DAS.Apprenticeships.Application.UnitTests.TestHelpers;
using SFA.DAS.Apprenticeships.Domain.Api;
using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api;
using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api.Requests;
using SFA.DAS.Apprenticeships.Domain.Apprenticeships.Api.Responses;
using SFA.DAS.Apprenticeships.Domain.Interfaces;

namespace SFA.DAS.Apprenticeships.Application.UnitTests.Services;

[TestFixture]
public class ApprenticeshipServiceTests
{
    private Mock<IApiClient> _apiClientMock;
    private Mock<ILogger<ApprenticeshipService>> _mockLogger;
    private ApprenticeshipService _apprenticeshipService;
    private readonly Fixture _fixture = new();

    [SetUp]
    public void SetUp()
    {
        _apiClientMock = new Mock<IApiClient>();
        _mockLogger = new Mock<ILogger<ApprenticeshipService>>();
        _apprenticeshipService = new ApprenticeshipService(_apiClientMock.Object, _mockLogger.Object);
    }

    [Test]
    public async Task GetApprenticeshipKey_WhenCalled_ReturnsApprenticeshipKey()
    {
        // Arrange
        var apprenticeshipHashId = "hashId";
        var expectedKey = _fixture.Create<Guid>();
        var response = new ApiResponse<Guid>(expectedKey, HttpStatusCode.Accepted, string.Empty);
        _apiClientMock.Setup(x => x.Get<Guid>(It.IsAny<GetApprenticeshipKeyRequest>())).ReturnsAsync(response);

        // Act
        var result = await _apprenticeshipService.GetApprenticeshipKey(apprenticeshipHashId);

        // Assert
        result.Should().Be(expectedKey);
    }

    [Test]
    public async Task GetApprenticeshipKey_WhenCalled_AndEmptyHashIdSend_LogsMessageAndReturnsEmptyGuid()
    {
        // Act
        var result = await _apprenticeshipService.GetApprenticeshipKey(string.Empty);

        // Assert
        result.Should().BeEmpty();
        _mockLogger.ShouldHaveLoggedMessage(LogLevel.Warning, "Cannot get apprenticeshipKey when apprenticeshipHashId is null or empty");
    }

    [Test]
    public async Task GetApprenticeshipKey_WhenCalled_AndNoKeyExists_LogsMessageAndReturnsEmptyGuid()
    {
        // Arrange
        var apprenticeshipHashId = "hashId";
        var response = new ApiResponse<Guid>(Guid.Empty, HttpStatusCode.Accepted, string.Empty);
        _apiClientMock.Setup(x => x.Get<Guid>(It.IsAny<GetApprenticeshipKeyRequest>())).ReturnsAsync(response);

        // Act
        var result = await _apprenticeshipService.GetApprenticeshipKey(apprenticeshipHashId);

        // Assert
        result.Should().BeEmpty();
        _mockLogger.ShouldHaveLoggedMessage(LogLevel.Warning, $"ApprenticeshipKey not found for apprenticeshipHashId {apprenticeshipHashId}");
    }

    [Test]
    public async Task GetApprenticeshipPrice_WhenCalled_ReturnsApprenticeshipPrice()
    {
        // Arrange
        var apprenticeshipHashId = _fixture.Create<string>();
        MockGetApprenticeshipKey(apprenticeshipHashId);

        var expectedPrice = _fixture.Create<ApprenticeshipPrice>();
        var response = new ApiResponse<ApprenticeshipPrice>(expectedPrice, HttpStatusCode.Accepted, string.Empty);
        _apiClientMock.Setup(x => x.Get<ApprenticeshipPrice>(It.IsAny<GetApprenticeshipPriceRequest>())).ReturnsAsync(response);

        // Act
        var result = await _apprenticeshipService.GetApprenticeshipPrice(apprenticeshipHashId);

        // Assert
        result.Should().Be(expectedPrice);
    }

    [Test]
    public async Task GetApprenticeshipPrice_WhenCalled_AndKeyNotFound_LogsMessageAndReturnsNull()
    {
        // Arrange
        var apprenticeshipHashId = _fixture.Create<string>();

        // Act
        var result = await _apprenticeshipService.GetApprenticeshipPrice(apprenticeshipHashId);

        // Assert
        result.Should().BeNull();
        _mockLogger.ShouldHaveLoggedMessage(LogLevel.Warning, $"ApprenticeshipKey not found for apprenticeshipHashId {apprenticeshipHashId}");

    }

    [Test]
    public async Task GetApprenticeshipPrice_WhenCalled_AndPriceNotFound_LogsMessageAndReturnsNull()
    {
        // Arrange
        var apprenticeshipHashId = _fixture.Create<string>();
        var apprenticeshipKey = _fixture.Create<Guid>();
        MockGetApprenticeshipKey(apprenticeshipHashId, apprenticeshipKey);

        // Act
        var result = await _apprenticeshipService.GetApprenticeshipPrice(apprenticeshipHashId);

        // Assert
        result.Should().BeNull();
        _mockLogger.ShouldHaveLoggedMessage(LogLevel.Warning, $"ApprenticeshipPrice not found for apprenticeshipKey {apprenticeshipKey}");

    }

    [Test]
    public async Task GetPendingPriceChange_WhenCalled_ReturnsPendingPriceChange()
    {
        // Arrange
        var apprenticeshipHashId = _fixture.Create<string>();
        MockGetApprenticeshipKey(apprenticeshipHashId);

        var expectedPrice = _fixture.Create<GetPendingPriceChangeResponse>();
        var response = new ApiResponse<GetPendingPriceChangeResponse>(expectedPrice, HttpStatusCode.Accepted, string.Empty);
        _apiClientMock.Setup(x => x.Get<GetPendingPriceChangeResponse>(It.IsAny<GetPendingPriceChangeRequest>())).ReturnsAsync(response);

        // Act
        var result = await _apprenticeshipService.GetPendingPriceChange(apprenticeshipHashId);

        // Assert
        result.Should().Be(expectedPrice);
    }

    [Test]
    public async Task GetPendingPriceChange_WhenCalled_AndKeyNotFound_LogsMessageAndReturnsNull()
    {
        // Arrange
        var apprenticeshipHashId = _fixture.Create<string>();

        // Act
        var result = await _apprenticeshipService.GetPendingPriceChange(apprenticeshipHashId);

        // Assert
        result.Should().BeNull();
        _mockLogger.ShouldHaveLoggedMessage(LogLevel.Warning, $"ApprenticeshipKey not found for apprenticeshipHashId {apprenticeshipHashId}");
    }

    [Test]
    public async Task GetPendingPriceChange_WhenCalled_AndPendingPriceNotFound_LogsMessageAndReturnsNull()
    {
        // Arrange
        var apprenticeshipHashId = _fixture.Create<string>();
        var apprenticeshipKey = _fixture.Create<Guid>();
        MockGetApprenticeshipKey(apprenticeshipHashId, apprenticeshipKey);

        // Act
        var result = await _apprenticeshipService.GetPendingPriceChange(apprenticeshipHashId);

        // Assert
        result.Should().BeNull();
        _mockLogger.ShouldHaveLoggedMessage(LogLevel.Warning, $"PendingPriceChange not found for apprenticeshipKey {apprenticeshipKey}");

    }

    [Test]
    public async Task CancelPendingPriceChange_WhenCalled_DeletesPendingPriceChange()
    {
        // Arrange
        var apprenticeshipKey = _fixture.Create<Guid>();
        var response = new ApiResponse<object>(_fixture.Create<string>(), HttpStatusCode.OK, "");
        _apiClientMock.Setup(x => x.Delete<object>(It.IsAny<CancelPendingPriceChangeRequest>())).ReturnsAsync(response);

        // Act
        await _apprenticeshipService.CancelPendingPriceChange(apprenticeshipKey);

        // Assert
        _apiClientMock.Verify(x => x.Delete<object>(It.IsAny<CancelPendingPriceChangeRequest>()), Times.Once);
    }

    [Test]
    public void CancelPendingPriceChange_WhenCalled_AndApiCallFails_ThrowsServiceException()
    {
        // Arrange
        var apprenticeshipKey = _fixture.Create<Guid>();
        var response = new ApiResponse<object>(_fixture.Create<string>(), HttpStatusCode.OK, "Error");
        _apiClientMock.Setup(x => x.Delete<object>(It.IsAny<CancelPendingPriceChangeRequest>())).ReturnsAsync(response);

        // Act / Assert
        Assert.ThrowsAsync<ServiceException>(() => _apprenticeshipService.CancelPendingPriceChange(apprenticeshipKey));
    }

    [Test]
    public async Task RejectPendingPriceChange_WhenCalled_PatchesPendingPriceChange()
    {
        // Arrange
        var apprenticeshipKey = _fixture.Create<Guid>();
        var response = new ApiResponse<object>(_fixture.Create<string>(), HttpStatusCode.OK, "");
        _apiClientMock.Setup(x => x.Patch<object>(It.IsAny<RejectPendingPriceChangeRequest>())).ReturnsAsync(response);

        // Act
        await _apprenticeshipService.RejectPendingPriceChange(apprenticeshipKey, _fixture.Create<string>());

        // Assert
        _apiClientMock.Verify(x => x.Patch<object>(It.IsAny<RejectPendingPriceChangeRequest>()), Times.Once);
    }

    [Test]
    public void RejectPendingPriceChange_WhenCalled_AndApiCallFails_ThrowsServiceException()
    {
        // Arrange
        var apprenticeshipKey = _fixture.Create<Guid>();
        var response = new ApiResponse<object>(_fixture.Create<string>(), HttpStatusCode.OK, "Error");
        _apiClientMock.Setup(x => x.Patch<object>(It.IsAny<RejectPendingPriceChangeRequest>())).ReturnsAsync(response);

        // Act / Assert
        Assert.ThrowsAsync<ServiceException>(() => _apprenticeshipService.RejectPendingPriceChange(apprenticeshipKey, _fixture.Create<string>()));

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
    public void ApprovePendingPriceChange_WhenCalled_AndApiCallFails_ThrowsServiceException()
    {
        // Arrange
        var apprenticeshipKey = _fixture.Create<Guid>();
        var response = new ApiResponse<object>(_fixture.Create<string>(), HttpStatusCode.OK, "Error");
        _apiClientMock.Setup(x => x.Patch<object>(It.IsAny<ApprovePendingPriceChangeRequest>())).ReturnsAsync(response);

        // Act / Assert
        Assert.ThrowsAsync<ServiceException>(() => _apprenticeshipService.ApprovePendingPriceChange(apprenticeshipKey, _fixture.Create<string>()));

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
    public void ApprovePendingPriceChange_Provider_WhenCalled_AndApiCallFails_ThrowsServiceException()
    {
        // Arrange
        var apprenticeshipKey = _fixture.Create<Guid>();
        var response = new ApiResponse<object>(_fixture.Create<string>(), HttpStatusCode.OK, "Error");
        _apiClientMock.Setup(x => x.Patch<object>(It.IsAny<ApprovePendingPriceChangeRequest>())).ReturnsAsync(response);

        // Act / Assert
        Assert.ThrowsAsync<ServiceException>(() => _apprenticeshipService.ApprovePendingPriceChange(apprenticeshipKey, _fixture.Create<string>(), _fixture.Create<decimal>(), _fixture.Create<decimal>()));

    }

    [Test]
    public async Task GetApprenticeshipStartDate_WhenCalled_ReturnsPendingStartDateChange()
    {
        // Arrange
        var apprenticeshipHashId = _fixture.Create<string>();

        MockGetApprenticeshipKey(apprenticeshipHashId);

        var expectedStartDate = _fixture.Create<ApprenticeshipStartDate>();
        var response = new ApiResponse<ApprenticeshipStartDate>(expectedStartDate, HttpStatusCode.Accepted, string.Empty);
        _apiClientMock.Setup(x => x.Get<ApprenticeshipStartDate>(It.IsAny<GetApprenticeshipStartDateRequest>())).ReturnsAsync(response);

        // Act
        var result = await _apprenticeshipService.GetApprenticeshipStartDate(apprenticeshipHashId);

        // Assert
        result.Should().Be(expectedStartDate);
    }

    [Test]
    public async Task GetPendingStartDateChange_WhenCalled_ReturnsPendingStartDateChange()
    {
        // Arrange
        var apprenticeshipHashId = _fixture.Create<string>();
        MockGetApprenticeshipKey(apprenticeshipHashId);

        var expectedStartDate = _fixture.Create<GetPendingStartDateChangeResponse>();
        expectedStartDate.HasPendingStartDateChange = true;
        var response = new ApiResponse<GetPendingStartDateChangeResponse>(expectedStartDate, HttpStatusCode.Accepted, string.Empty);
        _apiClientMock.Setup(x => x.Get<GetPendingStartDateChangeResponse>(It.IsAny<GetPendingStartDateChangeRequest>())).ReturnsAsync(response);

        // Act
        var result = await _apprenticeshipService.GetPendingStartDateChange(apprenticeshipHashId);

        // Assert
        result.Should().Be(expectedStartDate);
    }

    [Test]
    public async Task CreateStartDateChange_WhenCalled_PostsRequest()
    {
        // Arrange
        var apprenticeshipKey = _fixture.Create<Guid>();
        var data = _fixture.Create<CreateChangeOfStartDateData>();
        _apiClientMock.Setup(x => x.Post<object>(It.IsAny<CreateChangeOfStartDateRequest>())).ReturnsAsync(new ApiResponse<object>(string.Empty, HttpStatusCode.OK, ""));

        // Act
        await _apprenticeshipService.CreateStartDateChange(apprenticeshipKey, data.Initiator, data.UserId, data.Reason, data.ActualStartDate, data.PlannedEndDate);

        // Assert
        _apiClientMock.Verify(x => x.Post<object>(It.IsAny<CreateChangeOfStartDateRequest>()), Times.Once);
    }

    [Test]
    public void CreateStartDateChange_WhenCalled_ThrowsException()
    {
        // Arrange
        var apprenticeshipKey = _fixture.Create<Guid>();
        var data = _fixture.Create<CreateChangeOfStartDateData>();
        _apiClientMock.Setup(x => x.Post<object>(It.IsAny<CreateChangeOfStartDateRequest>())).ReturnsAsync(new ApiResponse<object>(string.Empty, HttpStatusCode.OK, "Error"));

        // Act / Assert
        Assert.ThrowsAsync<ServiceException>(() => _apprenticeshipService.CreateStartDateChange(
            apprenticeshipKey, data.Initiator, data.UserId, data.Reason, data.ActualStartDate, data.PlannedEndDate));
    }

    [Test]
    public async Task ApprovePendingStartDateChange_WhenCalled_PatchesRequest()
    {
        // Arrange
        var apprenticeshipKey = _fixture.Create<Guid>();
        var userId = _fixture.Create<string>();
        var responseData = new ApiResponse<object>(string.Empty, HttpStatusCode.OK, "");

        _apiClientMock.Setup(x => x.Patch<object>(It.IsAny<ApprovePendingStartDateChangeRequest>()))
            .ReturnsAsync(responseData);

        // Act
        await _apprenticeshipService.ApprovePendingStartDateChange(apprenticeshipKey, userId);

        // Assert
        _apiClientMock.Verify(x => x.Patch<object>(It.IsAny<ApprovePendingStartDateChangeRequest>()), Times.Once);
    }

    [Test]
    public void ApprovePendingStartDateChange_WhenCalled_ThrowsServiceExceptionOnError()
    {
        // Arrange
        var apprenticeshipKey = _fixture.Create<Guid>();
        var userId = _fixture.Create<string>();
        var responseData = new ApiResponse<object>(string.Empty, HttpStatusCode.OK, _fixture.Create<string>());

        _apiClientMock.Setup(x => x.Patch<object>(It.IsAny<ApprovePendingStartDateChangeRequest>()))
            .ReturnsAsync(responseData);

        // Act
        Assert.ThrowsAsync<ServiceException>(() => _apprenticeshipService.ApprovePendingStartDateChange(apprenticeshipKey, userId));
    }

        [Test]
        public async Task RejectPendingStartDateChange_WhenCalled_PatchesRequest()
        {
	        // Arrange
	        var apprenticeshipKey = _fixture.Create<Guid>();
	        var reason = _fixture.Create<string>();
	        var responseData = new ApiResponse<object>(string.Empty, HttpStatusCode.OK, "");

	        _apiClientMock.Setup(x => x.Patch<object>(It.IsAny<RejectPendingStartDateChangeRequest>()))
		        .ReturnsAsync(responseData);

	        // Act
	        await _apprenticeshipService.RejectPendingStartDateChange(apprenticeshipKey, reason);

	        // Assert
	        _apiClientMock.Verify(x => x.Patch<object>(It.IsAny<RejectPendingStartDateChangeRequest>()), Times.Once);
        }

        [Test]
        public void RejectPendingStartDateChange_WhenCalled_ThrowsServiceExceptionOnError()
        {
	        // Arrange
	        var apprenticeshipKey = _fixture.Create<Guid>();
	        var reason = _fixture.Create<string>();
	        var responseData = new ApiResponse<object>(string.Empty, HttpStatusCode.BadRequest, _fixture.Create<string>());

	        _apiClientMock.Setup(x => x.Patch<object>(It.IsAny<RejectPendingStartDateChangeRequest>()))
		        .ReturnsAsync(responseData);

	        // Act & Assert
	        Assert.ThrowsAsync<ServiceException>(() => _apprenticeshipService.RejectPendingStartDateChange(apprenticeshipKey, reason));
        }

	private void MockGetApprenticeshipKey(string apprenticeshipHashId, Guid? apprenticeshipKey = null)
    {
        if (apprenticeshipKey == null)
        {
            apprenticeshipKey = Guid.NewGuid();
        }

        _apiClientMock.Setup(x => x.Get<Guid>(It.Is<GetApprenticeshipKeyRequest>(r => r.GetUrl == $"Apprenticeship/{apprenticeshipHashId}/key")))
            .ReturnsAsync(new ApiResponse<Guid>(apprenticeshipKey.Value, HttpStatusCode.Accepted, string.Empty));
    }
}
