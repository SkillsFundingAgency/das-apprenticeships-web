using AutoFixture;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Moq;
using SFA.DAS.Apprenticeships.Infrastructure.Configuration;
using SFA.DAS.Apprenticeships.Web.Models;
using SFA.DAS.Apprenticeships.Web.Services;
using System.Text.Json;

namespace SFA.DAS.Apprenticeships.Web.UnitTests.Services;

public class CacheServiceTests
{
    private Mock<IDistributedCache> _distributedCacheMock;
    private IOptions<CacheConfiguration> _options;
    private Fixture _fixture;

    [SetUp]
    public void SetUp()
    {
        _distributedCacheMock = new Mock<IDistributedCache>();
        _options = Options.Create(new CacheConfiguration { ExpirationInMinutes = 60 });
        _fixture = new Fixture();
    }

    [Test]
    public async Task SetCacheModelAsync_SetsNewGuidAsCacheKey()
    {
        // Arrange
        var cacheService = new CacheService(_distributedCacheMock.Object, _options);
        var cacheModel = new ExampleCacheModel { Value = _fixture.Create<string>() };

        // Act
        await cacheService.SetCacheModelAsync(cacheModel);

        // Assert
        _distributedCacheMock.Verify(x => x.SetAsync(
            It.Is<string>(s => !string.IsNullOrEmpty(s)),
            It.IsAny<byte[]>(),
            It.IsAny<DistributedCacheEntryOptions>(),
            It.IsAny<System.Threading.CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task SetCacheModelAsync_UsesExistingGuidAsCacheKey()
    {
        // Arrange
        var cacheService = new CacheService(_distributedCacheMock.Object, _options);
        var cacheModel = _fixture.Create<ExampleCacheModel>();
        var expectedBytes = JsonSerializer.SerializeToUtf8Bytes(cacheModel);

        // Act
        await cacheService.SetCacheModelAsync(cacheModel);

        // Assert
        _distributedCacheMock.Verify(x => x.SetAsync(
            It.Is<string>(s => s == cacheModel.CacheKey),
            It.Is<byte[]>(b => b.Length == expectedBytes.Length),
            It.IsAny<DistributedCacheEntryOptions>(),
            It.IsAny<System.Threading.CancellationToken>()), Times.Once);
    }
}

internal class ExampleCacheModel : ICacheModel
{
    public string? CacheKey { get; set; } 
    public string? Value { get; set; }
}