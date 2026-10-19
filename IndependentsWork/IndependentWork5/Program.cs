using System.Globalization;
using System.Text;
using IndependentWork5;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture; // однаковий формат чисел на будь-якому ПК

        // ---------- Сценарій 1: System.Object та ToString() ----------
        Console.WriteLine("=== Сценарій 1: Object і ToString() ===");

        Product product = new Product("Ноутбук", 35000m);
        Customer customer = new Customer("Олена Коваль", "olena@example.com");
        Order order = new Order(1, customer, product, 2);
        Warehouse warehouse = new Warehouse("Центральний склад");

        List<object> items = new List<object> { product, customer, order, warehouse };

        foreach (object item in items)
        {
            // Console.WriteLine(object) сам викликає item.ToString() - віртуальний виклик
            Console.WriteLine($"{item.GetType().Name,-10} -> {item}");
        }

        // ---------- Сценарій 2: IEnumerable<string> ----------
        Console.WriteLine();
        Console.WriteLine("=== Сценарій 2: IEnumerable<string> ===");

        MyCustomCollection collection = new MyCustomCollection();
        collection.Add("Яблуко");
        collection.Add("Груша");
        collection.Add("Слива");

        foreach (string fruit in collection)
        {
            Console.WriteLine($"Елемент: {fruit}");
        }

        // Один метод працює з різними колекціями, бо всі вони є IEnumerable<string>
        PrintAll("MyCustomCollection", collection);
        PrintAll("string[]", new[] { "Червоний", "Зелений" });
        PrintAll("List<string>", new List<string> { "Один", "Два", "Три" });

        // Бонус: LINQ працює з нашою колекцією, бо вона реалізує IEnumerable<string>
        Console.WriteLine($"LINQ Count(): {collection.Count()}");

        // ---------- Сценарій 3: абстрактний клас Stream ----------
        Console.WriteLine();
        Console.WriteLine("=== Сценарій 3: Stream ===");

        // Тестовий файл створюється в коді, тому проєкт самодостатній
        string filePath = "test.txt";
        File.WriteAllText(filePath, "Hello from FileStream! Polymorphism in .NET.");

        using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            ProcessStream(fileStream);
        }

        byte[] data = Encoding.ASCII.GetBytes("Memory stream data for ProcessStream.");
        using (MemoryStream memoryStream = new MemoryStream(data))
        {
            ProcessStream(memoryStream);
        }
    }

    // Приймає IEnumerable<string>: підходить будь-яка колекція рядків
    static void PrintAll(string title, IEnumerable<string> source)
    {
        Console.WriteLine($"{title}: {string.Join(", ", source)}");
    }

    // Приймає абстрактний Stream: підходить FileStream, MemoryStream та будь-який інший нащадок
    static void ProcessStream(Stream stream)
    {
        Console.WriteLine($"Тип потоку: {stream.GetType().Name}");

        string length = stream.CanSeek ? stream.Length.ToString() : "невідомо";
        Console.WriteLine($"  CanRead: {stream.CanRead}, CanSeek: {stream.CanSeek}, Length: {length}");

        byte[] buffer = new byte[16];
        int bytesRead = stream.Read(buffer, 0, buffer.Length); // віртуальний виклик: у кожного потоку своя реалізація
        string text = Encoding.ASCII.GetString(buffer, 0, bytesRead);

        Console.WriteLine($"  Прочитано {bytesRead} байтів: \"{text}\"");
    }
}
