using AdPlatforms.Infrastructure.Implementations;

namespace AdPlatforms.Tests.Infrastructure.TrieCacheBuilderTests;

public class BuildTests
{
    private readonly TrieCacheBuilder<string, string> _builder = new();

    [Fact]
    public void ShouldReturnCache_WhenBuildCalledAfterStart()
    {
        _builder.StartNewBuild();
        var cache = _builder.Build();

        Assert.NotNull(cache);
    }

    [Fact]
    public void ShouldThrow_WhenBuildCalledTwice()
    {
        _builder.StartNewBuild();
        _builder.Build();

        Assert.Throws<InvalidOperationException>(() => _builder.Build());
    }

    [Fact]
    public void ShouldStartNewBuild_AfterPreviousBuild()
    {
        _builder.StartNewBuild();
        var firstCache = _builder.Build();

        _builder.StartNewBuild();
        var secondCache = _builder.Build();

        Assert.NotSame(firstCache, secondCache);
    }
}