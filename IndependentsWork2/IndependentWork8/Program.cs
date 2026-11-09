using System.Text;

namespace IndependentWork8;

public static class Program
{
    public static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        DemoFileMetadataCache();
        DemoTagManager();
        DemoSearchIndex();
    }

    // Сценарій 1: Dictionary
    private static void DemoFileMetadataCache()
    {
        Console.WriteLine("=== Сценарій 1: FileMetadataCache (Dictionary) ===");

        var cache = new FileMetadataCache();

        cache.Add(new FileMetadata { FileName = "report.docx", Size = 48_213, LastModified = new DateTime(2026, 10, 12, 9, 30, 0), Hash = "a1b2c3" });
        cache.Add(new FileMetadata { FileName = "photo.png", Size = 1_204_551, LastModified = new DateTime(2026, 9, 28, 17, 5, 0), Hash = "d4e5f6" });
        Console.WriteLine($"Додано файлів: {cache.Count}");

        Console.WriteLine($"Contains(\"REPORT.DOCX\") = {cache.Contains("REPORT.DOCX")} (регістр ігнорується)");
        Console.WriteLine($"Get(\"photo.png\") -> {cache.Get("photo.png")}");

        try
        {
            cache.Add(new FileMetadata { FileName = "Report.docx" });
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Повторне Add -> {ex.GetType().Name}: {ex.Message}");
        }

        Console.WriteLine($"Remove(\"photo.png\") = {cache.Remove("photo.png")}; Remove(\"photo.png\") ще раз = {cache.Remove("photo.png")}");
        Console.WriteLine($"TryGet(\"photo.png\") = {cache.TryGet("photo.png", out _)}; Count = {cache.Count}");
        Console.WriteLine();
    }

    // Сценарій 2: Dictionary + HashSet
    private static void DemoTagManager()
    {
        Console.WriteLine("=== Сценарій 2: TagManager (HashSet) ===");

        var tags = new TagManager();

        tags.AddTag("article-1", "csharp");
        tags.AddTag("article-1", "dotnet");
        tags.AddTag("article-1", "oop");
        tags.AddTag("article-1", "CSharp"); // дублікат без урахування регістру — не додасться

        tags.AddTag("article-2", "CSharp");
        tags.AddTag("article-2", "dotnet");
        tags.AddTag("article-2", "linq");

        tags.AddTag("article-3", "java");
        tags.AddTag("article-3", "oop");

        Console.WriteLine($"Теги article-1: {Format(tags.GetTags("article-1"))}");
        Console.WriteLine($"Усі унікальні теги (UnionWith): {Format(tags.GetAllUniqueTags())}");
        Console.WriteLine($"Спільні теги article-1 та article-2 (IntersectWith): {Format(tags.FindCommonTags("article-1", "article-2"))}");
        Console.WriteLine($"Теги лише в article-1 порівняно з article-2 (ExceptWith): {Format(tags.FindTagsOnlyInFirst("article-1", "article-2"))}");
        Console.WriteLine($"Спільні теги article-1 та article-3: {Format(tags.FindCommonTags("article-1", "article-3"))}");

        tags.RemoveTag("article-3", "java");
        tags.RemoveTag("article-3", "oop");
        Console.WriteLine($"Після видалення всіх тегів article-3, усі унікальні теги: {Format(tags.GetAllUniqueTags())}");
        Console.WriteLine();
    }

    // Сценарій 3 (додатковий): Dictionary<string, HashSet<int>>
    private static void DemoSearchIndex()
    {
        Console.WriteLine("=== Сценарій 3 (додатково): SearchIndex ===");

        var index = new SearchIndex();
        index.AddDocument(1, "C# is a modern object-oriented programming language.");
        index.AddDocument(2, "Java is a popular programming language.");
        index.AddDocument(3, "Python is a high-level programming language.");
        index.AddDocument(4, "C# supports LINQ and async programming.");

        Console.WriteLine($"Унікальних слів в індексі: {index.UniqueWords}");
        Console.WriteLine($"Search(\"programming\") = {Format(index.Search("programming"))}");
        Console.WriteLine($"Search(\"linq\") = {Format(index.Search("linq"))}");
        Console.WriteLine($"SearchMultiple(\"c\", \"programming\") = {Format(index.SearchMultiple("c", "programming"))}");
        Console.WriteLine($"SearchMultiple(\"java\", \"linq\") = {Format(index.SearchMultiple("java", "linq"))}");
    }

    private static string Format<T>(IEnumerable<T> items) where T : notnull =>
        "{ " + string.Join(", ", items.OrderBy(x => x)) + " }";
}
