namespace Lab8v14;

/// <summary>
/// Базовий клас платежу. Визначає віртуальний метод <see cref="Process"/>,
/// який похідні класи перевизначають під свій спосіб оплати.
/// </summary>
public class Payment
{
    /// <summary>Сума платежу.</summary>
    public decimal Amount { get; }

    /// <summary>Створює платіж із заданою сумою.</summary>
    /// <param name="amount">Сума платежу, має бути більшою за нуль.</param>
    /// <exception cref="ArgumentOutOfRangeException">Якщо сума не додатна.</exception>
    public Payment(decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Сума платежу має бути більшою за нуль.");
        }

        Amount = amount;
    }

    /// <summary>Обробляє платіж. Похідні класи надають власну реалізацію.</summary>
    public virtual void Process()
    {
        Console.WriteLine($"[Платіж] Оброблено платіж на суму {Amount:F2}.");
    }
}
