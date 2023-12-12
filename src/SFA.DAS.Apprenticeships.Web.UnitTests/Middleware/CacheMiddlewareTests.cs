using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using SFA.DAS.Apprenticeships.Web.Middleware;
using System.Text.Json;

namespace SFA.DAS.Apprenticeships.Web.UnitTests.Middleware
{
	[TestFixture]
	public class CacheMiddlewareTests
	{
		private Mock<RequestDelegate> _mockNext;
		private Mock<IDistributedCache> _mockDistributedCache;
		private Mock<HttpContext> _mockHttpContext;
		private Mock<IQueryCollection> _mockQueryCollection;
		private Mock<ILogger<CacheMiddleware>> _mockLogger;
		
		[SetUp]
		public void SetUp()
		{
			_mockLogger = new Mock<ILogger<CacheMiddleware>>();
			_mockNext = new Mock<RequestDelegate>();
			_mockDistributedCache = new Mock<IDistributedCache>();
			

			_mockHttpContext = new Mock<HttpContext>();
			_mockQueryCollection = new Mock<IQueryCollection>();
			var mockHttpRequest = new Mock<HttpRequest>();

			mockHttpRequest.Setup(m => m.Form).Returns(new FormCollection(new Dictionary<string, StringValues>()));
			mockHttpRequest.Setup(m => m.Query).Returns(_mockQueryCollection.Object);
			_mockHttpContext.Setup(m => m.Request).Returns(mockHttpRequest.Object);
			_mockHttpContext.SetupSet(m => m.Request.Form = It.IsAny<IFormCollection>()).Callback<IFormCollection>(x => mockHttpRequest.Setup(m => m.Form).Returns(x));

		}

		[Test]
		public async Task InvokeAsync_WithCacheKey_PopulatesRequestFromCache()
		{
			// Arrange
			_mockQueryCollection.Setup(m => m["cacheKey"]).Returns(new StringValues("testKey"));
			MockMethodGetStringAsync(_mockDistributedCache, new TestObject { TestProperty = "testValue" });
			var middleware = new CacheMiddleware(_mockNext.Object, _mockDistributedCache.Object, _mockLogger.Object);

			// Act
			await middleware.InvokeAsync(_mockHttpContext.Object);

			// Assert
			Assert.IsTrue(_mockHttpContext.Object.Request.Form.ContainsKey("TestProperty"));
			var actualValue = _mockHttpContext.Object.Request.Form["TestProperty"];
			Assert.That(actualValue, Is.EqualTo("testValue"));
		}

		[Test]
		public async Task InvokeAsync_WithoutCacheKey_DoesNotPopulateRequestFromCache()
		{
			// Arrange
			var middleware = new CacheMiddleware(_mockNext.Object, _mockDistributedCache.Object, _mockLogger.Object);

			// Act
			await middleware.InvokeAsync(_mockHttpContext.Object);

			// Assert
			_mockDistributedCache.Verify(x => x.GetAsync(It.IsAny<string>(), default), Times.Never);
		}

		//  This helps mock the extension method GetStringAsync
		private static void MockMethodGetStringAsync(Mock<IDistributedCache> mockDistributedCache, TestObject obj)
		{
			var bytes = JsonSerializer.SerializeToUtf8Bytes(obj);
			mockDistributedCache.Setup(x => x.GetAsync(It.IsAny<string>(), default)).ReturnsAsync(bytes);
		}
	}

	internal class TestObject
	{
		public string? TestProperty { get; set; }
	}
}
