using AdPlatforms.Infrastructure.Implementations;

namespace AdPlatforms.Tests.Infrastructure.TrieCacheBuilderTests;

public class AddAsyncTests
{
    private readonly TrieCacheBuilder<string, string> _builder = new();

    [Fact]
    public async Task ShouldAddValues_WhenBuildStarted()
    {
        _builder.StartNewBuild();

        await _builder.AddAsync(["a", "b"], "value1", CancellationToken.None);
        var cache = _builder.Build();

        var result = (await cache.GetAsync(["a", "b"], CancellationToken.None)).ToList();

        Assert.Single(result);
        Assert.Equal("value1", result[0]);
    }

    [Fact]
    public async Task ShouldThrow_WhenBuildNotStarted()
    {
        // Сбрасываем внутренний _trieCache вызовом Build()
        var cache = _builder.Build();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _builder.AddAsync(["x"], "y", CancellationToken.None));
    }

    [Fact]
    public async Task ShouldAllowMultipleAddAfterStartNewBuild()
    {
        _builder.StartNewBuild();

        await _builder.AddAsync(["root"], "r", CancellationToken.None);
        await _builder.AddAsync(["root", "child"], "c", CancellationToken.None);

        var cache = _builder.Build();
        var result = (await cache.GetAsync(["root", "child"], CancellationToken.None)).ToList();

        Assert.Equal(2, result.Count);
        Assert.Contains("r", result);
        Assert.Contains("c", result);
    }
}