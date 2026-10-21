namespace InvertedIndexDemo;

/// <summary>
/// Документ, який індексується та шукається.
/// </summary>
public class Document
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;

    public override string ToString() => $"[{Id}] {Title}";
}
