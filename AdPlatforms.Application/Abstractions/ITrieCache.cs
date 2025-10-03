namespace AdPlatforms.Application.Abstractions;

public interface ITrieCache<TKey, TValue>
{
    Task<IEnumerable<TValue>> GetAsync(IEnumerable<TKey> keys, CancellationToken cancellationToken);
    Task AddAsync(IEnumerable<TKey> keys, TValue value, CancellationToken cancellationToken);
}