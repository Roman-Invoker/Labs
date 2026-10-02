namespace Lab6V14;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine(" Лабораторна робота №4. Варіант 14 ");
        Console.WriteLine("Ієрархія: Electronic -> Television, Radio");
        Console.WriteLine();

   
        Electronic genericDevice = new Electronic("NoName", 5);
        Television tv = new Television("Samsung", 120, "3840x2160");
        Radio radio = new Radio("Sony", 8, "87.5-108 MHz");

        Console.WriteLine(" 1. Власні методи кожного об'єкта ");
        genericDevice.TurnOn();
        tv.TurnOn();
        tv.ChangeChannel();
        radio.TurnOn();
        radio.TuneStation();
        Console.WriteLine();

    
        Console.WriteLine("2. Поліморфна поведінка (масив посилань Electronic) ");
        Electronic[] devices = new Electronic[] { genericDevice, tv, radio };
        foreach (Electronic device in devices)
        {
      
            device.TurnOn();
        }
        Console.WriteLine();

  
        Console.WriteLine("3. Різниця між override (TurnOn) та new (GetElectronicType)");

        Television tvAsTelevision = tv;
        Electronic tvAsElectronic = tv;

        Console.WriteLine("Виклик TurnOn() метод ПЕРЕВИЗНАЧЕНО (override):");
        Console.Write("  через посилання Television: ");
        tvAsTelevision.TurnOn();
        Console.Write("  через посилання Electronic: ");
        tvAsElectronic.TurnOn();
        Console.WriteLine("  -> Результат однаковий, бо override визначається реальним типом об'єкта.");
        Console.WriteLine();

        Console.WriteLine("Виклик GetElectronicType() — метод ПРИХОВАНО (new):");
        Console.WriteLine($"  через посилання Television: {tvAsTelevision.GetElectronicType()}");
        Console.WriteLine($"  через посилання Electronic: {tvAsElectronic.GetElectronicType()}");
        Console.WriteLine("  -> Результат РІЗНИЙ, бо new визначається типом ПОСИЛАННЯ, а не об'єкта.");
        Console.WriteLine();

        Console.WriteLine("Кінець ");
    }
}
