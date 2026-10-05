namespace Lab8v14;

/// <summary>Платіж банківською карткою.</summary>
public class CreditCardPayment : Payment
{
    private const int CardNumberLength = 16;
    private const int VisibleDigits = 4;

    /// <summary>Номер картки (16 цифр, без пробілів).</summary>
    public string CardNumber { get; }

    /// <summary>Створює платіж карткою.</summary>
    /// <param name="amount">Сума платежу.</param>
    /// <param name="cardNumber">Номер картки; пробіли ігноруються.</param>
    /// <exception cref="ArgumentException">Якщо номер порожній або не містить 16 цифр.</exception>
    public CreditCardPayment(decimal amount, string cardNumber) : base(amount)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
        {
            throw new ArgumentException("Номер картки не може бути порожнім.", nameof(cardNumber));
        }

        string digits = new string(cardNumber.Where(char.IsDigit).ToArray());

        if (digits.Length != CardNumberLength)
        {
            throw new ArgumentException($"Номер картки має містити {CardNumberLength} цифр.", nameof(cardNumber));
        }

        CardNumber = digits;
    }

    /// <summary>Повертає номер картки з прихованими цифрами, окрім останніх чотирьох.</summary>
    public string GetMaskedNumber()
    {
        return $"**** **** **** {CardNumber[^VisibleDigits..]}";
    }

    /// <inheritdoc />
    public override void Process()
    {
        Console.WriteLine($"[Картка] Списання {Amount:F2} з картки {GetMaskedNumber()}. Платіж підтверджено банком.");
    }
}
