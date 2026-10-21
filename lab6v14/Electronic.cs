namespace Lab6v14;

/// <summary>Базовий клас: електронний пристрій.</summary>
public class Electronic
{
    private string _brand;
    private double _powerConsumption;

    public Electronic(string brand, double powerConsumption)
    {
        _brand = string.Empty;
        Brand = brand;
        PowerConsumption = powerConsumption;
    }

    public string Brand
    {
        get => _brand;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Бренд не може бути порожнім.", nameof(value));
            _brand = value;
        }
    }

    /// <summary>Споживана потужність, Вт.</summary>
    public double PowerConsumption
    {
        get => _powerConsumption;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Потужність не може бути від'ємною.");
            _powerConsumption = value;
        }
    }

    /// <summary>Віртуальний метод: перевизначається у похідних класах.</summary>
    public virtual void TurnOn()
    {
        Console.WriteLine($"[Electronic] {Brand} вмикається. Споживання: {PowerConsumption} Вт.");
    }

    /// <summary>Невіртуальний метод: у Television буде приховано через new.</summary>
    public string GetElectronicType() => "Електронний пристрій";
}
