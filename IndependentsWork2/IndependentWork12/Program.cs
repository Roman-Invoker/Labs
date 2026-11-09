using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;

namespace IndependentWork12;

public static class Program
{
    private const int MaxValue = 200_000;

    public static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine($"Логічних процесорів: {Environment.ProcessorCount}");
        Console.WriteLine();

        WarmUp();

        Console.WriteLine("=== Частина 1. Продуктивність: LINQ vs PLINQ ===");
        Console.WriteLine("Операція: Where(IsPrime).Select(Math.Sqrt).ToList() над випадковими числами з [2; 200000]");
        Console.WriteLine();

        foreach (int size in new[] { 1_000_000, 5_000_000, 10_000_000 })
        {
            ComparePerformance(size);
        }

        Console.WriteLine("=== Частина 2. Побічні ефекти в PLINQ ===");
        DemonstrateSideEffects();
    }

    // ---------- Обчислювально інтенсивна операція ----------

    /// <summary>Перевірка на просте число методом пробного ділення (O(√n)).</summary>
    private static bool IsPrime(int n)
    {
        if (n < 2) return false;
        if (n % 2 == 0) return n == 2;

        for (int i = 3; (long)i * i <= n; i += 2)
        {
            if (n % i == 0) return false;
        }

        return true;
    }

    private static List<int> GenerateData(int size)
    {
        var random = new Random(42);
        var data = new List<int>(size);

        for (int i = 0; i < size; i++)
        {
            data.Add(random.Next(2, MaxValue + 1));
        }

        return data;
    }

    // ---------- Частина 1: вимірювання продуктивності ----------

    private static void WarmUp()
    {
        List<int> data = GenerateData(50_000);
        _ = data.Where(IsPrime).Select(n => Math.Sqrt(n)).ToList();
        _ = data.AsParallel().Where(IsPrime).Select(n => Math.Sqrt(n)).ToList();
    }

    private static void ComparePerformance(int size)
    {
        List<int> data = GenerateData(size);
        Console.WriteLine($"--- Розмір колекції: {size:N0} ---");

        var sw = Stopwatch.StartNew();
        List<double> linq = data.Where(IsPrime).Select(n => Math.Sqrt(n)).ToList();
        sw.Stop();
        double linqMs = sw.Elapsed.TotalMilliseconds;
        Console.WriteLine($"LINQ:                  {linqMs,10:F1} мс, знайдено простих: {linq.Count:N0}");

        sw.Restart();
        List<double> plinq = data.AsParallel().Where(IsPrime).Select(n => Math.Sqrt(n)).ToList();
        sw.Stop();
        double plinqMs = sw.Elapsed.TotalMilliseconds;
        Console.WriteLine($"PLINQ:                 {plinqMs,10:F1} мс, знайдено простих: {plinq.Count:N0}  (прискорення: {linqMs / plinqMs:F2}x)");

        sw.Restart();
        List<double> plinqOrdered = data.AsParallel().AsOrdered().Where(IsPrime).Select(n => Math.Sqrt(n)).ToList();
        sw.Stop();
        double orderedMs = sw.Elapsed.TotalMilliseconds;
        Console.WriteLine($"PLINQ (AsOrdered):     {orderedMs,10:F1} мс, знайдено простих: {plinqOrdered.Count:N0}  (прискорення: {linqMs / orderedMs:F2}x)");

        bool countsMatch = linq.Count == plinq.Count && linq.Count == plinqOrdered.Count;
        bool orderMatches = linq.SequenceEqual(plinqOrdered);
        Console.WriteLine($"Кількість результатів збігається: {countsMatch}; AsOrdered зберіг порядок LINQ: {orderMatches}");
        Console.WriteLine();
    }

    // ---------- Частина 2: побічні ефекти ----------

    private static void DemonstrateSideEffects()
    {
        const int size = 5_000_000;
        const int attempts = 5;
        int degree = Math.Max(4, Environment.ProcessorCount);

        List<int> data = GenerateData(size);

        // Еталон — послідовне обчислення без побічних ефектів
        int expected = data.Count(n => n % 7 == 0);
        Console.WriteLine($"Колекція: {size:N0} чисел; умова: n % 7 == 0; ступінь паралелізму: {degree}");
        Console.WriteLine($"Еталонний (правильний) результат: {expected:N0}");
        Console.WriteLine();

        // 1. НЕБЕЗПЕЧНО: спільна змінна без синхронізації
        Console.WriteLine("1) Спільний лічильник counter++ без синхронізації (data race):");
        for (int attempt = 1; attempt <= attempts; attempt++)
        {
            int counter = 0;
            data.AsParallel().WithDegreeOfParallelism(degree).ForAll(n =>
            {
                if (n % 7 == 0)
                {
                    counter++; // читання-зміна-запис — не атомарна операція
                }
            });

            Console.WriteLine($"   спроба {attempt}: {counter:N0} (очікувалось {expected:N0}, втрачено {expected - counter:N0})");
        }

        Console.WriteLine();

        // 1б. Те саме, але з явно розширеним «вікном» гонки між читанням і записом.
        //     На багатоядерній машині помилка помітна і без цього; на 1 ядрі так гонка стає відтворюваною.
        Console.WriteLine("1б) Той самий лічильник, але між читанням і записом додано Thread.Yield() (розширене вікно гонки):");
        for (int attempt = 1; attempt <= 3; attempt++)
        {
            int counter = 0;
            data.Take(500_000).AsParallel().WithDegreeOfParallelism(degree).ForAll(n =>
            {
                if (n % 7 == 0)
                {
                    int current = counter;
                    Thread.Yield();
                    counter = current + 1;
                }
            });

            int expectedPart = data.Take(500_000).Count(n => n % 7 == 0);
            Console.WriteLine($"   спроба {attempt}: {counter:N0} (очікувалось {expectedPart:N0}, втрачено {expectedPart - counter:N0})");
        }

        Console.WriteLine();

        // 2. НЕБЕЗПЕЧНО: List<T>.Add з кількох потоків
        Console.WriteLine("2) Спільний List<int>.Add без синхронізації (List<T> не потокобезпечний):");
        for (int attempt = 1; attempt <= attempts; attempt++)
        {
            var list = new List<int>();
            try
            {
                data.AsParallel().WithDegreeOfParallelism(degree).Where(n => n % 7 == 0).ForAll(n => list.Add(n));
                Console.WriteLine($"   спроба {attempt}: у списку {list.Count:N0} елементів (очікувалось {expected:N0})");
            }
            catch (AggregateException ex)
            {
                Console.WriteLine($"   спроба {attempt}: виняток {ex.InnerExceptions[0].GetType().Name}: {ex.InnerExceptions[0].Message}");
            }
        }

        Console.WriteLine();

        // 3. ВИПРАВЛЕННЯ
        Console.WriteLine("3) Виправлення:");

        int lockCounter = 0;
        object sync = new();
        var sw = Stopwatch.StartNew();
        data.AsParallel().WithDegreeOfParallelism(degree).ForAll(n =>
        {
            if (n % 7 == 0)
            {
                lock (sync)
                {
                    lockCounter++;
                }
            }
        });
        sw.Stop();
        Console.WriteLine($"   lock:                    {lockCounter:N0} ({Verdict(lockCounter, expected)}), {sw.Elapsed.TotalMilliseconds:F0} мс");

        int interlockedCounter = 0;
        sw.Restart();
        data.AsParallel().WithDegreeOfParallelism(degree).ForAll(n =>
        {
            if (n % 7 == 0)
            {
                Interlocked.Increment(ref interlockedCounter);
            }
        });
        sw.Stop();
        Console.WriteLine($"   Interlocked.Increment:   {interlockedCounter:N0} ({Verdict(interlockedCounter, expected)}), {sw.Elapsed.TotalMilliseconds:F0} мс");

        var bag = new ConcurrentBag<int>();
        sw.Restart();
        data.AsParallel().WithDegreeOfParallelism(degree).Where(n => n % 7 == 0).ForAll(n => bag.Add(n));
        sw.Stop();
        Console.WriteLine($"   ConcurrentBag<int>.Add:  {bag.Count:N0} ({Verdict(bag.Count, expected)}), {sw.Elapsed.TotalMilliseconds:F0} мс");

        sw.Restart();
        int pure = data.AsParallel().WithDegreeOfParallelism(degree).Count(n => n % 7 == 0);
        sw.Stop();
        Console.WriteLine($"   без побічних ефектів (PLINQ Count): {pure:N0} ({Verdict(pure, expected)}), {sw.Elapsed.TotalMilliseconds:F0} мс");
    }

    private static string Verdict(int actual, int expected) => actual == expected ? "правильно" : "НЕПРАВИЛЬНО";
}
