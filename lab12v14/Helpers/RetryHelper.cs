namespace lab18v14.Helpers;

/// <summary>
/// Узагальнений допоміжний клас для патерну Retry з експоненційною затримкою.
/// </summary>
public static class RetryHelper
{
    /// <summary>Затримка за замовчуванням, якщо initialDelay не передано (default).</summary>
    private static readonly TimeSpan DefaultInitialDelay = TimeSpan.FromMilliseconds(100);

    /// <summary>
    /// Виконує операцію з повторними спробами.
    /// </summary>
    /// <param name="operation">Операція, яку потрібно виконати.</param>
    /// <param name="retryCount">Максимальна кількість ПОВТОРНИХ спроб (усього спроб = retryCount + 1).</param>
    /// <param name="initialDelay">Початкова затримка; перед повтором №n (n = 0, 1, 2, ...) чекаємо initialDelay * 2^n.</param>
    /// <param name="shouldRetry">Визначає, чи варто повторювати спробу для конкретного винятку (null = повторювати для будь-якого).</param>
    public static T ExecuteWithRetry<T>(
        Func<T> operation,
        int retryCount = 3,
        TimeSpan initialDelay = default,
        Func<Exception, bool>? shouldRetry = null)
    {
        ArgumentNullException.ThrowIfNull(operation);
        if (retryCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(retryCount), "Кількість повторів не може бути від'ємною.");
        }

        if (initialDelay <= TimeSpan.Zero)
        {
            initialDelay = DefaultInitialDelay;
        }

        int maxAttempts = retryCount + 1;

        for (int attempt = 0; ; attempt++)
        {
            try
            {
                Log($"Спроба {attempt + 1}/{maxAttempts}...");
                T result = operation();
                Log($"Спроба {attempt + 1}/{maxAttempts}: УСПІХ.");
                return result;
            }
            catch (Exception ex)
            {
                Log($"Спроба {attempt + 1}/{maxAttempts}: невдача — {ex.GetType().Name}: {ex.Message}");

                bool retryAllowed = shouldRetry?.Invoke(ex) ?? true;
                if (!retryAllowed)
                {
                    Log("shouldRetry повернув false — повтори не виконуються, виняток передається далі.");
                    throw;
                }

                if (attempt >= retryCount)
                {
                    Log($"Вичерпано всі повторні спроби ({retryCount}) — виняток передається далі.");
                    throw;
                }

                // Експоненційна затримка: initialDelay * 2^attempt
                TimeSpan delay = TimeSpan.FromTicks(initialDelay.Ticks * (1L << attempt));
                Log($"Повтор №{attempt + 1} через {delay.TotalMilliseconds:F0} мс " +
                    $"({initialDelay.TotalMilliseconds:F0} * 2^{attempt}).");
                Thread.Sleep(delay);
            }
        }
    }

    private static void Log(string message) => Console.WriteLine($"  [Retry] {message}");
}
