namespace IndependentWork19;

/// <summary>Виняток: на рахунку недостатньо коштів для зняття запитаної суми.</summary>
public class InsufficientFundsException : Exception
{
    /// <summary>Сума, яку намагались зняти.</summary>
    public decimal RequestedAmount { get; }

    /// <summary>Баланс рахунку на момент операції.</summary>
    public decimal CurrentBalance { get; }

    public InsufficientFundsException()
    {
    }

    public InsufficientFundsException(string message) : base(message)
    {
    }

    public InsufficientFundsException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public InsufficientFundsException(string message, decimal requestedAmount, decimal currentBalance)
        : base(message)
    {
        RequestedAmount = requestedAmount;
        CurrentBalance = currentBalance;
    }

    /// <summary>Скільки коштів не вистачає.</summary>
    public decimal Shortfall => RequestedAmount - CurrentBalance;
}
