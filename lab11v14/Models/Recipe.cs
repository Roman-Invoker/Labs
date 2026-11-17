namespace lab17v14.Models;

/// <summary>Рецепт з вкладеною колекцією інгредієнтів.</summary>
public class Recipe
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<Ingredient> Ingredients { get; set; } = new();

    public override string ToString() => $"[{Id}] {Name}";
}
