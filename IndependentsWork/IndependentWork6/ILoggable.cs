namespace IndependentWork6;

/// <summary>
/// Додаткова поведінка: об'єкт вміє виводити власний лог.
/// Клас може реалізувати цей інтерфейс разом з INotifier (множинна поведінка).
/// </summary>
public interface ILoggable
{
    void WriteLog();
}
