using System.Globalization;
using System.Text;
using Lab14v14;

namespace Lab14v14;

public static class Program
{
    public static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

        Console.WriteLine("=== ЛР №8, варіант 14: CollectionHelper<T>.Take ===");

        DemoInts();
        DemoStrings();
        DemoProducts();
        DemoEdgeCases();
        DemoLazyEvaluation();
    }

    private static void DemoInts()
    {
        Console.WriteLine("\n--- 1. List<int> ---");
        var numbers = new List<int> { 10, 20, 30, 40, 50, 60, 70 };

        Show("Вхідна колекція", numbers, n => n.ToString());
        Show("Take(5)", CollectionHelper<int>.Take(numbers, 5), n => n.ToString());
    }

    private static void DemoStrings()
    {
        Console.WriteLine("\n--- 2. List<string> ---");
        var words = new List<string> { "apple", "banana", "cherry", "date", "elderberry" };

        Show("Вхідна колекція", words, w => w);
        Show("Take(3)", CollectionHelper<string>.Take(words, 3), w => w);
        Show("Take(2), великими літерами", CollectionHelper<string>.Take(words, 2), w => w.ToUpperInvariant());
    }

    private static void DemoProducts()
    {
        Console.WriteLine("\n--- 3. List<Product> ---");
        var products = new List<Product>
        {
            new("Laptop", 1200m, "Electronics"),
            new("Smartphone", 800.50m, "Electronics"),
            new("Desk", 250m, "Furniture"),
            new("Chair", 99.99m, "Furniture"),
            new("Notebook", 4.50m, "Stationery"),
        };

        Show("Вхідна колекція", products, p => p.Name);
        Show("Take(3)", CollectionHelper<Product>.Take(products, 3), p => $"{p.Name} ({p.Price:F2} грн)");
    }

    private static void DemoEdgeCases()
    {
        Console.WriteLine("\n--- 4. Граничні випадки ---");
        var words = new List<string> { "apple", "banana", "cherry", "date", "elderberry" };

        Show("count = 0", CollectionHelper<string>.Take(words, 0), w => w);
        Show("count = -3", CollectionHelper<string>.Take(words, -3), w => w);
        Show("count = 100 (більше за розмір)", CollectionHelper<string>.Take(words, 100), w => w);
        Show("порожня колекція, count = 3", CollectionHelper<int>.Take(new List<int>(), 3), n => n.ToString());

        try
        {
            CollectionHelper<int>.Take(null!, 3);
        }
        catch (ArgumentNullException ex)
        {
            Console.WriteLine($"null-колекція: ArgumentNullException, параметр: {ex.ParamName}");
        }
    }

    private static void DemoLazyEvaluation()
    {
        Console.WriteLine("\n--- 5. Ліниве обчислення ---");

        // Нескінченні послідовності: Take коректно завершується, бо не читає зайвого.
        Show("квадрати, Take(5)", CollectionHelper<int>.Take(Generate(i => i * i), 5), n => n.ToString());
        Show("рядки, Take(3)", CollectionHelper<string>.Take(Generate(i => $"item-{i}"), 3), s => s);

        Console.WriteLine("Джерело з логуванням, Take(2):");
        foreach (int value in CollectionHelper<int>.Take(LoggedSource(), 2))
        {
            Console.WriteLine($"  отримано {value}");
        }
    }

    /// <summary>Нескінченна послідовність, елементи якої породжує лямбда-вираз.</summary>
    private static IEnumerable<TItem> Generate<TItem>(Func<int, TItem> factory)
    {
        for (int i = 0; ; i++)
        {
            yield return factory(i);
        }
    }

    /// <summary>Нескінченне джерело, що повідомляє про кожен виданий елемент.</summary>
    private static IEnumerable<int> LoggedSource()
    {
        for (int i = 1; ; i++)
        {
            Console.WriteLine($"  [джерело] видано {i}");
            yield return i;
        }
    }

    /// <summary>Виводить колекцію у форматі "мітка: [a, b, c]" за допомогою делегата-форматера.</summary>
    private static void Show<TItem>(string label, IEnumerable<TItem> items, Func<TItem, string> format)
    {
        var sb = new StringBuilder();
        foreach (TItem item in items)
        {
            if (sb.Length > 0)
            {
                sb.Append(", ");
            }

            sb.Append(format(item));
        }

        Console.WriteLine($"{label}: [{sb}]");
    }
}
