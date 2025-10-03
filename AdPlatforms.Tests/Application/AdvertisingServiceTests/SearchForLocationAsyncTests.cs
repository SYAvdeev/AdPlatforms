using AdPlatforms.Application.Abstractions;
using AdPlatforms.Application.Contracts;
using AdPlatforms.Application.Exceptions;
using AdPlatforms.Application.Services;
using AdPlatforms.Domain.Entities;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace AdPlatforms.Tests.Application.AdvertisingServiceTests;

public class SearchForLocationAsyncTests
{
    private readonly Mock<ITrieCacheBuilder<string, Advertising>> _cacheBuilderMock;
    private readonly Mock<ITrieCache<string, Advertising>> _trieCacheMock;
    private readonly IMemoryCache _memoryCache;
    private readonly Mock<IMapper> _mapperMock;
    private readonly AdvertisingService _service;

    public SearchForLocationAsyncTests()
    {
        _cacheBuilderMock = new Mock<ITrieCacheBuilder<string, Advertising>>();
        _trieCacheMock = new Mock<ITrieCache<string, Advertising>>();
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _mapperMock = new Mock<IMapper>();

        _service = new AdvertisingService(
            _cacheBuilderMock.Object,
            _memoryCache,
            _mapperMock.Object
        );
    }

    [Fact]
    public async Task ShouldThrow_NoFileException_WhenTrieCacheIsNull()
    {
        await Assert.ThrowsAsync<NoFileException>(() =>
            _service.SearchForLocationAsync("moscow", CancellationToken.None));
    }

    [Fact]
    public async Task ShouldReturnFromCache_WhenExists()
    {
        var location = "moscow";
        var expected = new List<AdvertisingDto> { new () { Name = "ad" } };
        _memoryCache.Set(location, expected);

        typeof(AdvertisingService)
            .GetField("_trieCache", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .SetValue(_service, _trieCacheMock.Object);

        var result = await _service.SearchForLocationAsync(location, CancellationToken.None);

        Assert.Same(expected, result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("///")]
    public async Task ShouldThrow_BadLocationException_WhenInvalidLocation(string location)
    {
        typeof(AdvertisingService)
            .GetField("_trieCache", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .SetValue(_service, _trieCacheMock.Object);

        await Assert.ThrowsAsync<BadLocationException>(() =>
            _service.SearchForLocationAsync(location, CancellationToken.None));
    }

    [Fact]
    public async Task ShouldReturnMappedResult_AndCacheIt()
    {
        var location = "moscow/russia";
        var advertising = new Advertising { Name = "Test", Location = location };
        var dto = new AdvertisingDto { Name = "Test" };

        _trieCacheMock
            .Setup(x => x.GetAsync(It.IsAny<List<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Advertising> { advertising });

        _mapperMock
            .Setup(m => m.Map<AdvertisingDto>(advertising))
            .Returns(dto);

        typeof(AdvertisingService)
            .GetField("_trieCache", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .SetValue(_service, _trieCacheMock.Object);

        var result = (await _service.SearchForLocationAsync(location, CancellationToken.None)).ToList();

        Assert.Single(result);
        Assert.Equal(dto.Name, result[0].Name);
        Assert.True(_memoryCache.TryGetValue(location, out _));
    }
}
