using AdPlatforms.Application.Abstractions;

namespace AdPlatforms.Infrastructure.Implementations;

public class TrieCache<TKey, TValue> : ITrieCache<TKey, TValue> where TKey : notnull
{
    private readonly TrieNode<TKey, TValue> _root = new();

    public Task<IEnumerable<TValue>> GetAsync(IEnumerable<TKey> keys, CancellationToken cancellationToken)
    {
        var childrenList = keys.ToList();
        if (childrenList.Count == 0)
        {
            throw new ArgumentException("Children collection cannot be empty.", nameof(keys));
        }
        
        var result = new List<TValue>();
        
        var currentNode = _root;
        foreach (var child in childrenList)
        {
            if (!currentNode.Children.TryGetValue(child, out currentNode))
            {
                return Task.FromResult<IEnumerable<TValue>>([]);
            }
            
            result.AddRange(currentNode!.Values);
        }
        
        return Task.FromResult<IEnumerable<TValue>>(result);
    }

    public Task AddAsync(IEnumerable<TKey> keys, TValue value, CancellationToken cancellationToken)
    {
        var currentNode = _root;
        foreach (var key in keys)
        {
            if(!currentNode.Children.TryGetValue(key, out var child))
            {
                child = new TrieNode<TKey, TValue>();
                currentNode.Children[key] = child;
            }

            currentNode = child;
        }
        
        currentNode.Values.Add(value);
        return Task.CompletedTask;
    }
}