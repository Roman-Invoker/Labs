namespace lab17v14.Models;

/// <summary>Інгредієнт з вкладеною колекцією постачальників.</summary>
public class Ingredient
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<Supplier> Suppliers { get; set; } = new();

    public override string ToString() => $"[{Id}] {Name}";
}
