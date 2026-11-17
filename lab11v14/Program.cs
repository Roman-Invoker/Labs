using System.Text;
using lab17v14.Data;
using lab17v14.Extensions;
using lab17v14.Models;


Console.OutputEncoding = Encoding.UTF8;

List<Recipe> recipes = SampleData.CreateRecipes();


PrintHeader("Тестові дані: рецепти → інгредієнти → постачальники");
foreach (var recipe in recipes)
{
    Console.WriteLine($"{recipe} — інгредієнтів: {recipe.Ingredients.Count}");
    foreach (var ingredient in recipe.Ingredients)
    {
        string suppliers = string.Join(", ", ingredient.Suppliers.Select(s => s.Name));
        Console.WriteLine($"    - {ingredient.Name}: {suppliers}");
    }
}


PrintHeader("1. SelectMany: унікальні постачальники");

var allSuppliers = recipes
    .SelectMany(r => r.Ingredients)
    .SelectMany(i => i.Suppliers)
    .ToList();


var uniqueSuppliers = allSuppliers
    .DistinctBy(s => s.Id)
    .OrderBy(s => s.Id)
    .ToList();

Console.WriteLine($"Усього записів після SelectMany (з дублікатами): {allSuppliers.Count}");
Console.WriteLine($"Унікальних постачальників: {uniqueSuppliers.Count}");
foreach (var supplier in uniqueSuppliers)
{
    Console.WriteLine($"  {supplier}");
}


var nested = recipes.Select(r => r.Ingredients);
Console.WriteLine($"\nДля порівняння: Select повернув {nested.Count()} вкладених колекцій інгредієнтів, " +
                  $"а SelectMany — {recipes.SelectMany(r => r.Ingredients).Count()} плоских інгредієнтів.");


PrintHeader("2. Пагінація: рецепти по 3 на сторінку");

const int pageSize = 3;
int totalPages = recipes.PageCount(pageSize);

for (int page = 1; page <= totalPages; page++)
{
    Console.WriteLine($"--- Сторінка {page} з {totalPages} ---");
    foreach (var recipe in recipes.GetPage(page, pageSize))
    {
        Console.WriteLine($"  {recipe}");
    }
}


PrintHeader("3. Custom extension: HasIngredient");

foreach (string name in new[] { "Борошно", "лосось", "Шоколад" })
{
    var matches = recipes.Where(r => r.HasIngredient(name)).ToList();
    Console.WriteLine($"Інгредієнт «{name}» є в {matches.Count} рецептах: " +
                      (matches.Count == 0 ? "—" : string.Join(", ", matches.Select(r => r.Name))));
}

Recipe borscht = recipes.First(r => r.Name == "Борщ");
Console.WriteLine($"\nborscht.HasIngredient(\"Буряк\")   = {borscht.HasIngredient("Буряк")}");
Console.WriteLine($"borscht.HasIngredient(\"Цукор\")   = {borscht.HasIngredient("Цукор")}");

static void PrintHeader(string title)
{
    Console.WriteLine();
    Console.WriteLine(new string('=', 70));
    Console.WriteLine(title);
    Console.WriteLine(new string('=', 70));
}
