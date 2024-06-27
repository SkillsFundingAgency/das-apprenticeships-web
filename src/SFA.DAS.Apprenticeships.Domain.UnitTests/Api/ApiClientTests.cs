using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using SFA.DAS.Apprenticeships.Domain.Api;
using SFA.DAS.Apprenticeships.Domain.Interfaces;
using SFA.DAS.Apprenticeships.Infrastructure;
using SFA.DAS.Apprenticeships.Infrastructure.Configuration;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace SFA.DAS.Apprenticeships.Domain.UnitTests.Api
{
    public class ApiClientTests
    {
        private Mock<IOptions<ApprenticeshipsOuterApi>> _configMock;
        private readonly Mock<IHttpContextAccessor> _contextAccessorMock;
        private const string ExpectedOcpApimKey = "ExpectedOcpApimKey";
        private const string BaseUrl = "http://test.com";
        private const string ExpectedUrl = "http://test.com/UnitTestEndpoin";
        private HttpRequestMessage? _sentMessage;
        private Fixture _fixture;

        public ApiClientTests()
        {
            _fixture = new Fixture();
            _contextAccessorMock = GetMockIHttpContextAccessor();
        }

        [SetUp]
        public void Setup()
        {
            _configMock = new Mock<IOptions<ApprenticeshipsOuterApi>>();
            var config = new ApprenticeshipsOuterApi { BaseUrl = BaseUrl, Key = ExpectedOcpApimKey };
            _configMock.Setup(x => x.Value).Returns(config);
            _sentMessage = null;
        }

        [Test]
        public async Task Get_WithValidRequest_ReturnsApiResponse()
        {
            // Arrange
            var request = new Mock<IGetApiRequest>();
            request.Setup(m => m.GetUrl).Returns(ExpectedUrl);

            var expectedApiResponse = _fixture.Create<string>();
            var httpResponseMessage = GetHttpResponseMessage(HttpStatusCode.OK, expectedApiResponse);
            var mockHttpClient = GetUnitTestHttpClient(httpResponseMessage);
            var apiClient = new ApiClient(mockHttpClient, _configMock.Object, _contextAccessorMock.Object);

            // Act
            var result = await apiClient.Get<string>(request.Object);

            // Assert
            result.Body.Should().Be(expectedApiResponse);
            VerifyRequest(ExpectedUrl, HttpMethod.Get);
        }

        [Test]
        public async Task Post_WithValidRequest_ReturnsApiResponse()
        {
            // Arrange
            var request = new Mock<IPostApiRequest>();
            request.Setup(m => m.PostUrl).Returns(ExpectedUrl);

            var expectedApiResponse = _fixture.Create<string>();
            var httpResponseMessage = GetHttpResponseMessage(HttpStatusCode.OK, expectedApiResponse);
            var mockHttpClient = GetUnitTestHttpClient(httpResponseMessage);
            var apiClient = new ApiClient(mockHttpClient, _configMock.Object, _contextAccessorMock.Object);

            // Act
            var result = await apiClient.Post<string>(request.Object);

            // Assert
            result.Body.Should().Be(expectedApiResponse);
            VerifyRequest(ExpectedUrl, HttpMethod.Post);
        }

        [Test]
        public async Task Put_WithValidRequest_ReturnsApiResponse()
        {
            // Arrange
            var request = new Mock<IPutApiRequest>();
            request.Setup(m => m.PutUrl).Returns(ExpectedUrl);

            var expectedApiResponse = _fixture.Create<string>();
            var httpResponseMessage = GetHttpResponseMessage(HttpStatusCode.OK, expectedApiResponse);
            var mockHttpClient = GetUnitTestHttpClient(httpResponseMessage);
            var apiClient = new ApiClient(mockHttpClient, _configMock.Object, _contextAccessorMock.Object);

            // Act
            var result = await apiClient.Put<string>(request.Object);

            // Assert
            result.Body.Should().Be(expectedApiResponse);
            VerifyRequest(ExpectedUrl, HttpMethod.Put);
        }

        [Test]
        public async Task Delete_WithValidRequest_ReturnsApiResponse()
        {
            // Arrange
            var request = new Mock<IDeleteApiRequest>();
            request.Setup(m => m.DeleteUrl).Returns(ExpectedUrl);

            var expectedApiResponse = _fixture.Create<string>();
            var httpResponseMessage = GetHttpResponseMessage(HttpStatusCode.OK, expectedApiResponse);
            var mockHttpClient = GetUnitTestHttpClient(httpResponseMessage);
            var apiClient = new ApiClient(mockHttpClient, _configMock.Object, _contextAccessorMock.Object);

            // Act
            var result = await apiClient.Delete<string>(request.Object);

            // Assert
            result.Body.Should().Be(expectedApiResponse);
            VerifyRequest(ExpectedUrl, HttpMethod.Delete);
        }

        [Test]
        public void UnauthorizedResponse_ThrowsCorrectException()
        {
            // Arrange
            var request = new Mock<IGetApiRequest>();
            request.Setup(m => m.GetUrl).Returns(ExpectedUrl);

            var expectedApiResponse = _fixture.Create<string>();
            var httpResponseMessage = GetHttpResponseMessage(HttpStatusCode.Unauthorized, expectedApiResponse);
            var mockHttpClient = GetUnitTestHttpClient(httpResponseMessage);
            var apiClient = new ApiClient(mockHttpClient, _configMock.Object, _contextAccessorMock.Object);

            // Act & Assert
            Assert.ThrowsAsync<ApiUnauthorizedException>(() => apiClient.Get<string>(request.Object));
        }

        [Test]
        public async Task Patch_WithValidRequest_ReturnsApiResponse()
        {
            // Arrange
            var request = new Mock<IPatchApiRequest>();
            request.Setup(m => m.PatchUrl).Returns(ExpectedUrl);

            var expectedApiResponse = _fixture.Create<string>();
            var httpResponseMessage = GetHttpResponseMessage(HttpStatusCode.OK, expectedApiResponse);
            var mockHttpClient = GetUnitTestHttpClient(httpResponseMessage);
            var apiClient = new ApiClient(mockHttpClient, _configMock.Object, _contextAccessorMock.Object);

            // Act
            var result = await apiClient.Patch<string>(request.Object);

            // Assert
            result.Body.Should().Be(expectedApiResponse);
            VerifyRequest(ExpectedUrl, HttpMethod.Patch);
        }

        private HttpClient GetUnitTestHttpClient(HttpResponseMessage expectedResponse)
        {
            var httpClientHandler = new Mock<HttpMessageHandler>();

            httpClientHandler.Protected()
                    .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                    .Callback((HttpRequestMessage requestMessage, CancellationToken _) =>
                    {
                        _sentMessage = requestMessage;
                    })
                    .ReturnsAsync(expectedResponse);

            var httpclient = new HttpClient(httpClientHandler.Object);

            return httpclient;
        }

        private void VerifyRequest(string expectedUrl, HttpMethod httpMethod)
        {
            if(_sentMessage == null)
            {
                Assert.Fail("No request was sent");
                return;
            }

            _sentMessage.Method.Should().Be(httpMethod);
            _sentMessage.RequestUri.Should().BeEquivalentTo(new Uri(expectedUrl));
            _sentMessage.Headers.GetValues("Ocp-Apim-Subscription-Key").Should().Contain(ExpectedOcpApimKey);
        }

        private static HttpResponseMessage GetHttpResponseMessage(HttpStatusCode statusCode, string apiResponse)
        {
            var response = new HttpResponseMessage(statusCode);
            response.Content = new StringContent(JsonConvert.SerializeObject(apiResponse), Encoding.UTF8, "application/json");
            return response;
        }

		private static Mock<IHttpContextAccessor> GetMockIHttpContextAccessor()
		{
            BearerTokenProvider.SetSigningKey("abcdefghijklmnopqrstuv123456789==");

			var contextMock = new Mock<HttpContext>();
			var claimsPrincipalMock = new Mock<ClaimsPrincipal>();

			// Create a list of claims for the authenticated user
			var claims = new List<Claim>
	        {
		        new Claim(ClaimTypes.Name, "Test User"),
		        new Claim(ClaimTypes.NameIdentifier, "1"),
                // Add more claims as needed for testing
            };

			// Setup the ClaimsPrincipal to return the authenticated user
			claimsPrincipalMock.Setup(m => m.Identity!.IsAuthenticated).Returns(true);
			claimsPrincipalMock.Setup(m => m.Claims).Returns(claims);

			contextMock.Setup(ctx => ctx.User).Returns(claimsPrincipalMock.Object);
			var contextAccessorMock = new Mock<IHttpContextAccessor>();
			contextAccessorMock.Setup(x => x.HttpContext).Returns(contextMock.Object);
			return contextAccessorMock;
		}
	}
}