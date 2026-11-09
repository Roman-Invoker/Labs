using System.Globalization;
using System.Text;

namespace Lab9SelfV14;

public static class Program
{
    public static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

        DemoInts();
        DemoStrings();
        DemoProducts();
        DemoEdgeCases();
    }

    // Колекція 1: List<int>
    private static void DemoInts()
    {
        Console.WriteLine("=== List<int> ===");

        var numbers = new List<int> { 4, 15, 8, 23, 16, 42, 7 };
        Console.WriteLine("Вхідні числа: " + string.Join(", ", numbers));

        var squares = CollectionUtils.Map(numbers, n => n * n);
        Console.WriteLine("Map (n => n * n): " + string.Join(", ", squares));

        var asText = CollectionUtils.Map(numbers, n => n % 2 == 0 ? $"{n} (парне)" : $"{n} (непарне)");
        Console.WriteLine("Map (число -> текст): " + string.Join("; ", asText));

        int max = CollectionUtils.MaxBy(numbers, n => n);
        int maxByLastDigit = CollectionUtils.MaxBy(numbers, n => n % 10);
        Console.WriteLine($"MaxBy (n => n): {max}");
        Console.WriteLine($"MaxBy (n => n % 10, найбільша остання цифра): {maxByLastDigit}");

        var evens = CollectionUtils.Filter(numbers, n => n % 2 == 0);
        Console.WriteLine("Filter (парні): " + string.Join(", ", evens));

        var big = CollectionUtils.Filter(numbers, n => n > 15);
        Console.Write("ForEach над Filter (n > 15): ");
        CollectionUtils.ForEach(big, n => Console.Write($"[{n}] "));
        Console.WriteLine();
        Console.WriteLine();
    }

    // Колекція 2: List<string>
    private static void DemoStrings()
    {
        Console.WriteLine("=== List<string> ===");

        var words = new List<string> { "delegate", "lambda", "func", "action", "predicate", "generic" };
        Console.WriteLine("Вхідні слова: " + string.Join(", ", words));

        var lengths = CollectionUtils.Map(words, w => w.Length);
        Console.WriteLine("Map (довжини): " + string.Join(", ", lengths));

        var upper = CollectionUtils.Map(words, w => w.ToUpperInvariant());
        Console.WriteLine("Map (ToUpper): " + string.Join(", ", upper));

        string longest = CollectionUtils.MaxBy(words, w => w.Length);
        string lastAlphabetically = CollectionUtils.MaxBy(words, w => w);
        Console.WriteLine($"MaxBy (найдовше слово): {longest}");
        Console.WriteLine($"MaxBy (останнє за алфавітом): {lastAlphabetically}");

        var withA = CollectionUtils.Filter(words, w => w.Contains('a'));
        Console.WriteLine("Filter (містять 'a'): " + string.Join(", ", withA));

        Console.WriteLine("ForEach (нумерований вивід):");
        int index = 1;
        CollectionUtils.ForEach(words, w => Console.WriteLine($"  {index++}. {w}"));
        Console.WriteLine();
    }

    // Колекція 3: List<Product>
    private static void DemoProducts()
    {
        Console.WriteLine("=== List<Product> ===");

        var products = new List<Product>
        {
            new() { Name = "Laptop", Category = "Electronics", Price = 25000m },
            new() { Name = "Mouse", Category = "Electronics", Price = 500m },
            new() { Name = "Keyboard", Category = "Electronics", Price = 1500m },
            new() { Name = "Notebook", Category = "Stationery", Price = 60m },
            new() { Name = "Backpack", Category = "Accessories", Price = 1200m },
        };

        var names = CollectionUtils.Map(products, p => p.Name);
        Console.WriteLine("Map (імена): " + string.Join(", ", names));

        var priceTags = CollectionUtils.Map(products, p => $"{p.Name}: {p.Price:F2} грн");
        Console.WriteLine("Map (цінники): " + string.Join(" | ", priceTags));

        Product mostExpensive = CollectionUtils.MaxBy(products, p => p.Price);
        Product longestName = CollectionUtils.MaxBy(products, p => p.Name.Length);
        Console.WriteLine($"MaxBy (найдорожчий): {mostExpensive.Name} — {mostExpensive.Price:F2} грн");
        Console.WriteLine($"MaxBy (найдовша назва): {longestName.Name}");

        var expensive = CollectionUtils.Filter(products, p => p.Price > 1000m);
        Console.WriteLine("Filter (ціна > 1000):");
        CollectionUtils.ForEach(expensive, p => Console.WriteLine($"  - {p.Name} ({p.Category})"));

        var electronics = CollectionUtils.Filter(products, p => p.Category == "Electronics");
        CollectionUtils.ForEach(electronics, p => p.Price *= 1.10m);
        Console.WriteLine("ForEach (ціна Electronics +10%):");
        CollectionUtils.ForEach(electronics, p => Console.WriteLine($"  {p.Name}: {p.Price:F2} грн"));
        Console.WriteLine();
    }

    // Граничні випадки
    private static void DemoEdgeCases()
    {
        Console.WriteLine("=== Граничні випадки ===");

        try
        {
            CollectionUtils.MaxBy(new List<int>(), n => n);
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"MaxBy(порожня колекція) -> {ex.GetType().Name}: {ex.Message}");
        }

        var empty = CollectionUtils.Filter(new List<int> { 1, 3, 5 }, n => n % 2 == 0);
        Console.WriteLine($"Filter(жоден не підходить) -> {{ {string.Join(", ", empty)} }}");
    }
}
