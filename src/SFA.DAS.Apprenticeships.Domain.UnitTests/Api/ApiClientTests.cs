using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using SFA.DAS.Apprenticeships.Domain.Api;
using SFA.DAS.Apprenticeships.Domain.Interfaces;
using SFA.DAS.Apprenticeships.Infrastructure.Configuration;
using System.Net;
using System.Text;

namespace SFA.DAS.Apprenticeships.Domain.UnitTests.Api
{
    public class ApiClientTests
    {
        private Mock<IOptions<ApprenticeshipsOuterApi>> _configMock;
        private const string _ExpectedOcpApimKey = "ExpectedOcpApimKey";
        private const string _baseUrl = "http://test.com";
        private const string _ExpectedUrl = "http://test.com/UnitTestEndpoin";
        private HttpRequestMessage? _sentMessage;
        private Fixture _fixture;

        public ApiClientTests()
        {
            _fixture = new Fixture();
        }

        [SetUp]
        public void Setup()
        {
            _configMock = new Mock<IOptions<ApprenticeshipsOuterApi>>();
            var config = new ApprenticeshipsOuterApi { BaseUrl = _baseUrl, Key = _ExpectedOcpApimKey };
            _configMock.Setup(x => x.Value).Returns(config);
            _sentMessage = null;
        }

        [Test]
        public async Task Get_WithValidRequest_ReturnsApiResponse()
        {
            // Arrange
            var request = new Mock<IGetApiRequest>();
            request.Setup(m => m.GetUrl).Returns(_ExpectedUrl);

            var expectedApiResponse = _fixture.Create<string>();
            var httpResponseMessage = GetHttpResponseMessage(HttpStatusCode.OK, expectedApiResponse);
            var mockHttpClient = GetUnitTestHttpClient(httpResponseMessage);
            var apiClient = new ApiClient(mockHttpClient, _configMock.Object);

            // Act
            var result = await apiClient.Get<string>(request.Object);

            // Assert
            result.Body.Should().Be(expectedApiResponse);
            VerifyRequest(_ExpectedUrl, HttpMethod.Get);
        }

        [Test]
        public async Task Post_WithValidRequest_ReturnsApiResponse()
        {
            // Arrange
            var request = new Mock<IPostApiRequest>();
            request.Setup(m => m.PostUrl).Returns(_ExpectedUrl);

            var expectedApiResponse = _fixture.Create<string>();
            var httpResponseMessage = GetHttpResponseMessage(HttpStatusCode.OK, expectedApiResponse);
            var mockHttpClient = GetUnitTestHttpClient(httpResponseMessage);
            var apiClient = new ApiClient(mockHttpClient, _configMock.Object);

            // Act
            var result = await apiClient.Post<string>(request.Object);

            // Assert
            result.Body.Should().Be(expectedApiResponse);
            VerifyRequest(_ExpectedUrl, HttpMethod.Post);
        }

        [Test]
        public async Task Put_WithValidRequest_ReturnsApiResponse()
        {
            // Arrange
            var request = new Mock<IPutApiRequest>();
            request.Setup(m => m.PutUrl).Returns(_ExpectedUrl);

            var expectedApiResponse = _fixture.Create<string>();
            var httpResponseMessage = GetHttpResponseMessage(HttpStatusCode.OK, expectedApiResponse);
            var mockHttpClient = GetUnitTestHttpClient(httpResponseMessage);
            var apiClient = new ApiClient(mockHttpClient, _configMock.Object);

            // Act
            var result = await apiClient.Put<string>(request.Object);

            // Assert
            result.Body.Should().Be(expectedApiResponse);
            VerifyRequest(_ExpectedUrl, HttpMethod.Put);
        }

        [Test]
        public async Task Delete_WithValidRequest_ReturnsApiResponse()
        {
            // Arrange
            var request = new Mock<IDeleteApiRequest>();
            request.Setup(m => m.DeleteUrl).Returns(_ExpectedUrl);

            var expectedApiResponse = _fixture.Create<string>();
            var httpResponseMessage = GetHttpResponseMessage(HttpStatusCode.OK, expectedApiResponse);
            var mockHttpClient = GetUnitTestHttpClient(httpResponseMessage);
            var apiClient = new ApiClient(mockHttpClient, _configMock.Object);

            // Act
            var result = await apiClient.Delete<string>(request.Object);

            // Assert
            result.Body.Should().Be(expectedApiResponse);
            VerifyRequest(_ExpectedUrl, HttpMethod.Delete);
        }

        private HttpClient GetUnitTestHttpClient(HttpResponseMessage expectedResponse)
        {
            var httpClientHandler = new Mock<HttpMessageHandler>();

            httpClientHandler.Protected()
                    .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                    .Callback((HttpRequestMessage requestMessage, CancellationToken token) =>
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
            _sentMessage.Headers.GetValues("Ocp-Apim-Subscription-Key").Should().Contain(_ExpectedOcpApimKey);
        }

        private HttpResponseMessage GetHttpResponseMessage(HttpStatusCode statusCode, string apiResponse)
        {
            var response = new HttpResponseMessage(statusCode);
            response.Content = new StringContent(JsonConvert.SerializeObject(apiResponse), Encoding.UTF8, "application/json");
            return response;
        }
    }
}