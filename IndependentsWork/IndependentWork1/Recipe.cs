namespace IndependentWork1;

/// <summary>
/// Кулінарний рецепт: порції, час приготування, калорійність.
/// </summary>
public class Recipe
{
    // Приватні поля
    private string _title;
    private int _servings;
    private int _caloriesPerServing;
    private int _cookingMinutes;

    
    public string Title
    {
        get { return _title; }
        set { _title = value; }
    }

    
    public int Servings
    {
        get { return _servings; }
        private set { _servings = value > 0 ? value : 1; }
    }

   
    public int CookingMinutes
    {
        get { return _cookingMinutes; }
    }

   
    public Recipe(string title, int servings, int caloriesPerServing, int cookingMinutes)
    {
        _title = title;
        Servings = servings;
        _caloriesPerServing = caloriesPerServing;
        _cookingMinutes = cookingMinutes;
    }

   
    public int CalculateTotalCalories()
    {
        return _caloriesPerServing * _servings;
    }

   
    public bool IsQuick()
    {
        return _cookingMinutes <= 30;
    }

    
    public void ScaleServings(int newServings)
    {
        if (newServings <= 0)
        {
            Console.WriteLine("Кількість порцій має бути додатною.");
            return;
        }

        Servings = newServings;
    }

    public void PrintInfo()
    {
        Console.WriteLine($"Рецепт \"{_title}\": порцій = {_servings}, " +
                          $"час = {_cookingMinutes} хв, " +
                          $"всього калорій = {CalculateTotalCalories()}, " +
                          $"швидкий: {(IsQuick() ? "так" : "ні")}");
    }
}
