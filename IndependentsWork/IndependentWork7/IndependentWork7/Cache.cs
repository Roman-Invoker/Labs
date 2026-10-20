namespace IndependentWork7;

/// <summary>
/// Узагальнений кеш з обмеженою ємністю та політикою витіснення FIFO
/// (First In, First Out): коли кеш повний, видаляється елемент, доданий найраніше.
/// </summary>
/// <typeparam name="TKey">Тип ключа (не може бути null).</typeparam>
/// <typeparam name="TValue">Тип значення.</typeparam>
public class Cache<TKey, TValue> where TKey : notnull
{
    private readonly int _capacity;
    private readonly Dictionary<TKey, TValue> _cache;
    private readonly Queue<TKey> _keysOrder; // порядок додавання ключів: попереду найстаріший

    /// <summary>
    /// Створює кеш із заданою ємністю.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Ємність менша або дорівнює нулю.</exception>
    public Cache(int capacity)
    {
        if (capacity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity), capacity, "Ємність кешу має бути більшою за нуль.");
        }

        _capacity = capacity;
        _cache = new Dictionary<TKey, TValue>(capacity);
        _keysOrder = new Queue<TKey>(capacity);
    }

    /// <summary>
    /// Поточна кількість елементів у кеші.
    /// </summary>
    public int Count
    {
        get { return _cache.Count; }
    }

    /// <summary>
    /// Додає елемент. Якщо ключ уже є, оновлює значення (порядок витіснення не змінюється).
    /// Якщо кеш повний, спершу видаляє найстаріший елемент.
    /// </summary>
    public void Add(TKey key, TValue value)
    {
        // Оновлення наявного ключа: розмір кешу та черга не змінюються
        if (_cache.ContainsKey(key))
        {
            _cache[key] = value;
            return;
        }

        // Кеш повний: витісняємо найстаріший елемент
        if (_cache.Count >= _capacity)
        {
            TKey oldestKey = _keysOrder.Dequeue();
            _cache.Remove(oldestKey);
        }

        _cache.Add(key, value);
        _keysOrder.Enqueue(key);
    }

    /// <summary>
    /// Повертає значення за ключем.
    /// </summary>
    /// <exception cref="KeyNotFoundException">Ключа немає в кеші.</exception>
    public TValue Get(TKey key)
    {
        if (_cache.TryGetValue(key, out TValue value))
        {
            return value;
        }

        throw new KeyNotFoundException($"Ключ '{key}' не знайдено в кеші.");
    }

    /// <summary>
    /// Перевіряє, чи є ключ у кеші.
    /// </summary>
    public bool ContainsKey(TKey key)
    {
        return _cache.ContainsKey(key);
    }
}
