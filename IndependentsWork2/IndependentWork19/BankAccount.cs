namespace IndependentWork19;

/// <summary>Банківський рахунок — предметна область для демонстрації власного винятку.</summary>
public class BankAccount
{
    public BankAccount(string owner, decimal balance)
    {
        Owner = owner;
        Balance = balance;
    }

    public string Owner { get; }
    public decimal Balance { get; private set; }

    /// <summary>Знімає кошти. Якщо коштів недостатньо — викидає <see cref="InsufficientFundsException"/>.</summary>
    public void Withdraw(decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), amount, "Сума має бути додатною.");
        }

        if (amount > Balance)
        {
            throw new InsufficientFundsException(
                $"Недостатньо коштів на рахунку {Owner}: запитано {amount:F2}, доступно {Balance:F2}.",
                amount,
                Balance);
        }

        Balance -= amount;
    }

    /// <summary>Валідація введеної суми — очікувана невдача, тому повертаємо Result, а не кидаємо виняток.</summary>
    public static Result<decimal> ParseAmount(string input)
    {
        if (!decimal.TryParse(input, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out decimal amount))
        {
            return Result<decimal>.Fail($"'{input}' не є числом.");
        }

        return amount > 0
            ? Result<decimal>.Ok(amount)
            : Result<decimal>.Fail("Сума має бути додатною.");
    }
}
