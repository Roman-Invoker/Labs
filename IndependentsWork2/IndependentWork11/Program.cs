using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace IndependentWork11;

public static class Program
{
    private const int WarmRuns = 200;

    public static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

        // Extract: відтворювані тестові дані (Seed = 42)
        var random = new Random(42);
        List<Product> products = DataGenerator.GenerateProducts(random, count: 60);
        List<Customer> customers = DataGenerator.GenerateCustomers(random, count: 80);
        List<Sale> sales = DataGenerator.GenerateSales(random, count: 1000, productCount: products.Count, customerCount: customers.Count);

        Console.WriteLine($"Дані: {products.Count} товарів, {customers.Count} клієнтів, {sales.Count} продажів (Seed = 42)");
        Console.WriteLine();

        RunPipeline1(sales, products);
        RunPipeline2(sales, products, customers);
        RunPipeline3(sales, products);
        RunPipeline4(sales, products, customers);
    }

    private static void RunPipeline1(List<Sale> sales, List<Product> products)
    {
        Console.WriteLine("=== Пайплайн 1: найпопулярніші продукти за категоріями (топ-3) ===");

        var (result, first, average) = Measure(() => SalesPipelines.TopProductsByCategory(sales, products));

        foreach (CategoryTop category in result)
        {
            Console.WriteLine($"{category.Category}:");
            foreach (ProductQuantity p in category.TopProducts)
            {
                Console.WriteLine($"  {p.Name,-20} {p.Quantity,4} шт.");
            }
        }

        PrintTiming(first, average);
    }

    private static void RunPipeline2(List<Sale> sales, List<Product> products, List<Customer> customers)
    {
        Console.WriteLine("=== Пайплайн 2: дохід за містами клієнтів ===");

        var (result, first, average) = Measure(() => SalesPipelines.RevenueByCity(sales, products, customers));

        foreach (CityRevenue city in result)
        {
            Console.WriteLine($"  {city.City,-10} {city.Revenue,14:N2} грн");
        }

        PrintTiming(first, average);
    }

    private static void RunPipeline3(List<Sale> sales, List<Product> products)
    {
        Console.WriteLine("=== Пайплайн 3: продажі за місяцями та роками ===");

        var (result, first, average) = Measure(() => SalesPipelines.SalesByYearMonth(sales, products));

        foreach (MonthlySales m in result)
        {
            Console.WriteLine($"  {m.Year}-{m.Month:00}: {m.Units,4} шт., {m.Revenue,14:N2} грн");
        }

        PrintTiming(first, average);
    }

    private static void RunPipeline4(List<Sale> sales, List<Product> products, List<Customer> customers)
    {
        Console.WriteLine("=== Пайплайн 4 (додатково): топ-5 клієнтів і їхня улюблена категорія ===");

        var (result, first, average) = Measure(() => SalesPipelines.TopCustomersWithFavoriteCategory(sales, products, customers));

        foreach (CustomerSpending c in result)
        {
            Console.WriteLine($"  {c.Name,-12} {c.City,-9} {c.TotalSpent,12:N2} грн  улюблена категорія: {c.FavoriteCategory}");
        }

        PrintTiming(first, average);
    }

    /// <summary>
    /// Вимірює пайплайн новим Stopwatch: перший запуск (разом із JIT-компіляцією) і середнє за <see cref="WarmRuns"/> повторних запусків.
    /// Результат матеріалізується (ToList усередині пайплайну), тому вимірюється повне виконання запиту.
    /// </summary>
    private static (T Result, double FirstMs, double AverageMs) Measure<T>(Func<T> pipeline)
    {
        var firstWatch = Stopwatch.StartNew();
        T result = pipeline();
        firstWatch.Stop();

        var warmWatch = Stopwatch.StartNew();
        for (int i = 0; i < WarmRuns; i++)
        {
            _ = pipeline();
        }

        warmWatch.Stop();

        return (result, firstWatch.Elapsed.TotalMilliseconds, warmWatch.Elapsed.TotalMilliseconds / WarmRuns);
    }

    private static void PrintTiming(double firstMs, double averageMs)
    {
        Console.WriteLine($"  Час: перший запуск (з JIT) = {firstMs:F3} мс; середнє з {WarmRuns} повторів = {averageMs:F3} мс");
        Console.WriteLine();
    }
}
