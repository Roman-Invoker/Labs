using System.Globalization;
using Lab9SelfV14;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

        DemoIntegers();
        Console.WriteLine();
        DemoStrings();
        Console.WriteLine();
        DemoProducts();
        Console.WriteLine();
        DemoLinqComparison();
    }

    // ---------- Колекція 1: List<int> ----------
    static void DemoIntegers()
    {
        Console.WriteLine("=== 1. List<int> ===");
        var numbers = new List<int> { 5, 12, 8, 21, 3, 17, 10 };
        Console.WriteLine($"Числа: {Join(numbers)}");

        // Map: лямбда-вираз піднесення до квадрата
        var squares = CollectionUtils.Map(numbers, n => n * n);
        Console.WriteLine($"Map (квадрати): {Join(squares)}");

        // Filter: лямбда-предикат парності
        var evens = CollectionUtils.Filter(numbers, n => n % 2 == 0);
        Console.WriteLine($"Filter (парні): {Join(evens)}");

        // MaxBy: два різні селектори ключа
        Console.WriteLine($"MaxBy (найбільше): {CollectionUtils.MaxBy(numbers, n => n)}");
        Console.WriteLine($"MaxBy (найбільша остання цифра): {CollectionUtils.MaxBy(numbers, n => n % 10)}");

        // ForEach: дія над кожним елементом
        Console.Write("ForEach (парні по одному):");
        CollectionUtils.ForEach(evens, n => Console.Write($" [{n}]"));
        Console.WriteLine();

        // ForEach із замиканням: лямбда змінює локальну змінну
        int sum = 0;
        CollectionUtils.ForEach(numbers, n => sum += n);
        Console.WriteLine($"ForEach (сума через замикання): {sum}");
    }

    // ---------- Колекція 2: List<string> ----------
    static void DemoStrings()
    {
        Console.WriteLine("=== 2. List<string> ===");
        var words = new List<string> { "яблуко", "груша", "апельсин", "слива", "ківі" };
        Console.WriteLine($"Слова: {Join(words)}");

        Console.WriteLine($"Map (верхній регістр): {Join(CollectionUtils.Map(words, w => w.ToUpper()))}");
        Console.WriteLine($"Map (довжини): {Join(CollectionUtils.Map(words, w => w.Length))}");
        Console.WriteLine($"Filter (довші за 5 символів): {Join(CollectionUtils.Filter(words, w => w.Length > 5))}");
        Console.WriteLine($"MaxBy (найдовше слово): {CollectionUtils.MaxBy(words, w => w.Length)}");
        Console.WriteLine($"MaxBy (останнє за алфавітом): {CollectionUtils.MaxBy(words, w => w)}");

        Console.WriteLine("ForEach:");
        CollectionUtils.ForEach(words, w => Console.WriteLine($" - {w}"));
    }

    // ---------- Колекція 3: List<Product> ----------
    static void DemoProducts()
    {
        Console.WriteLine("=== 3. List<Product> ===");
        var products = new List<Product>
        {
            new Product { Name = "Laptop", Price = 25000m },
            new Product { Name = "Mouse", Price = 500m },
            new Product { Name = "Keyboard", Price = 1500m },
            new Product { Name = "Monitor", Price = 8000m },
            new Product { Name = "Headphones", Price = 1200m }
        };

        Console.WriteLine("ForEach (усі товари):");
        CollectionUtils.ForEach(products, p => Console.WriteLine($"{p.Name}: {p.Price} грн"));

        Console.WriteLine($"Map (назви): {Join(CollectionUtils.Map(products, p => p.Name))}");
        Console.WriteLine($"Map (ціна зі знижкою 10%): {Join(CollectionUtils.Map(products, p => (p.Price * 0.9m).ToString("F2")))}");

        var mostExpensive = CollectionUtils.MaxBy(products, p => p.Price);
        Console.WriteLine($"MaxBy (найдорожчий): {mostExpensive.Name} - {mostExpensive.Price}");
        Console.WriteLine($"MaxBy (найдовша назва): {CollectionUtils.MaxBy(products, p => p.Name.Length).Name}");

        var expensive = CollectionUtils.Filter(products, p => p.Price > 1000);
        Console.WriteLine("Filter (дорожчі за 1000):");
        CollectionUtils.ForEach(expensive, p => Console.WriteLine($" - {p.Name}"));

        var middle = CollectionUtils.Filter(products, p => p.Price > 1000 && p.Price < 10000);
        Console.WriteLine($"Filter (від 1000 до 10000 грн): {Join(CollectionUtils.Map(middle, p => p.Name))}");

        decimal total = 0;
        CollectionUtils.ForEach(products, p => total += p.Price);
        Console.WriteLine($"ForEach (загальна вартість через замикання): {total} грн");

        // Ланцюжок: результат одного методу передається наступному
        Console.WriteLine("Ланцюжок Filter -> Map -> ForEach:");
        CollectionUtils.ForEach(
            CollectionUtils.Map(CollectionUtils.Filter(products, p => p.Price > 1000), p => p.Name.ToUpper()),
            Console.WriteLine);
    }

    // ---------- Порівняння з LINQ ----------
    static void DemoLinqComparison()
    {
        Console.WriteLine("=== 4. Порівняння з LINQ ===");
        var numbers = new List<int> { 5, 12, 8, 21, 3, 17, 10 };
        var products = new List<Product>
        {
            new Product { Name = "Laptop", Price = 25000m },
            new Product { Name = "Mouse", Price = 500m },
            new Product { Name = "Keyboard", Price = 1500m }
        };

        Console.WriteLine($"Map == Select: {CollectionUtils.Map(numbers, n => n * n).SequenceEqual(numbers.Select(n => n * n))}");
        Console.WriteLine($"Filter == Where: {CollectionUtils.Filter(numbers, n => n % 2 == 0).SequenceEqual(numbers.Where(n => n % 2 == 0))}");
        Console.WriteLine($"MaxBy == Enumerable.MaxBy: {ReferenceEquals(CollectionUtils.MaxBy(products, p => p.Price), products.MaxBy(p => p.Price))}");

        // Eager (власний Map) проти deferred (LINQ Select)
        int calls = 0;
        var mapped = CollectionUtils.Map(numbers, n => { calls++; return n * 2; });
        Console.WriteLine($"Виклики лямбди одразу після Map: {calls}");

        calls = 0;
        var selected = numbers.Select(n => { calls++; return n * 2; });
        Console.WriteLine($"Виклики лямбди одразу після Select: {calls}");
        selected.ToList();
        Console.WriteLine($"Виклики лямбди Select після ToList(): {calls}");

        try
        {
            CollectionUtils.MaxBy(new List<int>(), n => n);
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Помилка для порожньої колекції: {ex.Message}");
        }
    }

    static string Join<T>(IEnumerable<T> items)
    {
        return string.Join(", ", items);
    }
}
