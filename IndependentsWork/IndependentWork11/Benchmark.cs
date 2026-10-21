using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace IndependentWork11;

/// <summary>
/// Вимірювання продуктивності пайплайнів.
/// </summary>
public static class Benchmark
{
    // Результат присвоюється сюди, щоб JIT не вилучив виклик як мертвий код
    private static object? _sink;

    /// <summary>
    /// Середній час одного виклику в мілісекундах. Кількість повторів подвоюється,
    /// доки пакет не триватиме щонайменше 100 мс (ранні пакети слугують розігрівом JIT).
    /// </summary>
    public static double MeasureMilliseconds(Func<object> action)
    {
        long iterations = 1;

        while (true)
        {
            var sw = Stopwatch.StartNew();
            for (long i = 0; i < iterations; i++)
            {
                _sink = action();
            }
            sw.Stop();

            if (sw.ElapsedMilliseconds >= 100 || iterations >= (1L << 24))
            {
                return sw.Elapsed.TotalMilliseconds / iterations;
            }

            iterations *= 2;
        }
    }

    /// <summary>
    /// Залежність часу від кількості продажів. Повертає Markdown для REPORT.md.
    /// </summary>
    public static string RunScalability(int[] salesCounts)
    {
        var inv = CultureInfo.InvariantCulture;
        var sb = new StringBuilder();

        sb.AppendLine($"- .NET: {RuntimeInformation.FrameworkDescription}");
        sb.AppendLine($"- ОС: {RuntimeInformation.OSDescription}");
        sb.AppendLine($"- Логічних процесорів: {Environment.ProcessorCount}");
#if DEBUG
        sb.AppendLine("- Конфігурація: Debug (УВАГА: для коректних вимірювань запускайте з -c Release)");
#else
        sb.AppendLine("- Конфігурація: Release");
#endif
        sb.AppendLine("- Товарів: 60, клієнтів: 80 (сталі), кількість продажів змінюється");
        sb.AppendLine();
        sb.AppendLine("| Продажів | Пайплайн 1, мс | Пайплайн 2, мс | Пайплайн 3, мс | Пайплайн 4, мс | Пайплайн 2 (цикли), мс | Збіг LINQ / цикли |");
        sb.AppendLine("|---:|---:|---:|---:|---:|---:|:---:|");

        foreach (int count in salesCounts)
        {
            Dataset data = DataGenerator.Generate(count);
            Console.WriteLine($"Вимірювання для {count} продажів...");

            double p1 = MeasureMilliseconds(() => Pipelines.TopProductsByCategory(data.Sales, data.Products));
            double p2 = MeasureMilliseconds(() => Pipelines.RevenueByCity(data.Sales, data.Products, data.Customers));
            double p3 = MeasureMilliseconds(() => Pipelines.SalesByMonth(data.Sales, data.Products));
            double p4 = MeasureMilliseconds(() => Pipelines.TopCustomersWithFavoriteCategory(data.Sales, data.Products, data.Customers));
            double p2Manual = MeasureMilliseconds(() => Pipelines.RevenueByCityManual(data.Sales, data.Products, data.Customers));

            bool same = Pipelines.RevenueByCity(data.Sales, data.Products, data.Customers)
                .SequenceEqual(Pipelines.RevenueByCityManual(data.Sales, data.Products, data.Customers));

            sb.AppendLine($"| {count} | {p1.ToString("F3", inv)} | {p2.ToString("F3", inv)} | {p3.ToString("F3", inv)} | {p4.ToString("F3", inv)} | {p2Manual.ToString("F3", inv)} | {(same ? "так" : "НІ (!)")} |");
        }

        return sb.ToString();
    }
}
