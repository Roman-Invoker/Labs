namespace Lab8v14;

/// <summary>Онлайн-платіж через платіжний шлюз.</summary>
public class OnlinePayment : Payment
{
    /// <summary>Назва платіжного шлюзу.</summary>
    public string Gateway { get; }

    /// <summary>Створює онлайн-платіж.</summary>
    /// <param name="amount">Сума платежу.</param>
    /// <param name="gateway">Назва платіжного шлюзу.</param>
    /// <exception cref="ArgumentException">Якщо назва шлюзу порожня.</exception>
    public OnlinePayment(decimal amount, string gateway) : base(amount)
    {
        if (string.IsNullOrWhiteSpace(gateway))
        {
            throw new ArgumentException("Назва шлюзу не може бути порожньою.", nameof(gateway));
        }

        Gateway = gateway.Trim();
    }

    /// <inheritdoc />
    public override void Process()
    {
        Console.WriteLine($"[Онлайн] Платіж {Amount:F2} передано до шлюзу {Gateway}. Очікуємо підтвердження.");
    }
}
