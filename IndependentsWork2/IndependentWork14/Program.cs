using System.Text;

namespace IndependentWork14;

public static class Program
{
    public static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        DemoDelayRange();
        DemoSuccessAfterFailures();
        DemoFinalFailure();
        DemoShouldRetry();
    }

    // 0. Діапазон затримок за формулою (без випадкової частини)
    private static void DemoDelayRange()
    {
        Console.WriteLine("=== Діапазон затримок: initialDelay = 100 мс, jitterFactor = 0.2 ===");

        var policy = new RetryPolicy(maxRetries: 5, initialDelay: TimeSpan.FromMilliseconds(100), jitterFactor: 0.2);

        for (int retry = 0; retry < 5; retry++)
        {
            var (min, max) = policy.GetDelayRange(retry);
            Console.WriteLine($"  повтор #{retry + 1}: {min.TotalMilliseconds:F0}–{max.TotalMilliseconds:F0} мс");
        }

        Console.WriteLine();
    }

    // 1. Операція спочатку падає, потім успішна
    private static void DemoSuccessAfterFailures()
    {
        Console.WriteLine("=== Сценарій 1: успіх після кількох невдач ===");

        var policy = new RetryPolicy(maxRetries: 4, initialDelay: TimeSpan.FromMilliseconds(100), jitterFactor: 0.2);
        int calls = 0;

        string result = policy.Execute(() =>
        {
            calls++;
            if (calls <= 2)
            {
                throw new TimeoutException($"Сервер не відповів (виклик №{calls})");
            }

            return "Дані отримано";
        });

        Console.WriteLine($"Результат: {result}; операцію викликано {calls} разів.");
        Console.WriteLine();
    }

    // 2. Операція завжди падає — виняток після всіх спроб
    private static void DemoFinalFailure()
    {
        Console.WriteLine("=== Сценарій 2: остаточна невдача ===");

        var policy = new RetryPolicy(maxRetries: 3, initialDelay: TimeSpan.FromMilliseconds(50), jitterFactor: 0.2);
        int calls = 0;

        try
        {
            policy.Execute<int>(() =>
            {
                calls++;
                throw new HttpRequestException("Сервіс недоступний");
            });
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Остаточний виняток: {ex.GetType().Name}: {ex.Message}; викликів операції: {calls}.");
        }

        Console.WriteLine();
    }

    // 3. shouldRetry: повторюємо лише тимчасові збої
    private static void DemoShouldRetry()
    {
        Console.WriteLine("=== Сценарій 3: shouldRetry (повторюємо лише TimeoutException) ===");

        var policy = new RetryPolicy(maxRetries: 3, initialDelay: TimeSpan.FromMilliseconds(50), jitterFactor: 0.2);
        int calls = 0;

        try
        {
            policy.Execute<string>(
                () =>
                {
                    calls++;
                    throw new ArgumentException("Некоректні вхідні дані — повтор безглуздий");
                },
                shouldRetry: ex => ex is TimeoutException);
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Виняток без повторів: {ex.Message}; викликів операції: {calls}.");
        }
    }
}
