using FluentAssertions;
using SFA.DAS.Apprenticeships.Domain.Api;
using System.Net;

namespace SFA.DAS.Apprenticeships.Domain.UnitTests.Api;


[TestFixture]
public class ApiResponseTests
{
    [Test]
    public void ApiResponse_StatusOk_IsSuccessStatusCode_True()
    {
        // Arrange
        var expectedBody = "Test Body";
        var expectedStatusCode = HttpStatusCode.OK;
        var expectedErrorContent = string.Empty;

        // Act
        var apiResponse = new ApiResponse<string>(expectedBody, expectedStatusCode, expectedErrorContent);

        // Assert
        apiResponse.Body.Should().Be(expectedBody);
        apiResponse.StatusCode.Should().Be(expectedStatusCode);
        apiResponse.IsSuccessStatusCode.Should().BeTrue();
        apiResponse.ErrorContent.Should().Be(expectedErrorContent);
    }

    [Test]
    public void ApiResponse_StatusNotFound_IsSuccessStatusCode_False()
    {
        // Arrange
        var expectedBody = "Test Body";
        var expectedStatusCode = HttpStatusCode.NotFound;
        var expectedErrorContent = "Test Error";

        // Act
        var apiResponse = new ApiResponse<string>(expectedBody, expectedStatusCode, expectedErrorContent);

        // Assert
        apiResponse.Body.Should().Be(expectedBody);
        apiResponse.StatusCode.Should().Be(expectedStatusCode);
        apiResponse.IsSuccessStatusCode.Should().BeFalse();
        apiResponse.ErrorContent.Should().Be(expectedErrorContent);
    }
}

