namespace Lab15v14;

/// <summary>Запис в історії резервних копій: який бекап і які файли до нього увійшли.</summary>
public sealed record BackupRecord(int Id, IReadOnlyList<BackupFile> Files)
{
    public int TotalSizeKb => Files.Sum(f => f.SizeKb);

    public override string ToString() => $"Бекап #{Id}: {Files.Count} файл(ів), {TotalSizeKb} КБ";
}
