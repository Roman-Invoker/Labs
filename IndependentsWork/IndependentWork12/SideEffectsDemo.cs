using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;

namespace IndependentWork12;

/// <summary>
/// Побічні ефекти й потокобезпечність у PLINQ (завдання 5).
/// Метод повертає текст усього виводу, щоб його можна було зберегти у benchmark-results.md.
/// </summary>
public static class SideEffectsDemo
{
    public static string Run(int size)
    {
        var transcript = new StringBuilder();
        void Log(string line)
        {
            Console.WriteLine(line);
            transcript.AppendLine(line);
        }

        List<int> data = PerformanceExperiments.GenerateData(size);
        int expected = data.Count(x => x % 2 == 0); // еталон: послідовний підрахунок

        Log($"Колекція: {size:N0} чисел. Скільки з них парних (еталон): {expected:N0}");
        Log("");

        // ---------- Проблема 1: спільний лічильник без синхронізації ----------
        Log("--- Проблема 1: спільний лічильник count++ без синхронізації ---");
        for (int run = 1; run <= 5; run++)
        {
            int count = 0;
            data.AsParallel().Where(x => x % 2 == 0).ForAll(_ => count++); // гонка даних!
            Log($"Запуск {run}: отримано {count:N0}, очікувалось {expected:N0}, втрачено {expected - count:N0}");
        }
        Log("");

        // ---------- Проблема 2: спільна колекція List<int> ----------
        Log("--- Проблема 2: List<int>.Add з кількох потоків ---");
        for (int run = 1; run <= 5; run++)
        {
            var list = new List<int>();
            try
            {
                data.AsParallel().Where(x => x % 2 == 0).ForAll(x => list.Add(x)); // List<T> не потокобезпечний!
                Log($"Запуск {run}: у списку {list.Count:N0} елементів, очікувалось {expected:N0}, втрачено {expected - list.Count:N0}");
            }
            catch (AggregateException ex)
            {
                Log($"Запуск {run}: виняток {ex.Flatten().InnerExceptions[0].GetType().Name}");
            }
        }
        Log("");

        // ---------- Виправлення лічильника ----------
        Log("--- Виправлення 1: лічильник ---");

        var sw = Stopwatch.StartNew();
        int sequential = data.Count(x => x % 2 == 0);
        sw.Stop();
        Log($"Послідовний LINQ Count():           {sequential:N0}  ({sw.Elapsed.TotalMilliseconds:F1} мс) {Check(sequential, expected)}");

        int lockCount = 0;
        var sync = new object();
        sw.Restart();
        data.AsParallel().Where(x => x % 2 == 0).ForAll(_ => { lock (sync) { lockCount++; } });
        sw.Stop();
        Log($"PLINQ + lock:                       {lockCount:N0}  ({sw.Elapsed.TotalMilliseconds:F1} мс) {Check(lockCount, expected)}");

        int interlockedCount = 0;
        sw.Restart();
        data.AsParallel().Where(x => x % 2 == 0).ForAll(_ => Interlocked.Increment(ref interlockedCount));
        sw.Stop();
        Log($"PLINQ + Interlocked.Increment:      {interlockedCount:N0}  ({sw.Elapsed.TotalMilliseconds:F1} мс) {Check(interlockedCount, expected)}");

        sw.Restart();
        int aggregated = data.AsParallel().Count(x => x % 2 == 0);
        sw.Stop();
        Log($"PLINQ Count() (без спільного стану): {aggregated:N0}  ({sw.Elapsed.TotalMilliseconds:F1} мс) {Check(aggregated, expected)}");
        Log("");

        // ---------- Виправлення колекції ----------
        Log("--- Виправлення 2: збирання елементів у колекцію ---");

        var lockedList = new List<int>();
        sw.Restart();
        data.AsParallel().Where(x => x % 2 == 0).ForAll(x => { lock (sync) { lockedList.Add(x); } });
        sw.Stop();
        Log($"PLINQ + lock + List<int>:           {lockedList.Count:N0}  ({sw.Elapsed.TotalMilliseconds:F1} мс) {Check(lockedList.Count, expected)}");

        var bag = new ConcurrentBag<int>();
        sw.Restart();
        data.AsParallel().Where(x => x % 2 == 0).ForAll(x => bag.Add(x));
        sw.Stop();
        Log($"PLINQ + ConcurrentBag<int>:         {bag.Count:N0}  ({sw.Elapsed.TotalMilliseconds:F1} мс) {Check(bag.Count, expected)}");

        sw.Restart();
        List<int> collected = data.AsParallel().Where(x => x % 2 == 0).ToList();
        sw.Stop();
        Log($"PLINQ ToList() (без спільного стану): {collected.Count:N0}  ({sw.Elapsed.TotalMilliseconds:F1} мс) {Check(collected.Count, expected)}");

        return transcript.ToString();
    }

    private static string Check(int actual, int expected)
    {
        return actual == expected ? "[коректно]" : "[НЕВІРНО]";
    }
}
