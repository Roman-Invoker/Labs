namespace Lab6v14;

/// <summary>Похідний клас: радіоприймач.</summary>
public class Radio : Electronic
{
    private string _frequencyRange;

    public Radio(string brand, double powerConsumption, string frequencyRange)
        : base(brand, powerConsumption)
    {
        _frequencyRange = string.Empty;
        FrequencyRange = frequencyRange;
    }

    public string FrequencyRange
    {
        get => _frequencyRange;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Діапазон частот не може бути порожнім.", nameof(value));
            _frequencyRange = value;
        }
    }

    public override void TurnOn()
    {
        base.TurnOn();
        Console.WriteLine($"[Radio] Діапазон {FrequencyRange} налаштовано.");
    }

    /// <summary>Власний унікальний метод.</summary>
    public void TuneStation(double frequency)
    {
        Console.WriteLine($"[Radio] {Brand}: налаштування на станцію {frequency} МГц.");
    }
}
