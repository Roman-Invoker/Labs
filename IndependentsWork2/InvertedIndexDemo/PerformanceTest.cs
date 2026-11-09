using System.Diagnostics;

namespace InvertedIndexDemo;

public static class PerformanceTest
{
    /// <summary>
    /// Порівнює лінійний пошук і пошук через інвертований індекс.
    /// Для обох підходів використовується однакова нормалізація (TextProcessor.Tokenize).
    /// </summary>
    public static void CompareSearchMethods(List<Document> documents, string searchWord)
    {
        var sw = new Stopwatch();

        // 1. Лінійний пошук (без індексу)
        sw.Start();
        string normalizedWord = searchWord.ToLowerInvariant();
        var linearResults = documents
            .Where(d => TextProcessor.Tokenize(d.Content).Contains(normalizedWord))
            .ToList();
        sw.Stop();
        Console.WriteLine($"Linear search: {sw.ElapsedTicks} ticks");

        // 2. Inverted Index
        var index = new InvertedIndex();
        foreach (Document doc in documents)
        {
            index.AddDocument(doc);
        }

        sw.Restart();
        var indexResults = index.SearchSingleWord(searchWord);
        sw.Stop();
        Console.WriteLine($"Inverted Index: {sw.ElapsedTicks} ticks");

        // Перевірка коректності
        Console.WriteLine($"Linear found: {linearResults.Count}, Index found: {indexResults.Count}");
    }

    /// <summary>
    /// Точніше порівняння: кожен пошук повторюється <paramref name="iterations"/> разів,
    /// наприкінці виводиться середній час одного запиту (мкс) та час побудови індексу.
    /// </summary>
    public static void RunBenchmark(int documentCount, string searchWord, int iterations)
    {
        List<Document> documents = GenerateDocuments(documentCount, seed: 42);
        string normalizedWord = searchWord.ToLowerInvariant();

        // Прогрів JIT, щоб перше виконання не спотворювало результат
        _ = documents.Where(d => TextProcessor.Tokenize(d.Content).Contains(normalizedWord)).Count();

        var buildWatch = Stopwatch.StartNew();
        var index = new InvertedIndex();
        foreach (Document doc in documents)
        {
            index.AddDocument(doc);
        }

        buildWatch.Stop();
        _ = index.SearchSingleWord(searchWord);

        int linearFound = 0;
        var linearWatch = Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++)
        {
            linearFound = documents.Where(d => TextProcessor.Tokenize(d.Content).Contains(normalizedWord)).ToList().Count;
        }

        linearWatch.Stop();

        int indexFound = 0;
        long indexIterations = iterations * 1000L;
        var indexWatch = Stopwatch.StartNew();
        for (long i = 0; i < indexIterations; i++)
        {
            indexFound = index.SearchSingleWord(searchWord).Count;
        }

        indexWatch.Stop();

        double linearMicros = linearWatch.Elapsed.TotalMilliseconds * 1000.0 / iterations;
        double indexMicros = indexWatch.Elapsed.TotalMilliseconds * 1000.0 / indexIterations;
        string correct = linearFound == indexFound ? "збігаються" : "НЕ збігаються!";

        Console.WriteLine(
            $"{documentCount,6} док. | лінійний: {linearMicros,10:F2} мкс | індекс: {indexMicros,8:F2} мкс | " +
            $"прискорення: {linearMicros / indexMicros,8:F0}x | побудова індексу: {buildWatch.Elapsed.TotalMilliseconds,8:F2} мс | " +
            $"знайдено {linearFound}/{indexFound} ({correct})");
    }

    /// <summary>Генерує документи з фіксованим seed (результати відтворювані).</summary>
    public static List<Document> GenerateDocuments(int count, int seed)
    {
        string[] vocabulary = BuildVocabulary();
        var random = new Random(seed);
        var documents = new List<Document>(count);

        for (int id = 1; id <= count; id++)
        {
            var words = new string[30];
            for (int i = 0; i < words.Length; i++)
            {
                words[i] = vocabulary[random.Next(vocabulary.Length)];
            }

            documents.Add(new Document
            {
                Id = id,
                Title = $"Generated document {id}",
                Content = string.Join(' ', words) + "."
            });
        }

        return documents;
    }

    private static string[] BuildVocabulary()
    {
        var words = new List<string> { "programming", "language", "java", "python", "csharp", "linq", "async", "modern", "popular", "syntax" };

        for (int i = 0; i < 490; i++)
        {
            words.Add($"term{i}");
        }

        return words.ToArray();
    }
}
