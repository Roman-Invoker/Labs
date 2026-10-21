namespace Lab14v14;

/// <summary>
/// Статична узагальнена утиліта для роботи з колекціями.
/// Варіант 14: метод <see cref="Take"/>.
/// </summary>
/// <typeparam name="T">Тип елементів колекції.</typeparam>
public static class CollectionHelper<T>
{
    /// <summary>
    /// Повертає перші <paramref name="count"/> елементів колекції.
    /// Власна реалізація, без виклику <c>Enumerable.Take</c>.
    /// </summary>
    /// <param name="collection">Вихідна послідовність.</param>
    /// <param name="count">Скільки елементів узяти.</param>
    /// <returns>
    /// Лінива послідовність із не більше ніж <paramref name="count"/> елементів.
    /// Якщо <paramref name="count"/> ≤ 0 — порожня; якщо більше за розмір колекції — вся колекція.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="collection"/> дорівнює null.</exception>
    public static IEnumerable<T> Take(IEnumerable<T> collection, int count)
    {
        // Перевірка аргументів виконується одразу під час виклику,
        // а не відкладається до першого перебору результату.
        ArgumentNullException.ThrowIfNull(collection);

        return TakeIterator(collection, count);
    }

    private static IEnumerable<T> TakeIterator(IEnumerable<T> collection, int count)
    {
        if (count <= 0)
        {
            yield break;
        }

        int taken = 0;
        foreach (T item in collection)
        {
            yield return item;

            // Зупиняємось одразу після останнього потрібного елемента,
            // не запитуючи в джерела наступний (важливо для нескінченних послідовностей).
            taken++;
            if (taken >= count)
            {
                yield break;
            }
        }
    }
}
