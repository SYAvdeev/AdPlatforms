namespace AdPlatforms.Application.Abstractions;

public interface ITrieCacheBuilder<TKey, TValue>
{
    void StartNewBuild();
    Task AddAsync(IEnumerable<TKey> keys, TValue value, CancellationToken cancellationToken);
    ITrieCache<TKey, TValue> Build();
}