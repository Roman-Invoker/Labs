namespace lab17v14.Models;

/// <summary>Постачальник інгредієнта.</summary>
public class Supplier
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public override string ToString() => $"[{Id}] {Name}";
}
