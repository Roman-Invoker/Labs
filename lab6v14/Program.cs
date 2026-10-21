using Lab6v14;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Electronic device = new Electronic("Generic", 50);
Television tv = new Television("Samsung", 120, "4K");
Radio radio = new Radio("Sony", 15, "FM 87.5-108 МГц");

Console.WriteLine("=== 1. Виклик через посилання власного типу ===");
device.TurnOn();
tv.TurnOn();
radio.TurnOn();

Console.WriteLine();
Console.WriteLine("=== 2. Поліморфізм: посилання базового типу (virtual/override) ===");
Electronic[] devices = { device, tv, radio };
foreach (Electronic d in devices)
{
    d.TurnOn();
}

Console.WriteLine();
Console.WriteLine("=== 3. Власні методи похідних класів ===");
tv.ChangeChannel(5);
radio.TuneStation(101.7);

Console.WriteLine();
Console.WriteLine("=== 4. Різниця між override та new ===");
Electronic tvAsBase = tv; // той самий об'єкт, але посилання базового типу
Console.WriteLine($"tv.GetElectronicType()        -> {tv.GetElectronicType()}");
Console.WriteLine($"tvAsBase.GetElectronicType()  -> {tvAsBase.GetElectronicType()}");
Console.WriteLine("tvAsBase.TurnOn() (override):");
tvAsBase.TurnOn();
