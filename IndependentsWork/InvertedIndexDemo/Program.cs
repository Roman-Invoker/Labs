using System.Globalization;
using InvertedIndexDemo;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

       
        if (args.Length > 0 && args[0].Equals("benchmark", StringComparison.OrdinalIgnoreCase))
        {
            RunBenchmarks();
            return;
        }

        RunInteractive();
    }

    // ---------- Частина 4: інтерактивна демонстрація ----------
    static void RunInteractive()
    {
        var index = new InvertedIndex();
        LoadDocuments(index);

        Console.WriteLine($"Loaded {index.DocumentCount} documents");
        Console.WriteLine($"Index contains {index.UniqueWords} unique words");
        Console.WriteLine("Tip: start a query with \"any:\" to find documents with ANY of the words (OR)");
        Console.WriteLine();

        while (true)
        {
            Console.Write("Enter search query (or 'exit'): ");
            string? query = Console.ReadLine();

            if (query == null || query.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            if (string.IsNullOrWhiteSpace(query))
            {
                continue;
            }

            bool anyMode = query.StartsWith("any:", StringComparison.OrdinalIgnoreCase);
            if (anyMode)
            {
                query = query.Substring("any:".Length);
            }

            string[] words = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            List<Document> results;
            if (anyMode)
            {
                Console.WriteLine("\n--- Search Results (OR) ---");
                results = index.SearchAnyWord(words);
            }
            else
            {
                Console.WriteLine("\n--- Search Results (AND) ---");
                results = index.SearchMultipleWords(words);
            }

            foreach (Document doc in results)
            {
                Console.WriteLine(doc);
                Console.WriteLine($"  {Snippet(doc.Content)}");
            }

            Console.WriteLine($"\nFound {results.Count} documents\n");
        }
    }

    static void LoadDocuments(InvertedIndex index)
    {
        foreach (Document doc in DocumentGenerator.GetSampleDocuments())
        {
            index.AddDocument(doc);
        }
    }

    // Перші 100 символів вмісту + "..."
    static string Snippet(string content)
    {
        string head = content.Substring(0, Math.Min(100, content.Length)).TrimEnd('.');
        return head + "...";
    }

    // ---------- Частина 3: вимірювання продуктивності ----------
    static void RunBenchmarks()
    {
#if DEBUG
        Console.WriteLine("УВАГА: це Debug-збірка, результати будуть спотворені. Запустіть: dotnet run -c Release -- benchmark");
        Console.WriteLine();
#endif
        Console.WriteLine("=== Одноразові вимірювання (ticks) ===");
        foreach (int count in PerformanceTest.DocumentCounts)
        {
            Console.WriteLine($"--- {count} документів, слово \"async\" ---");
            PerformanceTest.CompareSearchMethods(DocumentGenerator.Generate(count), "async");
        }

        Console.WriteLine();
        Console.WriteLine("=== Усереднені вимірювання (кілька секунд) ===");
        var rows = PerformanceTest.RunBenchmarks(new[] { "async", "programming" });
        string markdown = PerformanceTest.FormatMarkdown(rows);

        Console.WriteLine(markdown);
        File.WriteAllText("benchmark-results.md", markdown);
        Console.WriteLine("Результати збережено у benchmark-results.md");
    }
}
