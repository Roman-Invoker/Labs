namespace IndependentWork14;

/// <summary>
/// Політика повторних спроб (Retry) з експоненційною затримкою (exponential backoff) та випадковим jitter.
/// </summary>
public class RetryPolicy
{
    private readonly int _maxRetries;
    private readonly TimeSpan _initialDelay;
    private readonly double _jitterFactor;
    private readonly Random _random = Random.Shared;

    /// <param name="maxRetries">Максимальна кількість ПОВТОРНИХ спроб (загалом операція виконається до maxRetries + 1 разів).</param>
    /// <param name="initialDelay">Початкова затримка для експоненційного backoff.</param>
    /// <param name="jitterFactor">Коефіцієнт jitter: 0.2 означає ±20% від розрахованої затримки.</param>
    public RetryPolicy(int maxRetries, TimeSpan initialDelay, double jitterFactor = 0.2)
    {
        if (maxRetries < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxRetries), maxRetries, "Кількість повторів не може бути від'ємною.");
        }

        if (initialDelay < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(initialDelay), initialDelay, "Початкова затримка не може бути від'ємною.");
        }

        if (jitterFactor < 0 || jitterFactor > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(jitterFactor), jitterFactor, "Jitter має бути в діапазоні [0; 1].");
        }

        _maxRetries = maxRetries;
        _initialDelay = initialDelay;
        _jitterFactor = jitterFactor;
    }

    /// <summary>
    /// Виконує операцію з повторними спробами. Якщо спроби вичерпано (або shouldRetry повернув false),
    /// викидається останній виняток.
    /// </summary>
    /// <param name="operation">Операція, яку потрібно виконати.</param>
    /// <param name="shouldRetry">Необов'язковий предикат: чи слід повторювати спробу для цього винятку (за замовчуванням — завжди).</param>
    public T Execute<T>(Func<T> operation, Func<Exception, bool>? shouldRetry = null)
    {
        ArgumentNullException.ThrowIfNull(operation);
        shouldRetry ??= _ => true;

        int retryAttempt = 0; // скільки повторів уже було (від 0)

        while (true)
        {
            int attemptNumber = retryAttempt + 1;

            try
            {
                Console.WriteLine($"  [спроба {attemptNumber}/{_maxRetries + 1}] виконання операції...");
                T result = operation();
                Console.WriteLine($"  [спроба {attemptNumber}/{_maxRetries + 1}] успіх");
                return result;
            }
            catch (Exception ex) when (retryAttempt < _maxRetries && shouldRetry(ex))
            {
                TimeSpan delay = CalculateDelay(retryAttempt);
                Console.WriteLine(
                    $"  [спроба {attemptNumber}/{_maxRetries + 1}] невдача: {ex.GetType().Name}: {ex.Message}. " +
                    $"Очікування {delay.TotalMilliseconds:F0} мс перед повтором...");

                Thread.Sleep(delay);
                retryAttempt++;
            }
            catch (Exception ex)
            {
                string reason = retryAttempt >= _maxRetries
                    ? "вичерпано всі спроби"
                    : "виняток не підлягає повторній спробі (shouldRetry повернув false)";

                Console.WriteLine(
                    $"  [спроба {attemptNumber}/{_maxRetries + 1}] невдача: {ex.GetType().Name}: {ex.Message}. Зупинка — {reason}.");
                throw;
            }
        }
    }

    /// <summary>
    /// delay = initialDelay * 2^retryAttempt * (1 + random * jitterFactor * 2 - jitterFactor).
    /// </summary>
    public TimeSpan CalculateDelay(int retryAttempt)
    {
        double jitterMultiplier = 1 + _random.NextDouble() * _jitterFactor * 2 - _jitterFactor;
        double milliseconds = _initialDelay.TotalMilliseconds * Math.Pow(2, retryAttempt) * jitterMultiplier;
        return TimeSpan.FromMilliseconds(milliseconds);
    }

    /// <summary>Мінімально та максимально можлива затримка для заданого повтору (для демонстрації діапазону jitter).</summary>
    public (TimeSpan Min, TimeSpan Max) GetDelayRange(int retryAttempt)
    {
        double baseMs = _initialDelay.TotalMilliseconds * Math.Pow(2, retryAttempt);
        return (TimeSpan.FromMilliseconds(baseMs * (1 - _jitterFactor)), TimeSpan.FromMilliseconds(baseMs * (1 + _jitterFactor)));
    }
}
