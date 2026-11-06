namespace Lab15v14;

/// <summary>
/// Зв'язує три колекції в єдиний сценарій резервного копіювання:
/// HashSet відсікає дублікати, Queue задає порядок копіювання, Stack зберігає історію для Undo.
/// </summary>
public class BackupService
{
    private readonly BackupFileSet _files = new();
    private readonly CopyQueue _queue = new();
    private readonly BackupHistory _history = new();
    private int _nextId = 1;

    public int UniqueCount => _files.Count;

    public int PendingCount => _queue.Count;

    public int BackupsCount => _history.Count;

    /// <summary>Ставить файл у чергу, лише якщо його ще не планували (перевірка через HashSet).</summary>
    public bool Schedule(BackupFile file)
    {
        if (!_files.Add(file))
        {
            return false;
        }

        _queue.Enqueue(file);
        return true;
    }

    /// <summary>Копіює до <paramref name="maxFiles"/> файлів з початку черги й записує бекап в історію.</summary>
    public BackupRecord? RunBackup(int maxFiles)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxFiles);

        var batch = new List<BackupFile>();
        while (batch.Count < maxFiles && _queue.CopyNext() is { } file)
        {
            batch.Add(file);
        }

        if (batch.Count == 0)
        {
            return null;
        }

        var record = new BackupRecord(_nextId++, batch);
        _history.Push(record);
        return record;
    }

    /// <summary>Скасовує останній бекап: його файли повертаються в чергу для повторного копіювання.</summary>
    public BackupRecord? UndoLastBackup()
    {
        var record = _history.Undo();
        if (record is null)
        {
            return null;
        }

        foreach (var file in record.Files)
        {
            _queue.Enqueue(file);
        }

        return record;
    }
}
