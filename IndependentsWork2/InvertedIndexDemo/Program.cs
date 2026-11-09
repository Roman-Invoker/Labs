using System.Text;

namespace InvertedIndexDemo;

public static class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        var index = new InvertedIndex();
        LoadDocuments(index);

        Console.WriteLine($"Loaded {index.DocumentCount} documents");
        Console.WriteLine($"Index contains {index.UniqueWords} unique words");
        Console.WriteLine();

        if (args.Length > 0 && args[0] == "--interactive")
        {
            RunInteractive(index);
            return;
        }

        // Демонстраційні запити (без введення з клавіатури)
        RunQuery(index, "programming language");
        RunQuery(index, "c# async");
        RunQuery(index, "java python", any: true);
        RunQuery(index, "kotlin");

        Console.WriteLine("=== Performance: лінійний пошук vs інвертований індекс (слово \"programming\") ===");
        PerformanceTest.CompareSearchMethods(PerformanceTest.GenerateDocuments(10, 42), "programming");
        Console.WriteLine();

        Console.WriteLine("Середній час одного запиту (повторні вимірювання):");
        PerformanceTest.RunBenchmark(10, "programming", iterations: 2000);
        PerformanceTest.RunBenchmark(100, "programming", iterations: 500);
        PerformanceTest.RunBenchmark(1000, "programming", iterations: 50);
        PerformanceTest.RunBenchmark(10000, "programming", iterations: 5);

        Console.WriteLine();
        Console.WriteLine("Для інтерактивного режиму: dotnet run -- --interactive");
    }

    private static void RunQuery(InvertedIndex index, string query, bool any = false)
    {
        string[] words = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var results = any ? index.SearchAnyWord(words) : index.SearchMultipleWords(words);

        Console.WriteLine($"Query: \"{query}\"");
        Console.WriteLine($"--- Search Results ({(any ? "OR" : "AND")}) ---");
        PrintResults(results);
    }

    private static void RunInteractive(InvertedIndex index)
    {
        while (true)
        {
            Console.Write("Enter search query (or 'exit'): ");
            string? query = Console.ReadLine();

            if (query is null || query == "exit")
            {
                break;
            }

            string[] words = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            Console.WriteLine("\n--- Search Results (AND) ---");
            PrintResults(index.SearchMultipleWords(words));
        }
    }

    private static void PrintResults(List<Document> results)
    {
        foreach (Document doc in results)
        {
            Console.WriteLine(doc);
            Console.WriteLine($"  {doc.Content.Substring(0, Math.Min(100, doc.Content.Length))}...");
        }

        Console.WriteLine($"\nFound {results.Count} documents\n");
    }

    private static void LoadDocuments(InvertedIndex index)
    {
        var documents = new List<Document>
        {
            new() { Id = 1, Title = "Introduction to C#", Content = "C# is a modern object-oriented programming language developed by Microsoft." },
            new() { Id = 2, Title = "Java Basics", Content = "Java is a popular programming language known for its portability and performance." },
            new() { Id = 3, Title = "Python Overview", Content = "Python is a high-level programming language with simple syntax." },
            new() { Id = 4, Title = "C# Advanced Features", Content = "C# supports LINQ, async/await, and many modern programming paradigms." },
            new() { Id = 5, Title = "Comparing Languages", Content = "C#, Java, and Python are among the most popular programming languages today." }
        };

        foreach (Document doc in documents)
        {
            index.AddDocument(doc);
        }
    }
}
