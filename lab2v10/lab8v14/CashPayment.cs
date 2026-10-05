namespace Lab8v14;

/// <summary>Платіж готівкою.</summary>
public class CashPayment : Payment
{
    /// <summary>Код валюти (наприклад, UAH).</summary>
    public string Currency { get; }

    /// <summary>Створює готівковий платіж.</summary>
    /// <param name="amount">Сума платежу.</param>
    /// <param name="currency">Код валюти.</param>
    /// <exception cref="ArgumentException">Якщо валюта порожня.</exception>
    public CashPayment(decimal amount, string currency) : base(amount)
    {
        if (string.IsNullOrWhiteSpace(currency))
        {
            throw new ArgumentException("Валюта не може бути порожньою.", nameof(currency));
        }

        Currency = currency.Trim().ToUpperInvariant();
    }

    /// <inheritdoc />
    public override void Process()
    {
        Console.WriteLine($"[Готівка] Прийнято {Amount:F2} {Currency} готівкою. Видано касовий чек.");
    }
}
