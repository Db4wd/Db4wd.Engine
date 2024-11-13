namespace DbForward.Utilities;

internal static class EnumerableExtensions
{
    public static Dictionary<TKey, TValue> MergeAdd<TKey, TValue>(this IDictionary<TKey, TValue> source,
        IEnumerable<KeyValuePair<TKey, TValue>> items) where TKey : notnull
    {
        var dictionary = new Dictionary<TKey, TValue>(source);
        
        foreach (var (key, value) in items)
        {
            dictionary.Add(key, value);
        }

        return dictionary;
    }
    
    public static IDictionary<TKey, TValue> MergeReplace<TKey, TValue>(this IDictionary<TKey, TValue> source,
        IEnumerable<KeyValuePair<TKey, TValue>> items) where TKey : notnull
    {
        var dictionary = new Dictionary<TKey, TValue>(source);
        
        foreach (var (key, value) in items)
        {
            dictionary[key] = value;
        }

        return dictionary;
    }

    public static IEnumerable<T> TakeUntilInclusive<T>(this IEnumerable<T> source, Predicate<T> predicate)
    {
        foreach (var item in source)
        {
            yield return item;

            if (predicate(item))
                yield break;
        }
    }
}