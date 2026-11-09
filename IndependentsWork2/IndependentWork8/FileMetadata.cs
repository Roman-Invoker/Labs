namespace IndependentWork8;

/// <summary>Метадані файлу, які зберігаються в кеші.</summary>
public class FileMetadata
{
    public string FileName { get; set; } = string.Empty;
    public long Size { get; set; }
    public DateTime LastModified { get; set; }
    public string Hash { get; set; } = string.Empty;

    public override string ToString() =>
        $"{FileName} | {Size} байт | {LastModified:yyyy-MM-dd HH:mm} | хеш {Hash}";
}
