namespace lab19v14.Common;

/// <summary>
/// Патерн Result: або успішне значення, або повідомлення про помилку —
/// без викидання винятків для очікуваних сценаріїв невдачі.
/// </summary>
public class Result<T>
{
    public bool IsSuccess { get; }
    public T Value { get; }
    public string Error { get; }

    private Result(T value, bool isSuccess, string error)
    {
        Value = value;
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result<T> Success(T value) => new(value, true, string.Empty);

    public static Result<T> Failure(string error) => new(default!, false, error);

    public static Result<T> Failure(Exception ex)
    {
        ArgumentNullException.ThrowIfNull(ex);
        return new Result<T>(default!, false, ex.Message);
    }

    public override string ToString() =>
        IsSuccess ? $"Success({Value})" : $"Failure({Error})";
}
