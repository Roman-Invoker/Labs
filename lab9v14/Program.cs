using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Lab15v14;

public static class Program
{
    public static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

        Console.WriteLine("=== ЛР №9, варіант 14: система резервного копіювання ===");

        DemoHashSet();
        DemoQueue();
        DemoStack();
        DemoOrder();
        DemoService();
        DemoPerformance();
    }

    private static void DemoHashSet()
    {
        Console.WriteLine("\n--- 1. HashSet<T>: унікальні файли для бекапу ---");
        var set = new BackupFileSet();

        var candidates = new[]
        {
            new BackupFile("docs/report.docx", 120),
            new BackupFile("docs/budget.xlsx", 340),
            new BackupFile("photos/trip.jpg", 2048),
            new BackupFile("docs/report.docx", 120),
            new BackupFile("DOCS/REPORT.DOCX", 120),
            new BackupFile("src/app.cs", 15),
        };

        foreach (var file in candidates)
        {
            bool added = set.Add(file);
            Console.WriteLine($"Add({file.Path}): {(added ? "додано" : "дублікат, пропущено")}");
        }

        Console.WriteLine($"Унікальних файлів: {set.Count} із {candidates.Length} спроб додавання");
        Show("Вміст набору (порядок не гарантується)", set.Files);

        Console.WriteLine($"Contains(photos/trip.jpg): {set.Contains(new BackupFile("photos/trip.jpg", 0))}");
        Console.WriteLine($"Contains(music/song.mp3): {set.Contains(new BackupFile("music/song.mp3", 0))}");
        Console.WriteLine($"Remove(src/app.cs): {set.Remove(new BackupFile("src/app.cs", 0))}");
        Console.WriteLine($"Remove(src/app.cs) повторно: {set.Remove(new BackupFile("src/app.cs", 0))}");

        var other = new[]
        {
            new BackupFile("docs/budget.xlsx", 340),
            new BackupFile("music/song.mp3", 4096),
        };

        var union = new BackupFileSet(set.Files);
        union.UnionWith(other);
        Show("UnionWith (об'єднання)", union.Files);

        var intersect = new BackupFileSet(set.Files);
        intersect.IntersectWith(other);
        Show("IntersectWith (перетин)", intersect.Files);

        var except = new BackupFileSet(set.Files);
        except.ExceptWith(other);
        Show("ExceptWith (різниця)", except.Files);
    }

    private static void DemoQueue()
    {
        Console.WriteLine("\n--- 2. Queue<T>: черга файлів на копіювання (FIFO) ---");
        var queue = new CopyQueue();

        foreach (var file in SampleFiles())
        {
            queue.Enqueue(file);
            Console.WriteLine($"Enqueue: {file.Path}");
        }

        Console.WriteLine($"У черзі: {queue.Count}");
        Console.WriteLine($"Peek (наступний на копіювання): {queue.Peek()}");

        while (queue.Count > 0)
        {
            var file = queue.CopyNext()!;
            Console.WriteLine($"Dequeue → копіюється {file.Path} (залишилось у черзі: {queue.Count})");
        }

        Console.WriteLine($"Порожня черга: Peek = {Describe(queue.Peek())}, CopyNext = {Describe(queue.CopyNext())}");
    }

    private static void DemoStack()
    {
        Console.WriteLine("\n--- 3. Stack<T>: історія бекапів з Undo (LIFO) ---");
        var history = new BackupHistory();
        var files = SampleFiles();

        history.Push(new BackupRecord(1, new[] { files[0], files[1] }));
        history.Push(new BackupRecord(2, new[] { files[2] }));
        history.Push(new BackupRecord(3, new[] { files[3] }));

        foreach (var record in history.Records.Reverse())
        {
            Console.WriteLine($"Push: {record}");
        }

        Console.WriteLine($"Peek (останній бекап): {history.Peek()}");
        Show("Історія (від новішого до старішого)", history.Records);

        Console.WriteLine($"Undo → скасовано: {history.Undo()}");
        Console.WriteLine($"Undo → скасовано: {history.Undo()}");
        Console.WriteLine($"Залишилось у історії: {history.Count}, Peek = {history.Peek()}");
        history.Undo();
        Console.WriteLine($"Порожня історія: Peek = {Describe(history.Peek())}, Undo = {Describe(history.Undo())}");
    }

    private static void DemoOrder()
    {
        Console.WriteLine("\n--- 4. Порядок обробки: Queue (FIFO) проти Stack (LIFO) ---");
        var files = SampleFiles();
        var queue = new Queue<BackupFile>(files);
        var stack = new Stack<BackupFile>(files);

        var queueOrder = new List<string>();
        while (queue.TryDequeue(out var queued))
        {
            queueOrder.Add(queued.Path);
        }

        var stackOrder = new List<string>();
        while (stack.TryPop(out var popped))
        {
            stackOrder.Add(popped.Path);
        }

        Console.WriteLine("Додавання: " + string.Join(" → ", files.Select(f => f.Path)));
        Console.WriteLine("Queue віддає: " + string.Join(" → ", queueOrder));
        Console.WriteLine("Stack віддає: " + string.Join(" → ", stackOrder));
    }

    private static void DemoService()
    {
        Console.WriteLine("\n--- 5. Інтегрований сценарій: HashSet + Queue + Stack ---");
        var service = new BackupService();

        var incoming = new[]
        {
            new BackupFile("docs/report.docx", 120),
            new BackupFile("docs/budget.xlsx", 340),
            new BackupFile("docs/report.docx", 120),
            new BackupFile("photos/trip.jpg", 2048),
            new BackupFile("src/app.cs", 15),
        };

        foreach (var file in incoming)
        {
            Console.WriteLine($"Schedule({file.Path}): {(service.Schedule(file) ? "у черзі" : "уже заплановано")}");
        }

        PrintState("Після планування", service);

        Console.WriteLine($"RunBackup(2): {service.RunBackup(2)}");
        Console.WriteLine($"RunBackup(2): {service.RunBackup(2)}");
        PrintState("Після двох бекапів", service);

        Console.WriteLine($"UndoLastBackup: скасовано {service.UndoLastBackup()}");
        PrintState("Після скасування", service);
    }

    private static void DemoPerformance()
    {
        Console.WriteLine("\n--- 6. Продуктивність: List<T>.Contains проти HashSet<T>.Contains ---");
        const int n = 10_000;

        var list = new List<BackupFile>(n);
        var set = new HashSet<BackupFile>(n);
        for (int i = 0; i < n; i++)
        {
            var file = new BackupFile($"data/file{i}.bin", i % 500);
            list.Add(file);
            set.Add(file);
        }

        var probes = Enumerable.Range(0, n).Select(i => new BackupFile($"data/file{i}.bin", 0)).ToArray();

        double listMs = Measure(() => probes.Count(list.Contains));
        double setMs = Measure(() => probes.Count(set.Contains));

        Console.WriteLine($"Пошук {n} файлів у колекції з {n} елементів:");
        Console.WriteLine($"  List<T>.Contains    (O(n)): {listMs:F2} мс");
        Console.WriteLine($"  HashSet<T>.Contains (O(1)): {setMs:F2} мс");
        Console.WriteLine(setMs > 0
            ? $"  HashSet швидший приблизно у {listMs / setMs:F0} разів"
            : "  HashSet швидший: час вимірювання HashSet менший за роздільність таймера");
    }

    private static BackupFile[] SampleFiles() => new[]
    {
        new BackupFile("docs/report.docx", 120),
        new BackupFile("docs/budget.xlsx", 340),
        new BackupFile("photos/trip.jpg", 2048),
        new BackupFile("src/app.cs", 15),
    };

    private static void PrintState(string title, BackupService service) =>
        Console.WriteLine($"{title}: унікальних = {service.UniqueCount}, у черзі = {service.PendingCount}, бекапів в історії = {service.BackupsCount}");

    private static double Measure(Func<int> action)
    {
        var stopwatch = Stopwatch.StartNew();
        int found = action();
        stopwatch.Stop();

        if (found <= 0)
        {
            throw new InvalidOperationException("Пошук має знаходити елементи.");
        }

        return stopwatch.Elapsed.TotalMilliseconds;
    }

    private static string Describe<T>(T? value) where T : class => value is null ? "null" : value.ToString()!;

    private static void Show<T>(string label, IEnumerable<T> items) =>
        Console.WriteLine($"{label}: [{string.Join(", ", items)}]");
}
