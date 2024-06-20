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
        Assert.That(apiResponse.Body.Equals(expectedBody));
        Assert.That(apiResponse.StatusCode.Equals(expectedStatusCode));
        Assert.IsTrue(apiResponse.IsSuccessStatusCode);
        Assert.That(apiResponse.ErrorContent.Equals(expectedErrorContent));
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
        Assert.That(apiResponse.Body.Equals(expectedBody));
        Assert.That(apiResponse.StatusCode.Equals(expectedStatusCode));
        Assert.IsFalse(apiResponse.IsSuccessStatusCode);
        Assert.That(apiResponse.ErrorContent.Equals(expectedErrorContent));
    }
}

