namespace IndependentWork8;

/// <summary>
/// Сценарій 1. Кеш метаданих файлів на основі Dictionary&lt;string, FileMetadata&gt;.
/// Ключ — ім'я файлу (без урахування регістру, як у файловій системі Windows).
/// Усі операції доступу за ключем мають складність O(1) в середньому.
/// </summary>
public class FileMetadataCache
{
    private readonly Dictionary<string, FileMetadata> _cache = new(StringComparer.OrdinalIgnoreCase);

    public int Count => _cache.Count;

    /// <summary>Додає метадані. Якщо файл із такою назвою вже є — викидає ArgumentException.</summary>
    public void Add(FileMetadata metadata)
    {
        ArgumentNullException.ThrowIfNull(metadata);

        if (string.IsNullOrWhiteSpace(metadata.FileName))
        {
            throw new ArgumentException("Ім'я файлу не може бути порожнім.", nameof(metadata));
        }

        if (!_cache.TryAdd(metadata.FileName, metadata))
        {
            throw new ArgumentException($"Метадані для файлу '{metadata.FileName}' уже є в кеші.", nameof(metadata));
        }
    }

    /// <summary>Повертає метадані за іменем файлу; якщо їх немає — KeyNotFoundException.</summary>
    public FileMetadata Get(string fileName)
    {
        if (!_cache.TryGetValue(fileName, out FileMetadata? metadata))
        {
            throw new KeyNotFoundException($"Метадані для файлу '{fileName}' не знайдено.");
        }

        return metadata;
    }

    /// <summary>Безпечна версія Get без винятку.</summary>
    public bool TryGet(string fileName, out FileMetadata? metadata) => _cache.TryGetValue(fileName, out metadata);

    /// <summary>Видаляє метадані; повертає true, якщо запис існував.</summary>
    public bool Remove(string fileName) => _cache.Remove(fileName);

    /// <summary>Перевіряє, чи є метадані для файлу.</summary>
    public bool Contains(string fileName) => _cache.ContainsKey(fileName);
}
