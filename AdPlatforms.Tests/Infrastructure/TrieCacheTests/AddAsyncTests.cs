using AdPlatforms.Infrastructure.Implementations;

namespace AdPlatforms.Tests.Infrastructure.TrieCacheTests;

public class AddAsyncTests
{
    private readonly TrieCache<string, string> _cache = new();

    [Fact]
    public async Task ShouldAddSingleValue()
    {
        await _cache.AddAsync(["x"], "valueX", CancellationToken.None);

        var result = (await _cache.GetAsync(["x"], CancellationToken.None)).ToList();

        Assert.Single(result);
        Assert.Equal("valueX", result[0]);
    }

    [Fact]
    public async Task ShouldAddMultipleValuesOnSamePath()
    {
        await _cache.AddAsync(["y"], "v1", CancellationToken.None);
        await _cache.AddAsync(["y"], "v2", CancellationToken.None);

        var result = (await _cache.GetAsync(["y"], CancellationToken.None)).ToList();

        Assert.Equal(2, result.Count);
        Assert.Contains("v1", result);
        Assert.Contains("v2", result);
    }

    [Fact]
    public async Task ShouldCreateNestedNodes()
    {
        await _cache.AddAsync(["p", "q", "r"], "deepValue", CancellationToken.None);

        var result = (await _cache.GetAsync(["p", "q", "r"], CancellationToken.None)).ToList();

        Assert.Single(result);
        Assert.Equal("deepValue", result[0]);
    }

    [Fact]
    public async Task ShouldNotInterfereDifferentBranches()
    {
        await _cache.AddAsync(["a", "b"], "branch1", CancellationToken.None);
        await _cache.AddAsync(["a", "c"], "branch2", CancellationToken.None);

        var res1 = (await _cache.GetAsync(["a", "b"], CancellationToken.None)).ToList();
        var res2 = (await _cache.GetAsync(["a", "c"], CancellationToken.None)).ToList();

        Assert.Single(res1);
        Assert.Equal("branch1", res1[0]);

        Assert.Single(res2);
        Assert.Equal("branch2", res2[0]);
    }
}
