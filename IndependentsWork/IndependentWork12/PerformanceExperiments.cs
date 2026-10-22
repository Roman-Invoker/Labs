using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace IndependentWork12;

public record ExperimentRow(int Size, string Operation, double LinqMs, double PlinqMs, bool ResultsMatch, int ResultCount);

public record ParallelismRow(int Degree, double Milliseconds);

/// <summary>
/// Порівняння продуктивності LINQ і PLINQ (завдання 2-4).
/// </summary>
public static class PerformanceExperiments
{
    /// <summary>
    /// Колекція випадкових чисел від 1 до 1 000 000 (фіксований seed - дані однакові під час кожного запуску).
    /// </summary>
    public static List<int> GenerateData(int size, int seed = 42)
    {
        var random = new Random(seed);
        var data = new List<int>(size);
        for (int i = 0; i < size; i++)
        {
            data.Add(random.Next(1, 1_000_001));
        }
        return data;
    }

    public static List<double> RunLinq(List<int> data, Operation op)
    {
        return data.Where(op.Predicate).Select(op.Selector).ToList();
    }

    public static List<double> RunPlinq(List<int> data, Operation op, int? degreeOfParallelism = null)
    {
        ParallelQuery<int> query = data.AsParallel();
        if (degreeOfParallelism.HasValue)
        {
            query = query.WithDegreeOfParallelism(degreeOfParallelism.Value);
        }

        return query.Where(op.Predicate).Select(op.Selector).ToList();
    }

    /// <summary>
    /// Експеримент 1: кожна операція на кожному розмірі колекції, LINQ проти PLINQ.
    /// Час - середнє за runs запусків після розігріву.
    /// </summary>
    public static List<ExperimentRow> RunExperiments(int[] sizes, int runs = 3)
    {
        var rows = new List<ExperimentRow>();

        // Розігрів JIT на невеликій колекції, щоб компіляція не потрапила у вимірювання
        List<int> warmUp = GenerateData(50_000);
        foreach (Operation op in Operations.All)
        {
            RunLinq(warmUp, op);
            RunPlinq(warmUp, op);
        }

        foreach (int size in sizes)
        {
            List<int> data = GenerateData(size);

            foreach (Operation op in Operations.All)
            {
                Console.WriteLine($"Розмір {size:N0}, операція \"{op.Name}\"...");

                double linqMs = AverageMilliseconds(runs, () => RunLinq(data, op), out List<double> linqResult);
                double plinqMs = AverageMilliseconds(runs, () => RunPlinq(data, op), out List<double> plinqResult);

                rows.Add(new ExperimentRow(size, op.Name, linqMs, plinqMs, ResultsMatch(linqResult, plinqResult), linqResult.Count));
            }
        }

        return rows;
    }

    /// <summary>
    /// Експеримент 2: залежність часу від кількості потоків (WithDegreeOfParallelism).
    /// </summary>
    public static (double LinqMs, List<ParallelismRow> Rows) RunParallelismScaling(Operation op, int size, int runs = 3)
    {
        List<int> data = GenerateData(size);
        RunLinq(GenerateData(50_000), op); // розігрів

        double linqMs = AverageMilliseconds(runs, () => RunLinq(data, op), out _);

        var degrees = new[] { 1, 2, 4, Environment.ProcessorCount }
            .Where(d => d <= Environment.ProcessorCount)
            .Distinct()
            .OrderBy(d => d)
            .ToList();

        var rows = new List<ParallelismRow>();
        foreach (int degree in degrees)
        {
            Console.WriteLine($"Потоків: {degree}...");
            double ms = AverageMilliseconds(runs, () => RunPlinq(data, op, degree), out _);
            rows.Add(new ParallelismRow(degree, ms));
        }

        return (linqMs, rows);
    }

    private static double AverageMilliseconds(int runs, Func<List<double>> action, out List<double> lastResult)
    {
        double total = 0;
        List<double> last = new List<double>();

        for (int i = 0; i < runs; i++)
        {
            // Прибирання сміття перед кожним запуском зменшує вплив GC на вимірювання
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            var stopwatch = Stopwatch.StartNew();
            last = action();
            stopwatch.Stop();

            total += stopwatch.Elapsed.TotalMilliseconds;
        }

        lastResult = last;
        return total / runs;
    }

    /// <summary>
    /// PLINQ без AsOrdered не гарантує порядок, тому порівнюємо кількість і суму
    /// (з допуском на різний порядок додавання чисел з плаваючою комою).
    /// </summary>
    private static bool ResultsMatch(List<double> linq, List<double> plinq)
    {
        if (linq.Count != plinq.Count)
        {
            return false;
        }

        double a = linq.Sum();
        double b = plinq.Sum();
        return Math.Abs(a - b) <= 1e-9 * Math.Max(1.0, Math.Abs(a));
    }

    // ---------- Форматування ----------

    public static string FormatEnvironment()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"- .NET: {RuntimeInformation.FrameworkDescription}");
        sb.AppendLine($"- ОС: {RuntimeInformation.OSDescription}");
        sb.AppendLine($"- Логічних процесорів: {Environment.ProcessorCount}");
#if DEBUG
        sb.AppendLine("- Конфігурація: Debug (УВАГА: для коректних вимірювань запускайте з -c Release)");
#else
        sb.AppendLine("- Конфігурація: Release");
#endif
        sb.AppendLine();
        return sb.ToString();
    }

    public static string FormatExperiments(IReadOnlyList<ExperimentRow> rows)
    {
        var inv = CultureInfo.InvariantCulture;
        var sb = new StringBuilder();

        sb.AppendLine("| Розмір | Операція | LINQ, мс | PLINQ, мс | Прискорення | Результат | Збіг результатів |");
        sb.AppendLine("|---:|---|---:|---:|---:|---|:---:|");

        foreach (ExperimentRow r in rows)
        {
            double speedup = r.PlinqMs > 0 ? r.LinqMs / r.PlinqMs : double.NaN;
            string verdict = speedup >= 1.0 ? "PLINQ швидший" : "LINQ швидший";

            sb.AppendLine($"| {r.Size.ToString("N0", inv)} | {r.Operation} | {r.LinqMs.ToString("F1", inv)} | {r.PlinqMs.ToString("F1", inv)} | {speedup.ToString("F2", inv)}x | {verdict} | {(r.ResultsMatch ? "так" : "НІ (!)")} |");
        }

        sb.AppendLine();
        return sb.ToString();
    }

    public static string FormatScaling(double linqMs, IReadOnlyList<ParallelismRow> rows)
    {
        var inv = CultureInfo.InvariantCulture;
        var sb = new StringBuilder();

        sb.AppendLine($"Базовий час звичайного LINQ: {linqMs.ToString("F1", inv)} мс");
        sb.AppendLine();
        sb.AppendLine("| Потоків (WithDegreeOfParallelism) | Час, мс | Прискорення відносно LINQ |");
        sb.AppendLine("|---:|---:|---:|");

        foreach (ParallelismRow r in rows)
        {
            double speedup = r.Milliseconds > 0 ? linqMs / r.Milliseconds : double.NaN;
            sb.AppendLine($"| {r.Degree} | {r.Milliseconds.ToString("F1", inv)} | {speedup.ToString("F2", inv)}x |");
        }

        sb.AppendLine();
        return sb.ToString();
    }
}
