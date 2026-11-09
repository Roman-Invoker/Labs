using Polly;
using Polly.CircuitBreaker;
using Polly.Fallback;
using Polly.Retry;
using Polly.Timeout;

namespace IndependentWork19;

/// <summary>Три сценарії відмовостійкості на основі Polly (ResiliencePipelineBuilder).</summary>
public static class ResilienceScenarios
{
    private static void Log(string message) => Console.WriteLine($"  [{DateTime.Now:HH:mm:ss.fff}] {message}");

    // ------------------------------------------------------------------
    // Сценарій 1. Зовнішній API тимчасово недоступний -> Retry + exponential backoff + jitter
    // ------------------------------------------------------------------
    public static void RunRetryScenario()
    {
        Console.WriteLine("=== Сценарій 1: Retry (зовнішній API погоди тимчасово недоступний) ===");

        var pipeline = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                ShouldHandle = new PredicateBuilder().Handle<HttpRequestException>(),
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromMilliseconds(200),
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
                OnRetry = args =>
                {
                    Log($"Повтор #{args.AttemptNumber + 1} через {args.RetryDelay.TotalMilliseconds:F0} мс. Причина: {args.Outcome.Exception?.Message}");
                    return default;
                }
            })
            .Build();

        int calls = 0;

        string forecast = pipeline.Execute(() =>
        {
            calls++;
            Log($"Виклик API (спроба {calls})...");

            if (calls <= 2)
            {
                throw new HttpRequestException($"503 Service Unavailable (виклик №{calls})");
            }

            return "Рівне: +7°C, хмарно";
        });

        Log($"Успіх: {forecast}");
        Console.WriteLine();
    }

    // ------------------------------------------------------------------
    // Сценарій 2. База даних «лежить» -> Circuit Breaker (Closed -> Open -> Half-Open -> Closed)
    // ------------------------------------------------------------------
    public static void RunCircuitBreakerScenario()
    {
        Console.WriteLine("=== Сценарій 2: Circuit Breaker (база даних втратила з'єднання) ===");

        var pipeline = new ResiliencePipelineBuilder()
            .AddCircuitBreaker(new CircuitBreakerStrategyOptions
            {
                ShouldHandle = new PredicateBuilder().Handle<InvalidOperationException>(),
                FailureRatio = 0.5,
                MinimumThroughput = 3,
                SamplingDuration = TimeSpan.FromSeconds(10),
                BreakDuration = TimeSpan.FromSeconds(1),
                OnOpened = args =>
                {
                    Log($"*** Автомат РОЗІМКНУТО (Open) на {args.BreakDuration.TotalSeconds:F0} с. Остання помилка: {args.Outcome.Exception?.Message}");
                    return default;
                },
                OnHalfOpened = _ =>
                {
                    Log("*** Автомат НАПІВРОЗІМКНУТО (Half-Open): пробний запит.");
                    return default;
                },
                OnClosed = _ =>
                {
                    Log("*** Автомат ЗАМКНУТО (Closed): сервіс відновився.");
                    return default;
                }
            })
            .Build();

        bool databaseIsUp = false;
        int dbCalls = 0;

        void Query(int requestNumber)
        {
            try
            {
                pipeline.Execute(() =>
                {
                    dbCalls++;
                    Log($"Запит #{requestNumber}: звернення до БД (реальних звернень: {dbCalls})");

                    if (!databaseIsUp)
                    {
                        throw new InvalidOperationException("З'єднання з БД розірвано");
                    }
                });

                Log($"Запит #{requestNumber}: успіх");
            }
            catch (BrokenCircuitException)
            {
                Log($"Запит #{requestNumber}: ВІДХИЛЕНО миттєво (автомат розімкнений), БД не навантажується");
            }
            catch (InvalidOperationException ex)
            {
                Log($"Запит #{requestNumber}: помилка — {ex.Message}");
            }
        }

        for (int i = 1; i <= 6; i++)
        {
            Query(i);
        }

        Log("... чекаємо 1.2 с (BreakDuration = 1 с); тим часом БД відновлюється ...");
        databaseIsUp = true;
        Thread.Sleep(1200);

        Query(7);
        Query(8);
        Console.WriteLine();
    }

    // ------------------------------------------------------------------
    // Сценарій 3. Брокер повідомлень перевантажений -> Fallback + Retry + Timeout
    // ------------------------------------------------------------------
    public static void RunTimeoutFallbackScenario()
    {
        Console.WriteLine("=== Сценарій 3: Timeout + Retry + Fallback (черга повідомлень перевантажена) ===");

        var pipeline = new ResiliencePipelineBuilder<string>()
            // Зовнішня стратегія: якщо все інше не допомогло — запасний варіант
            .AddFallback(new FallbackStrategyOptions<string>
            {
                ShouldHandle = new PredicateBuilder<string>().Handle<TimeoutRejectedException>(),
                FallbackAction = _ => Outcome.FromResultAsValueTask("повідомлення збережено в локальну папку outbox"),
                OnFallback = _ =>
                {
                    Log("Fallback: брокер недоступний, використовуємо запасний варіант.");
                    return default;
                }
            })
            // Середня стратегія: повторити після тайм-ауту
            .AddRetry(new RetryStrategyOptions<string>
            {
                ShouldHandle = new PredicateBuilder<string>().Handle<TimeoutRejectedException>(),
                MaxRetryAttempts = 2,
                Delay = TimeSpan.FromMilliseconds(100),
                BackoffType = DelayBackoffType.Exponential,
                OnRetry = args =>
                {
                    Log($"Retry #{args.AttemptNumber + 1} після тайм-ауту (затримка {args.RetryDelay.TotalMilliseconds:F0} мс)");
                    return default;
                }
            })
            // Внутрішня стратегія: обмеження часу однієї спроби
            .AddTimeout(new TimeoutStrategyOptions
            {
                Timeout = TimeSpan.FromMilliseconds(300),
                OnTimeout = args =>
                {
                    Log($"Timeout: спроба перевищила {args.Timeout.TotalMilliseconds:F0} мс і була скасована");
                    return default;
                }
            })
            .Build();

        // 3а. Брокер «оговтується»: перші 2 спроби повільні, третя — швидка
        Console.WriteLine(" 3а) Брокер оговтується після двох повільних відповідей:");
        int calls = 0;
        string result = pipeline.Execute(ct => PublishMessage(ref calls, slowAttempts: 2, ct));
        Log($"Результат: {result}");

        // 3б. Брокер недоступний: усі спроби повільні -> спрацьовує Fallback
        Console.WriteLine(" 3б) Брокер недоступний (усі спроби повільні):");
        calls = 0;
        result = pipeline.Execute(ct => PublishMessage(ref calls, slowAttempts: int.MaxValue, ct));
        Log($"Результат: {result}");
        Console.WriteLine();
    }

    private static string PublishMessage(ref int calls, int slowAttempts, CancellationToken cancellationToken)
    {
        calls++;
        Log($"Публікація повідомлення (спроба {calls})...");

        if (calls <= slowAttempts)
        {
            // Імітація повільної відповіді брокера (до 2 с); виходимо одразу після скасування токена
            cancellationToken.WaitHandle.WaitOne(TimeSpan.FromSeconds(2));
            cancellationToken.ThrowIfCancellationRequested();
        }

        return "повідомлення опубліковано в чергу";
    }
}
