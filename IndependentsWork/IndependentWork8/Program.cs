using System.Globalization;
using IndependentWork8;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

        DemoFileMetadataCache();
        Console.WriteLine();
        DemoTagManager();
        Console.WriteLine();
        DemoSearchIndex();
        Console.WriteLine();
        DemoCustomComparer();
    }

    // ---------- Сценарій 1: Dictionary ----------
    static void DemoFileMetadataCache()
    {
        Console.WriteLine("=== Сценарій 1: FileMetadataCache (Dictionary) ===");

        FileMetadataCache cache = new FileMetadataCache();
        cache.Add(new FileMetadata("report.docx", 24576, new DateTime(2026, 10, 1, 14, 30, 0), "a1b2c3"));
        cache.Add(new FileMetadata("photo.png", 1048576, new DateTime(2026, 9, 15, 9, 5, 0), "d4e5f6"));
        cache.Add(new FileMetadata("notes.txt", 512, new DateTime(2026, 10, 10, 18, 45, 0), "789abc"));
        Console.WriteLine($"Додано файлів: {cache.Count}");

        Console.WriteLine($"Get(\"report.docx\") -> {cache.Get("report.docx")}");
        Console.WriteLine($"Get(\"REPORT.DOCX\") -> {cache.Get("REPORT.DOCX")} (регістр ключа не важливий)");
        Console.WriteLine($"Contains(\"photo.png\"): {cache.Contains("photo.png")}");
        Console.WriteLine($"Contains(\"missing.txt\"): {cache.Contains("missing.txt")}");
        Console.WriteLine($"Get(\"missing.txt\") -> {(cache.Get("missing.txt") == null ? "null" : "знайдено")}");
        Console.WriteLine($"Remove(\"photo.png\"): {cache.Remove("photo.png")}");
        Console.WriteLine($"Remove(\"photo.png\") ще раз: {cache.Remove("photo.png")}");
        Console.WriteLine($"Лишилось файлів: {cache.Count}");
    }

    // ---------- Сценарій 2: Dictionary + HashSet (теги) ----------
    static void DemoTagManager()
    {
        Console.WriteLine("=== Сценарій 2: TagManager (Dictionary + HashSet) ===");

        TagManager manager = new TagManager();
        manager.AddTag("article-1", "csharp");
        manager.AddTag("article-1", "oop");
        manager.AddTag("article-1", "collections");
        manager.AddTag("article-2", "csharp");
        manager.AddTag("article-2", "linq");
        manager.AddTag("article-2", "collections");
        manager.AddTag("article-3", "python");

        Console.WriteLine($"Теги article-1: {Format(manager.GetTags("article-1"))}");

        manager.AddTag("article-1", "CSharp");
        Console.WriteLine($"Після повторного додавання \"CSharp\" (дублікат ігнорується): {Format(manager.GetTags("article-1"))}");

        Console.WriteLine($"Усі унікальні теги: {Format(manager.GetAllUniqueTags())}");
        Console.WriteLine($"Спільні теги article-1 і article-2: {Format(manager.FindCommonTags("article-1", "article-2"))}");
        Console.WriteLine($"Теги лише в article-1 (відносно article-2): {Format(manager.FindTagsOnlyIn("article-1", "article-2"))}");
        Console.WriteLine($"Теги лише в article-2 (відносно article-1): {Format(manager.FindTagsOnlyIn("article-2", "article-1"))}");

        manager.RemoveTag("article-1", "oop");
        Console.WriteLine($"Після RemoveTag(article-1, oop): {Format(manager.GetTags("article-1"))}");
        Console.WriteLine($"Спільні теги article-1 і article-3: {Format(manager.FindCommonTags("article-1", "article-3"))}");
    }

    // ---------- Сценарій 3: Dictionary + HashSet (пошуковий індекс) ----------
    static void DemoSearchIndex()
    {
        Console.WriteLine("=== Сценарій 3: SearchIndex (Dictionary + HashSet) ===");

        SearchIndex index = new SearchIndex();
        index.AddDocument(1, "Кішка сидить на вікні");
        index.AddDocument(2, "Собака і кішка живуть разом");
        index.AddDocument(3, "Собака біжить по парку");
        index.AddDocument(4, "Кішка біжить по даху");
        Console.WriteLine($"Документів проіндексовано: 4, унікальних слів: {index.WordCount}");

        Console.WriteLine($"Search(\"кішка\"): {Format(index.Search("кішка"))}");
        Console.WriteLine($"Search(\"КІШКА\") (регістр не важливий): {Format(index.Search("КІШКА"))}");
        Console.WriteLine($"Search(\"собака\"): {Format(index.Search("собака"))}");
        Console.WriteLine($"Search(\"риба\"): {Format(index.Search("риба"))}");
        Console.WriteLine($"SearchMultiple(\"кішка\", \"біжить\"): {Format(index.SearchMultiple("кішка", "біжить"))}");
        Console.WriteLine($"SearchMultiple(\"кішка\", \"собака\"): {Format(index.SearchMultiple("кішка", "собака"))}");
        Console.WriteLine($"SearchMultiple(\"собака\", \"біжить\", \"по\"): {Format(index.SearchMultiple("собака", "біжить", "по"))}");
        Console.WriteLine($"SearchMultiple(\"кішка\", \"риба\"): {Format(index.SearchMultiple("кішка", "риба"))}");
    }

    // ---------- Додатково: HashSet з custom comparer ----------
    static void DemoCustomComparer()
    {
        Console.WriteLine("=== Додатково: HashSet з custom comparer ===");

        HashSet<Person> people = new HashSet<Person>(new PersonNameComparer());
        Console.WriteLine($"Додавання Person(\"John\", 25): {people.Add(new Person("John", 25))}");
        Console.WriteLine($"Додавання Person(\"John\", 30): {people.Add(new Person("John", 30))}");

        Person stored = people.Single();
        Console.WriteLine($"Елементів у HashSet: {people.Count} (Name = {stored.Name}, Age = {stored.Age})");
    }

    // Для стабільного виводу елементи множин сортуються
    static string Format(IEnumerable<string> items)
    {
        string[] sorted = items.OrderBy(item => item, StringComparer.Ordinal).ToArray();
        return sorted.Length == 0 ? "(немає)" : string.Join(", ", sorted);
    }

    static string Format(IEnumerable<int> items)
    {
        int[] sorted = items.OrderBy(item => item).ToArray();
        return sorted.Length == 0 ? "(немає)" : string.Join(", ", sorted);
    }
}
