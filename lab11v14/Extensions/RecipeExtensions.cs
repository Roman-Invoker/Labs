using lab17v14.Models;

namespace lab17v14.Extensions;

public static class RecipeExtensions
{
    /// <summary>
    /// Перевіряє, чи містить рецепт інгредієнт із вказаною назвою
    /// (без урахування регістру та пробілів по краях).
    /// </summary>
    public static bool HasIngredient(this Recipe recipe, string ingredientName)
    {
        ArgumentNullException.ThrowIfNull(recipe);

        if (string.IsNullOrWhiteSpace(ingredientName))
        {
            return false;
        }

        var name = ingredientName.Trim();
        return recipe.Ingredients.Any(i =>
            string.Equals(i.Name, name, StringComparison.OrdinalIgnoreCase));
    }
}
