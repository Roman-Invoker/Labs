namespace Lab9SelfV14;

/// <summary>Власні узагальнені утиліти для колекцій на основі делегатів (без використання LINQ).</summary>
public static class CollectionUtils
{
    /// <summary>Перетворює кожен елемент колекції за допомогою <paramref name="mapper"/> (аналог LINQ Select). Виконання відкладене.</summary>
    public static IEnumerable<TResult> Map<TSource, TResult>(
        IEnumerable<TSource> collection,
        Func<TSource, TResult> mapper)
    {
        ArgumentNullException.ThrowIfNull(collection);
        ArgumentNullException.ThrowIfNull(mapper);

        return MapIterator(collection, mapper);
    }

    private static IEnumerable<TResult> MapIterator<TSource, TResult>(
        IEnumerable<TSource> collection,
        Func<TSource, TResult> mapper)
    {
        foreach (TSource item in collection)
        {
            yield return mapper(item);
        }
    }

    /// <summary>
    /// Повертає елемент з максимальним значенням ключа. Для порожньої колекції викидає <see cref="InvalidOperationException"/>.
    /// Якщо максимум досягається в кількох елементах — повертається перший з них.
    /// </summary>
    public static T MaxBy<T, TKey>(
        IEnumerable<T> collection,
        Func<T, TKey> keySelector)
        where TKey : IComparable<TKey>
    {
        ArgumentNullException.ThrowIfNull(collection);
        ArgumentNullException.ThrowIfNull(keySelector);

        bool first = true;
        T maxItem = default!;
        TKey maxKey = default!;

        foreach (T item in collection)
        {
            TKey key = keySelector(item);

            if (first || key.CompareTo(maxKey) > 0)
            {
                maxItem = item;
                maxKey = key;
                first = false;
            }
        }

        if (first)
        {
            throw new InvalidOperationException("Колекція не містить жодного елемента.");
        }

        return maxItem;
    }

    /// <summary>Повертає елементи, що задовольняють умову (аналог LINQ Where). Виконання відкладене.</summary>
    public static IEnumerable<T> Filter<T>(
        IEnumerable<T> collection,
        Predicate<T> predicate)
    {
        ArgumentNullException.ThrowIfNull(collection);
        ArgumentNullException.ThrowIfNull(predicate);

        return FilterIterator(collection, predicate);
    }

    private static IEnumerable<T> FilterIterator<T>(IEnumerable<T> collection, Predicate<T> predicate)
    {
        foreach (T item in collection)
        {
            if (predicate(item))
            {
                yield return item;
            }
        }
    }

    /// <summary>Виконує дію для кожного елемента колекції (негайно).</summary>
    public static void ForEach<T>(
        IEnumerable<T> collection,
        Action<T> action)
    {
        ArgumentNullException.ThrowIfNull(collection);
        ArgumentNullException.ThrowIfNull(action);

        foreach (T item in collection)
        {
            action(item);
        }
    }
}
