namespace InvertedIndexDemo;

/// <summary>
/// Тестові дані: п'ять документів із завдання та генерація довільної кількості документів.
/// </summary>
public static class DocumentGenerator
{
    private static readonly string[] Vocabulary =
    {
        "system", "data", "network", "cloud", "memory", "compiler", "thread", "server", "client", "module",
        "object", "class", "method", "interface", "library", "framework", "testing", "debugging", "deployment", "security",
        "database", "query", "cache", "index", "algorithm", "structure", "pattern", "design", "software", "engine"
    };

    public static List<Document> GetSampleDocuments()
    {
        return new List<Document>
        {
            new Document
            {
                Id = 1,
                Title = "Introduction to C#",
                Content = "C# is a modern object-oriented programming language developed by Microsoft."
            },
            new Document
            {
                Id = 2,
                Title = "Java Basics",
                Content = "Java is a popular programming language known for its portability and performance."
            },
            new Document
            {
                Id = 3,
                Title = "Python Overview",
                Content = "Python is a high-level programming language with simple syntax."
            },
            new Document
            {
                Id = 4,
                Title = "C# Advanced Features",
                Content = "C# supports LINQ, async/await, and many modern programming paradigms."
            },
            new Document
            {
                Id = 5,
                Title = "Comparing Languages",
                Content = "C#, Java, and Python are among the most popular programming languages today."
            }
        };
    }

    /// <summary>
    /// Повертає count документів. Перші п'ять (якщо count >= 5) - оригінальні; решта створюється
    /// комбінуванням п'яти базових текстів із випадковими словами. Seed фіксований, тому набір
    /// документів однаковий під час кожного запуску.
    /// </summary>
    public static List<Document> Generate(int count, int seed = 42)
    {
        List<Document> samples = GetSampleDocuments();
        var documents = new List<Document>(count);
        var random = new Random(seed);

        for (int id = 1; id <= count; id++)
        {
            if (id <= samples.Count)
            {
                documents.Add(samples[id - 1]);
                continue;
            }

            Document template = samples[random.Next(samples.Count)];

            var extraWords = new List<string>();
            for (int i = 0; i < 6; i++)
            {
                // Половина слів - зі словника, інші - з великої кількості унікальних "keywordN"
                extraWords.Add(random.Next(2) == 0
                    ? Vocabulary[random.Next(Vocabulary.Length)]
                    : $"keyword{random.Next(500)}");
            }

            documents.Add(new Document
            {
                Id = id,
                Title = $"Generated document {id}",
                Content = template.Content + " " + string.Join(" ", extraWords)
            });
        }

        return documents;
    }
}
