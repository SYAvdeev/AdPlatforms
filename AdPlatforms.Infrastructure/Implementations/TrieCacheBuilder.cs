using AdPlatforms.Application.Abstractions;

namespace AdPlatforms.Infrastructure.Implementations;

public class TrieCacheBuilder<TKey, TValue> : ITrieCacheBuilder<TKey, TValue> where TKey : notnull
{
    private ITrieCache<TKey, TValue>? _trieCache = new TrieCache<TKey, TValue>();
    
    public void StartNewBuild()
    {
        _trieCache = new TrieCache<TKey, TValue>();
    }

    public async Task AddAsync(IEnumerable<TKey> keys, TValue value, CancellationToken cancellationToken)
    {
        if (_trieCache == null)
        {
            throw new InvalidOperationException("Build process has not been started. Call StartNewBuild() before adding items.");
        }
        
        await _trieCache.AddAsync(keys, value, cancellationToken);
    }

    public ITrieCache<TKey, TValue> Build()
    {
        if (_trieCache == null)
        {
            throw new InvalidOperationException("Build process has not been started. Call StartNewBuild() before adding items.");
        }
        
        var result = _trieCache;
        _trieCache = null;
        return result;
    }
}