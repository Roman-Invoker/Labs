namespace Lab9SelfV14;

/// <summary>
/// Власні узагальнені утиліти для колекцій. Поведінка передається через делегати.
/// </summary>
public static class CollectionUtils
{
    /// <summary>
    /// Перетворює кожен елемент колекції за допомогою mapper (аналог LINQ Select).
    /// Виконується одразу (eager), результат - новий список.
    /// </summary>
    public static IEnumerable<TResult> Map<TSource, TResult>(
        IEnumerable<TSource> collection,
        Func<TSource, TResult> mapper)
    {
        ArgumentNullException.ThrowIfNull(collection);
        ArgumentNullException.ThrowIfNull(mapper);

        var result = new List<TResult>();
        foreach (var item in collection)
        {
            result.Add(mapper(item));
        }
        return result;
    }

    /// <summary>
    /// Повертає елемент з максимальним значенням ключа (аналог Enumerable.MaxBy).
    /// Якщо максимум повторюється, повертається перший такий елемент.
    /// </summary>
    /// <exception cref="InvalidOperationException">Колекція порожня.</exception>
    public static T MaxBy<T, TKey>(
        IEnumerable<T> collection,
        Func<T, TKey> keySelector)
        where TKey : IComparable<TKey>
    {
        ArgumentNullException.ThrowIfNull(collection);
        ArgumentNullException.ThrowIfNull(keySelector);

        T maxItem = default!;
        TKey maxKey = default!;
        bool first = true;

        foreach (var item in collection)
        {
            TKey key = keySelector(item);
            if (first || key.CompareTo(maxKey) > 0)
            {
                maxKey = key;
                maxItem = item;
                first = false;
            }
        }

        if (first)
        {
            throw new InvalidOperationException("Колекція не містить жодного елемента.");
        }

        return maxItem;
    }

    /// <summary>
    /// Повертає елементи, що задовольняють умову (аналог LINQ Where).
    /// Виконується одразу (eager), результат - новий список.
    /// </summary>
    public static IEnumerable<T> Filter<T>(
        IEnumerable<T> collection,
        Predicate<T> predicate)
    {
        ArgumentNullException.ThrowIfNull(collection);
        ArgumentNullException.ThrowIfNull(predicate);

        var result = new List<T>();
        foreach (var item in collection)
        {
            if (predicate(item))
            {
                result.Add(item);
            }
        }
        return result;
    }

    /// <summary>
    /// Виконує дію для кожного елемента колекції.
    /// </summary>
    public static void ForEach<T>(
        IEnumerable<T> collection,
        Action<T> action)
    {
        ArgumentNullException.ThrowIfNull(collection);
        ArgumentNullException.ThrowIfNull(action);

        foreach (var item in collection)
        {
            action(item);
        }
    }
}
