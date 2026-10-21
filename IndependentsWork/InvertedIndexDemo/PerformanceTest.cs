using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace InvertedIndexDemo;

/// <summary>
/// Один рядок результатів вимірювання.
/// </summary>
public record BenchmarkRow(
    string Word,
    int Documents,
    double LinearMicroseconds,
    double IndexMicroseconds,
    double BuildMilliseconds,
    int LinearFound,
    int IndexFound);

/// <summary>
/// Порівняння лінійного пошуку та інвертованого індексу.
/// </summary>
public static class PerformanceTest
{
    public static readonly int[] DocumentCounts = { 10, 100, 1000, 10000 };

    // Результат присвоюється сюди, щоб JIT не міг викинути виклик як "мертвий код"
    private static object? _sink;

    /// <summary>
    /// Лінійний пошук без індексу. Нормалізація запиту й документів така сама, як в індексі
    /// (TextProcessor.Tokenize), інакше порівняння було б некоректним.
    /// </summary>
    public static List<Document> LinearSearch(IEnumerable<Document> documents, string searchWord)
    {
        List<string> terms = TextProcessor.Tokenize(searchWord).Distinct().ToList();
        if (terms.Count == 0)
        {
            return new List<Document>();
        }

        return documents
            .Where(d => terms.All(term => TextProcessor.Tokenize(d.Content).Contains(term)))
            .ToList();
    }

    /// <summary>
    /// Одноразове порівняння (як у завданні): час у тіках для одного виклику.
    /// Один виклик включає JIT-компіляцію і шум, тому для висновків використовуйте RunBenchmarks.
    /// </summary>
    public static void CompareSearchMethods(List<Document> documents, string searchWord)
    {
        var sw = new Stopwatch();

        // 1. Лінійний пошук
        sw.Start();
        var linearResults = LinearSearch(documents, searchWord);
        sw.Stop();
        Console.WriteLine($"Linear search: {sw.ElapsedTicks} ticks");

        // 2. Inverted Index (побудова індексу не входить у час пошуку)
        var index = new InvertedIndex();
        foreach (var doc in documents)
        {
            index.AddDocument(doc);
        }

        sw.Restart();
        var indexResults = index.SearchSingleWord(searchWord);
        sw.Stop();
        Console.WriteLine($"Inverted Index: {sw.ElapsedTicks} ticks");

        // Перевірка коректності
        Console.WriteLine($"Linear found: {linearResults.Count}, Index found: {indexResults.Count}");
        Console.WriteLine($"Results match: {linearResults.Select(d => d.Id).SequenceEqual(indexResults.Select(d => d.Id))}");
    }

    /// <summary>
    /// Усереднені вимірювання для 10 / 100 / 1000 / 10000 документів.
    /// </summary>
    public static List<BenchmarkRow> RunBenchmarks(IEnumerable<string> words)
    {
        var rows = new List<BenchmarkRow>();

        foreach (int count in DocumentCounts)
        {
            List<Document> documents = DocumentGenerator.Generate(count);

            var index = new InvertedIndex();
            foreach (Document doc in documents)
            {
                index.AddDocument(doc);
            }

            // Вартість побудови індексу (одноразова)
            double buildMs = MeasureMicroseconds(() =>
            {
                var temp = new InvertedIndex();
                foreach (Document doc in documents)
                {
                    temp.AddDocument(doc);
                }
                _sink = temp;
            }) / 1000.0;

            foreach (string word in words)
            {
                int linearFound = LinearSearch(documents, word).Count;
                int indexFound = index.SearchSingleWord(word).Count;

                double linearUs = MeasureMicroseconds(() => _sink = LinearSearch(documents, word));
                double indexUs = MeasureMicroseconds(() => _sink = index.SearchSingleWord(word));

                rows.Add(new BenchmarkRow(word, count, linearUs, indexUs, buildMs, linearFound, indexFound));
            }
        }

        return rows;
    }

    /// <summary>
    /// Середній час одного виклику в мікросекундах. Кількість повторів збільшується вдвічі,
    /// доки пакет не триватиме щонайменше 100 мс (ранні пакети слугують розігрівом JIT).
    /// </summary>
    private static double MeasureMicroseconds(Action action)
    {
        long iterations = 1;

        while (true)
        {
            var sw = Stopwatch.StartNew();
            for (long i = 0; i < iterations; i++)
            {
                action();
            }
            sw.Stop();

            if (sw.ElapsedMilliseconds >= 100 || iterations >= (1L << 24))
            {
                return sw.Elapsed.TotalMilliseconds * 1000.0 / iterations;
            }

            iterations *= 2;
        }
    }

    /// <summary>
    /// Форматує результати у вигляді Markdown-таблиць для REPORT.md.
    /// </summary>
    public static string FormatMarkdown(IReadOnlyList<BenchmarkRow> rows)
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
        sb.AppendLine();

        foreach (var group in rows.GroupBy(r => r.Word))
        {
            sb.AppendLine($"### Запит: «{group.Key}»");
            sb.AppendLine();
            sb.AppendLine("| Документів | Лінійний пошук, мкс | Індекс (пошук), мкс | Прискорення | Побудова індексу, мс | Поріг окупності, запитів | Знайдено (лін. / інд.) |");
            sb.AppendLine("|---:|---:|---:|---:|---:|---:|---:|");

            foreach (BenchmarkRow r in group)
            {
                string speedup = r.IndexMicroseconds > 0
                    ? (r.LinearMicroseconds / r.IndexMicroseconds).ToString("F1", inv) + "x"
                    : "-";

                double saved = r.LinearMicroseconds - r.IndexMicroseconds;
                string breakEven = saved > 0
                    ? Math.Ceiling(r.BuildMilliseconds * 1000.0 / saved).ToString("F0", inv)
                    : "-";

                string match = r.LinearFound == r.IndexFound
                    ? $"{r.LinearFound} / {r.IndexFound}"
                    : $"{r.LinearFound} / {r.IndexFound} (!)";

                sb.AppendLine($"| {r.Documents} | {r.LinearMicroseconds.ToString("F1", inv)} | {r.IndexMicroseconds.ToString("F2", inv)} | {speedup} | {r.BuildMilliseconds.ToString("F2", inv)} | {breakEven} | {match} |");
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }
}
