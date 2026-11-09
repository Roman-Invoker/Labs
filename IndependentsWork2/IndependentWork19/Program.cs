using System.Text;

namespace IndependentWork19;

public static class Program
{
    public static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        DemoCustomException();
        DemoExceptionVersusResult();

        ResilienceScenarios.RunRetryScenario();
        ResilienceScenarios.RunCircuitBreakerScenario();
        ResilienceScenarios.RunTimeoutFallbackScenario();
    }

    // Власний тип винятку + try / catch / finally
    private static void DemoCustomException()
    {
        Console.WriteLine("=== Власний виняток InsufficientFundsException ===");

        var account = new BankAccount("Іван Петренко", 1000m);

        try
        {
            account.Withdraw(300m);
            Console.WriteLine($"Знято 300.00, баланс: {account.Balance:F2}");

            account.Withdraw(1500m);
            Console.WriteLine("Цей рядок не виконається.");
        }
        catch (InsufficientFundsException ex)
        {
            Console.WriteLine($"Помилка: {ex.Message}");
            Console.WriteLine($"  Запитано: {ex.RequestedAmount:F2}; баланс: {ex.CurrentBalance:F2}; не вистачає: {ex.Shortfall:F2}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Інша помилка: {ex.Message}");
        }
        finally
        {
            Console.WriteLine("Операцію завершено (блок finally).");
        }

        Console.WriteLine();
    }

    // Коли виняток, а коли Result
    private static void DemoExceptionVersusResult()
    {
        Console.WriteLine("=== Result<T> для очікуваних помилок вводу ===");

        foreach (string input in new[] { "250.50", "abc", "-10" })
        {
            Result<decimal> result = BankAccount.ParseAmount(input);
            Console.WriteLine(result.IsSuccess
                ? $"'{input}' -> OK: {result.Value:F2}"
                : $"'{input}' -> помилка валідації: {result.Error}");
        }

        Console.WriteLine();
    }
}
