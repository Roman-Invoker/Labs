using System.Globalization;
using System.Text;

namespace IndependentWork7;

public static class Program
{
    public static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        DemoStringCache();
        DemoProductCache();
    }

    private static void DemoStringCache()
    {
        Console.WriteLine("=== Cache<string, string>, ємність 3 ===");

        var cache = new Cache<string, string>(3);

        cache.Add("uk", "Україна");
        cache.Add("pl", "Польща");
        cache.Add("de", "Німеччина");
        Console.WriteLine($"Після 3 додавань: Count = {cache.Count}");
        Console.WriteLine($"Get(\"pl\") = {cache.Get("pl")}");

        Console.WriteLine("Додаємо четвертий елемент (\"fr\") — кеш повний, витісняється найстаріший (\"uk\")...");
        cache.Add("fr", "Франція");

        Console.WriteLine($"Count = {cache.Count}");
        foreach (string key in new[] { "uk", "pl", "de", "fr" })
        {
            Console.WriteLine($"  ContainsKey(\"{key}\") = {cache.ContainsKey(key)}");
        }

        try
        {
            cache.Get("uk");
        }
        catch (KeyNotFoundException ex)
        {
            Console.WriteLine($"Get(\"uk\") -> {ex.GetType().Name}: {ex.Message}");
        }

        Console.WriteLine();
    }

    private static void DemoProductCache()
    {
        Console.WriteLine("=== Cache<int, Product>, ємність 2 ===");

        var cache = new Cache<int, Product>(2);

        cache.Add(1, new Product(1, "Ноутбук", 32000m));
        cache.Add(2, new Product(2, "Миша", 450m));
        cache.Add(2, new Product(2, "Миша (оновлено)", 500m)); // оновлення наявного ключа не витісняє нічого
        Console.WriteLine($"Після оновлення ключа 2: Count = {cache.Count}, Get(2) = {Format(cache.Get(2))}");

        cache.Add(3, new Product(3, "Клавіатура", 1500m)); // витісняє ключ 1
        Console.WriteLine($"Після додавання ключа 3: ContainsKey(1) = {cache.ContainsKey(1)}, ContainsKey(2) = {cache.ContainsKey(2)}, ContainsKey(3) = {cache.ContainsKey(3)}");
        Console.WriteLine($"Get(3) = {Format(cache.Get(3))}");
    }

    private static string Format(Product p) =>
        $"{p.Name} — {p.Price.ToString("F2", CultureInfo.InvariantCulture)} грн";
}
