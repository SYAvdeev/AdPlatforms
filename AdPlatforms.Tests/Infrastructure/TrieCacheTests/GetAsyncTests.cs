using AdPlatforms.Infrastructure.Implementations;

namespace AdPlatforms.Tests.Infrastructure.TrieCacheTests;

public class GetAsyncTests
{
    private readonly TrieCache<string, string> _cache = new();

    [Fact]
    public async Task ShouldThrow_ArgumentException_WhenKeysEmpty()
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _cache.GetAsync(Array.Empty<string>(), CancellationToken.None));
    }

    [Fact]
    public async Task ShouldReturnEmpty_WhenPathNotFound()
    {
        var result = await _cache.GetAsync(["unknown"], CancellationToken.None);

        Assert.Empty(result);
    }

    [Fact]
    public async Task ShouldReturnValues_WhenPathExists()
    {
        await _cache.AddAsync(["a", "b"], "value1", CancellationToken.None);
        await _cache.AddAsync(["a", "b"], "value2", CancellationToken.None);

        var result = (await _cache.GetAsync(["a", "b"], CancellationToken.None)).ToList();

        Assert.Equal(2, result.Count);
        Assert.Contains("value1", result);
        Assert.Contains("value2", result);
    }

    [Fact]
    public async Task ShouldReturnIntermediateValues_AlongPath()
    {
        await _cache.AddAsync(["root"], "r", CancellationToken.None);
        await _cache.AddAsync(["root", "child"], "c", CancellationToken.None);

        var result = (await _cache.GetAsync(["root", "child"], CancellationToken.None)).ToList();

        Assert.Equal(2, result.Count);
        Assert.Contains("r", result);
        Assert.Contains("c", result);
    }
}