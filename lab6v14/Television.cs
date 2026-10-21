namespace Lab6v14;

/// <summary>Похідний клас: телевізор.</summary>
public class Television : Electronic
{
    private string _screenResolution;

    public Television(string brand, double powerConsumption, string screenResolution)
        : base(brand, powerConsumption)
    {
        _screenResolution = string.Empty;
        ScreenResolution = screenResolution;
    }

    public string ScreenResolution
    {
        get => _screenResolution;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Роздільна здатність не може бути порожньою.", nameof(value));
            _screenResolution = value;
        }
    }

    public override void TurnOn()
    {
        base.TurnOn();
        Console.WriteLine($"[Television] Екран {ScreenResolution} активовано.");
    }

    /// <summary>Власний унікальний метод.</summary>
    public void ChangeChannel(int channel)
    {
        Console.WriteLine($"[Television] {Brand}: перемикання на канал {channel}.");
    }

    /// <summary>Приховування методу базового класу (не override!).</summary>
    public new string GetElectronicType() => "Телевізор";
}
