namespace AdPlatforms.Infrastructure.Implementations;

public class TrieNode<TKey, TValue> where TKey : notnull
{
    internal IDictionary<TKey, TrieNode<TKey, TValue>> Children { get; } = new Dictionary<TKey, TrieNode<TKey, TValue>>();
    internal IList<TValue> Values { get; } = new List<TValue>();
}