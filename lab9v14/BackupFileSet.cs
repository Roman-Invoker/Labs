namespace Lab15v14;

/// <summary>
/// Підзадача 1 (HashSet): унікальні файли для бекапу.
/// Дублікати відсікаються автоматично, перевірка наявності працює за O(1).
/// </summary>
public class BackupFileSet
{
    private readonly HashSet<BackupFile> _files;

    public BackupFileSet(IEnumerable<BackupFile>? files = null)
    {
        _files = files is null ? new HashSet<BackupFile>() : new HashSet<BackupFile>(files);
    }

    public int Count => _files.Count;

    /// <summary>Порядок перебору у HashSet не гарантується.</summary>
    public IReadOnlyCollection<BackupFile> Files => _files;

    /// <summary>Повертає false, якщо такий файл уже є (дублікат).</summary>
    public bool Add(BackupFile file) => _files.Add(file);

    public bool Remove(BackupFile file) => _files.Remove(file);

    public bool Contains(BackupFile file) => _files.Contains(file);

    /// <summary>Об'єднання: усі файли з обох наборів.</summary>
    public void UnionWith(IEnumerable<BackupFile> other) => _files.UnionWith(other);

    /// <summary>Перетин: залишаються лише файли, що є в обох наборах.</summary>
    public void IntersectWith(IEnumerable<BackupFile> other) => _files.IntersectWith(other);

    /// <summary>Різниця: прибираються файли, що є в <paramref name="other"/>.</summary>
    public void ExceptWith(IEnumerable<BackupFile> other) => _files.ExceptWith(other);
}
