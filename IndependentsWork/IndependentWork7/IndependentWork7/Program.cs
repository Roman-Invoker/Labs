using IndependentWork7;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine("=== Демонстрація FIFO-кешу (ємність 3) ===");
        Cache<string, string> cache = new Cache<string, string>(3);

        AddAndPrint(cache, "user:1", "Олена");
        AddAndPrint(cache, "user:2", "Богдан");
        AddAndPrint(cache, "user:3", "Соломія");
        Console.WriteLine($"Count = {cache.Count}");
        Console.WriteLine($"Get(\"user:2\") = {cache.Get("user:2")}");

        Console.WriteLine();
        AddAndPrint(cache, "user:4", "Тарас", " (кеш повний, видаляється найстаріший)");
        PrintPresence(cache, "user:1", "user:2", "user:3", "user:4");
        Console.WriteLine($"Count = {cache.Count}");

        // Get("user:2") було вище, але в FIFO читання не впливає на порядок витіснення
        Console.WriteLine();
        AddAndPrint(cache, "user:5", "Марія", " (кеш повний, видаляється найстаріший)");
        PrintPresence(cache, "user:1", "user:2", "user:3", "user:4", "user:5");
        Console.WriteLine($"Count = {cache.Count}");

        Console.WriteLine();
        try
        {
            cache.Get("user:1");
        }
        catch (KeyNotFoundException ex)
        {
            Console.WriteLine($"Помилка: {ex.Message}");
        }
    }

    static void AddAndPrint(Cache<string, string> cache, string key, string value, string note = "")
    {
        cache.Add(key, value);
        Console.WriteLine($"Add: {key} -> {value}{note}");
    }

    static void PrintPresence(Cache<string, string> cache, params string[] keys)
    {
        foreach (string key in keys)
        {
            Console.WriteLine($"{key} у кеші: {cache.ContainsKey(key)}");
        }
    }
}
