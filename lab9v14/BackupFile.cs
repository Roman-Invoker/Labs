namespace Lab15v14;

/// <summary>
/// Файл, що підлягає резервному копіюванню.
/// Два файли вважаються однаковими, якщо збігаються шляхи (без урахування регістру).
/// Саме це робить <see cref="HashSet{T}"/> «набором унікальних файлів».
/// </summary>
public sealed class BackupFile : IEquatable<BackupFile>
{
    public BackupFile(string path, int sizeKb)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentOutOfRangeException.ThrowIfNegative(sizeKb);

        Path = path;
        SizeKb = sizeKb;
    }

    public string Path { get; }

    public int SizeKb { get; }

    public bool Equals(BackupFile? other) =>
        other is not null && string.Equals(Path, other.Path, StringComparison.OrdinalIgnoreCase);

    public override bool Equals(object? obj) => Equals(obj as BackupFile);

    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Path);

    public override string ToString() => $"{Path} ({SizeKb} КБ)";
}
