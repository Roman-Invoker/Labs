namespace IndependentWork7;

/// <summary>
/// Узагальнений кеш обмеженої ємності з політикою витіснення FIFO (First In, First Out):
/// коли кеш заповнений, під час додавання нового ключа видаляється найстаріший доданий елемент.
/// </summary>
/// <typeparam name="TKey">Тип ключа.</typeparam>
/// <typeparam name="TValue">Тип значення.</typeparam>
public class Cache<TKey, TValue> where TKey : notnull
{
    private readonly int _capacity;
    private readonly Dictionary<TKey, TValue> _cache;
    private readonly Queue<TKey> _keysOrder;

    /// <param name="capacity">Максимальна кількість елементів (більше 0).</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> не більша за 0.</exception>
    public Cache(int capacity)
    {
        if (capacity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity), capacity, "Ємність кешу має бути більшою за 0.");
        }

        _capacity = capacity;
        _cache = new Dictionary<TKey, TValue>(capacity);
        _keysOrder = new Queue<TKey>(capacity);
    }

    /// <summary>Поточна кількість елементів у кеші.</summary>
    public int Count => _cache.Count;

    /// <summary>
    /// Додає елемент. Якщо ключ уже є — оновлює значення (порядок витіснення не змінюється).
    /// Якщо кеш повний — спершу видаляє найстаріший елемент.
    /// </summary>
    public void Add(TKey key, TValue value)
    {
        ArgumentNullException.ThrowIfNull(key);

        if (_cache.ContainsKey(key))
        {
            _cache[key] = value;
            return;
        }

        if (_cache.Count >= _capacity)
        {
            TKey oldestKey = _keysOrder.Dequeue();
            _cache.Remove(oldestKey);
        }

        _cache.Add(key, value);
        _keysOrder.Enqueue(key);
    }

    /// <summary>Повертає значення за ключем.</summary>
    /// <exception cref="KeyNotFoundException">Ключа немає в кеші.</exception>
    public TValue Get(TKey key)
    {
        ArgumentNullException.ThrowIfNull(key);

        if (!_cache.TryGetValue(key, out TValue? value))
        {
            throw new KeyNotFoundException($"Ключ '{key}' не знайдено в кеші.");
        }

        return value;
    }

    /// <summary>Перевіряє, чи є ключ у кеші.</summary>
    public bool ContainsKey(TKey key)
    {
        ArgumentNullException.ThrowIfNull(key);
        return _cache.ContainsKey(key);
    }
}
