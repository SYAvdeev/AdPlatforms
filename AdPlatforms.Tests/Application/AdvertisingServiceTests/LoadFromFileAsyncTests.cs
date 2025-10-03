using AdPlatforms.Application.Abstractions;
using AdPlatforms.Application.Exceptions;
using AdPlatforms.Application.Services;
using AdPlatforms.Domain.Entities;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace AdPlatforms.Tests.Application.AdvertisingServiceTests;

public class LoadFromFileAsyncTests
{
    private readonly Mock<ITrieCacheBuilder<string, Advertising>> _cacheBuilderMock;
    private readonly Mock<ITrieCache<string, Advertising>> _trieCacheMock;
    private readonly IMemoryCache _memoryCache;
    private readonly Mock<IMapper> _mapperMock;
    private readonly AdvertisingService _service;

    public LoadFromFileAsyncTests()
    {
        _cacheBuilderMock = new Mock<ITrieCacheBuilder<string, Advertising>>();
        _trieCacheMock = new Mock<ITrieCache<string, Advertising>>();
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _mapperMock = new Mock<IMapper>();

        _cacheBuilderMock.Setup(c => c.Build()).Returns(_trieCacheMock.Object);

        _service = new AdvertisingService(_cacheBuilderMock.Object, _memoryCache, _mapperMock.Object);
    }

    [Fact]
    public async Task ShouldLoadDataFromStream_WhenFormatIsValid()
    {
        var content = "Ad1: moscow/spb, spb\nAd2: kazan";
        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));

        await _service.LoadFromFileAsync(stream, CancellationToken.None);

        _cacheBuilderMock.Verify(c => c.StartNewBuild(), Times.Once);
        _cacheBuilderMock.Verify(c => c.AddAsync(
            It.IsAny<IEnumerable<string>>(),
            It.IsAny<Advertising>(),
            It.IsAny<CancellationToken>()),
            Times.AtLeastOnce);
        _cacheBuilderMock.Verify(c => c.Build(), Times.Once);
    }

    [Theory]
    [InlineData("invalid_line_without_colon")]
    [InlineData("Ad1: ")]
    [InlineData(": moscow")]
    [InlineData("Ad2: ,,,")]
    [InlineData("Ad3: /")]
    public async Task ShouldThrowBadFileFormatException_WhenFileIsInvalid(string content)
    {
        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));

        await Assert.ThrowsAsync<BadFileFormatException>(() => _service.LoadFromFileAsync(stream, CancellationToken.None));
    }
}
