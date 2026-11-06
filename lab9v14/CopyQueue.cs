namespace Lab15v14;

/// <summary>
/// Підзадача 2 (Queue): черга файлів на копіювання.
/// Дисципліна FIFO: першим копіюється той файл, який додали першим.
/// </summary>
public class CopyQueue
{
    private readonly Queue<BackupFile> _queue = new();

    public int Count => _queue.Count;

    public IReadOnlyCollection<BackupFile> Pending => _queue;

    public void Enqueue(BackupFile file) => _queue.Enqueue(file);

    /// <summary>Дивиться, який файл буде скопійовано наступним, не видаляючи його.</summary>
    public BackupFile? Peek() => _queue.TryPeek(out var file) ? file : null;

    /// <summary>«Копіює» наступний файл: забирає його з початку черги. Для порожньої черги повертає null.</summary>
    public BackupFile? CopyNext() => _queue.TryDequeue(out var file) ? file : null;
}
