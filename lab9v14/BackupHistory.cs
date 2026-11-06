namespace Lab15v14;

/// <summary>
/// Підзадача 3 (Stack): історія резервних копій.
/// Дисципліна LIFO: першим скасовується останній виконаний бекап (Undo).
/// </summary>
public class BackupHistory
{
    private readonly Stack<BackupRecord> _history = new();

    public int Count => _history.Count;

    /// <summary>Перебір іде від новішого запису до старішого.</summary>
    public IReadOnlyCollection<BackupRecord> Records => _history;

    public void Push(BackupRecord record) => _history.Push(record);

    /// <summary>Останній бекап без видалення з історії.</summary>
    public BackupRecord? Peek() => _history.TryPeek(out var record) ? record : null;

    /// <summary>Скасовує останній бекап: знімає запис з вершини стека. Для порожньої історії повертає null.</summary>
    public BackupRecord? Undo() => _history.TryPop(out var record) ? record : null;
}
