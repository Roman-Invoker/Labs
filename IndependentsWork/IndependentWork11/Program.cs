using System.Diagnostics;
using System.Globalization;
using IndependentWork11;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

        // dotnet run                          -> пайплайни на стандартних даних + час
        // dotnet run -c Release -- benchmark  -> залежність часу від кількості продажів
        if (args.Length > 0 && args[0].Equals("benchmark", StringComparison.OrdinalIgnoreCase))
        {
            RunScalabilityBenchmark();
            return;
        }

        RunDemo();
    }

    static void RunDemo()
    {
        Dataset data = DataGenerator.Generate(salesCount: 1000);

        Console.WriteLine("=== IndependentWork11: LINQ-пайплайни (сценарій \"Продажі\") ===");
        Console.WriteLine($"Дані: товарів {data.Products.Count}, клієнтів {data.Customers.Count}, продажів {data.Sales.Count} (Random, seed 42)");
        Console.WriteLine();

        var coldTimes = new List<(string Name, double Ms)>();

        RunPipeline("Пайплайн 1: Топ-3 товари за категоріями",
            () => Pipelines.TopProductsByCategory(data.Sales, data.Products),
            PrintTopProducts, coldTimes);

        RunPipeline("Пайплайн 2: Дохід за містами клієнтів",
            () => Pipelines.RevenueByCity(data.Sales, data.Products, data.Customers),
            PrintRevenueByCity, coldTimes);

        RunPipeline("Пайплайн 3: Продажі за місяцями та роками",
            () => Pipelines.SalesByMonth(data.Sales, data.Products),
            PrintSalesByMonth, coldTimes);

        RunPipeline("Пайплайн 4: Топ-5 клієнтів за витратами з улюбленою категорією",
            () => Pipelines.TopCustomersWithFavoriteCategory(data.Sales, data.Products, data.Customers),
            PrintTopCustomers, coldTimes);

        // Повторні вимірювання після розігріву: середній час одного виклику
        var warmFunctions = new List<Func<object>>
        {
            () => Pipelines.TopProductsByCategory(data.Sales, data.Products),
            () => Pipelines.RevenueByCity(data.Sales, data.Products, data.Customers),
            () => Pipelines.SalesByMonth(data.Sales, data.Products),
            () => Pipelines.TopCustomersWithFavoriteCategory(data.Sales, data.Products, data.Customers)
        };

        Console.WriteLine("=== Зведення вимірювань ===");
        Console.WriteLine($"{"Пайплайн",-10} | {"Перший запуск, мс",18} | {"Середнє після розігріву, мс",28}");
        for (int i = 0; i < coldTimes.Count; i++)
        {
            double warm = Benchmark.MeasureMilliseconds(warmFunctions[i]);
            Console.WriteLine($"{"№" + (i + 1),-10} | {coldTimes[i].Ms,18:F3} | {warm,28:F4}");
        }
    }

    // Для кожного пайплайну створюється новий Stopwatch; друк у вимірювання не входить
    static void RunPipeline<T>(string title, Func<T> pipeline, Action<T> print, List<(string, double)> coldTimes)
    {
        Console.WriteLine($"--- {title} ---");

        var stopwatch = Stopwatch.StartNew();
        T result = pipeline();
        stopwatch.Stop();

        print(result);

        double ms = stopwatch.Elapsed.TotalMilliseconds;
        Console.WriteLine($"Час виконання: {ms:F3} мс (перший запуск, включає JIT-компіляцію)");
        Console.WriteLine();

        coldTimes.Add((title, ms));
    }

    static void PrintTopProducts(List<CategoryTop> result)
    {
        foreach (CategoryTop category in result)
        {
            Console.WriteLine($"{category.Category}:");
            int rank = 1;
            foreach (ProductQuantity product in category.TopProducts)
            {
                Console.WriteLine($"  {rank++}. {product.Name} - {product.Quantity} шт.");
            }
        }
    }

    static void PrintRevenueByCity(List<CityRevenue> result)
    {
        foreach (CityRevenue city in result)
        {
            Console.WriteLine($"{city.City,-12} {city.Revenue,12:N0} грн  ({city.SalesCount} продажів)");
        }
    }

    static void PrintSalesByMonth(List<MonthlySales> result)
    {
        foreach (MonthlySales month in result)
        {
            Console.WriteLine($"{month.Year}-{month.Month:00}  {month.Quantity,5} шт.  {month.Revenue,12:N0} грн");
        }
    }

    static void PrintTopCustomers(List<TopCustomer> result)
    {
        int rank = 1;
        foreach (TopCustomer c in result)
        {
            Console.WriteLine($"{rank++}. {c.Name} ({c.City}): {c.TotalSpent:N0} грн, замовлень: {c.OrdersCount}, улюблена категорія: {c.FavoriteCategory}");
        }
    }

    static void RunScalabilityBenchmark()
    {
#if DEBUG
        Console.WriteLine("УВАГА: це Debug-збірка, результати будуть спотворені. Запустіть: dotnet run -c Release -- benchmark");
        Console.WriteLine();
#endif
        string markdown = Benchmark.RunScalability(new[] { 1_000, 10_000, 100_000, 1_000_000 });

        Console.WriteLine();
        Console.WriteLine(markdown);
        File.WriteAllText("benchmark-results.md", markdown);
        Console.WriteLine("Результати збережено у benchmark-results.md");
    }
}
