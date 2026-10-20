namespace IndependentWork8;

/// <summary>
/// Кеш метаданих файлів (сценарій 1). Ключ - ім'я файлу, значення - його метадані.
/// Dictionary дає пошук за ключем за O(1) у середньому.
/// Імена файлів порівнюються без урахування регістру (custom comparer).
/// </summary>
public class FileMetadataCache
{
    private readonly Dictionary<string, FileMetadata> _cache =
        new Dictionary<string, FileMetadata>(StringComparer.OrdinalIgnoreCase);

    public int Count
    {
        get { return _cache.Count; }
    }

    /// <summary>
    /// Додає метадані або замінює наявні для цього імені файлу.
    /// </summary>
    public void Add(FileMetadata metadata)
    {
        ArgumentNullException.ThrowIfNull(metadata);
        _cache[metadata.FileName] = metadata;
    }

    /// <summary>
    /// Повертає метадані або null, якщо файлу немає в кеші.
    /// </summary>
    public FileMetadata? Get(string fileName)
    {
        return _cache.TryGetValue(fileName, out FileMetadata? metadata) ? metadata : null;
    }

    /// <summary>
    /// Видаляє запис. Повертає true, якщо запис існував.
    /// </summary>
    public bool Remove(string fileName)
    {
        return _cache.Remove(fileName);
    }

    public bool Contains(string fileName)
    {
        return _cache.ContainsKey(fileName);
    }
}
