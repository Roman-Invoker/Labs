namespace IndependentWork5;

/// <summary>
/// Клас БЕЗ перевизначення ToString(): потрібен для порівняння з успадкованою реалізацією System.Object.
/// </summary>
public class Warehouse
{
    public string Name { get; }

    public Warehouse(string name)
    {
        Name = name;
    }
}
