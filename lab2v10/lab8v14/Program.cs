using System.Globalization;
using System.Text;

namespace Lab8v14;


public static class Program
{
    public static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

        
        List<Payment> payments = new()
        {
            new CreditCardPayment(1250.50m, "4111 1111 1111 1234"),
            new CashPayment(300m, "UAH"),
            new OnlinePayment(780.25m, "LiqPay"),
            new CreditCardPayment(99.99m, "5500 0000 0000 0004"),
            new CashPayment(150m, "UAH"),
            new OnlinePayment(420m, "Stripe")
        };

        Console.WriteLine("=== Лабораторна робота №5: поліморфізм (варіант 14, платежі) ===");
        Console.WriteLine();

        Console.WriteLine("--- Обробка платежів (динамічне зв'язування) ---");
        ProcessAll(payments);
        Console.WriteLine();

        PrintAggregation(payments);
    }

    
    private static void ProcessAll(IReadOnlyList<Payment> payments)
    {
        for (int i = 0; i < payments.Count; i++)
        {
            Payment payment = payments[i];
            Console.WriteLine($"#{i + 1} {payment.GetType().Name}:");
            payment.Process();
        }
    }

        private static decimal CalculateTotal(IEnumerable<Payment> payments)
    {
        decimal total = 0m;

        foreach (Payment payment in payments)
        {
            total += payment.Amount;
        }

        return total;
    }

    private static void PrintAggregation(IReadOnlyCollection<Payment> payments)
    {
        Console.WriteLine("--- Агрегація результатів ---");
        Console.WriteLine($"Кількість платежів: {payments.Count}");
        Console.WriteLine($"Загальна сума всіх платежів: {CalculateTotal(payments):F2}");
        Console.WriteLine($"Найбільший платіж: {payments.Max(p => p.Amount):F2}");
        Console.WriteLine($"Найменший платіж: {payments.Min(p => p.Amount):F2}");
    }
}
