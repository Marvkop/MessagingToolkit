namespace MessagingToolkit.Extensions;

public static class DictionaryExtensions
{
    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> valueCreator)
    {
        if (dictionary.TryGetValue(key, out var value))
        {
            return value;
        }

        return dictionary[key] = valueCreator();
    }
}