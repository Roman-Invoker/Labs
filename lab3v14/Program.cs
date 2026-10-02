using System;

namespace Lab3v14;

internal class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        
        Console.WriteLine("=== Сценарій 1: об'єкт створено через using ===");
        using (var client = new HttpClient("https://api.example.com"))
        {
            client.Get("/users");
            client.Get("/users/42");
        } 
        Console.WriteLine("Вийшли з блоку using\n");

        
        Console.WriteLine("=== Сценарій 2: без using, явний виклик Dispose() ===");
        var client2 = new HttpClient("https://api.weather.ua");
        client2.Get("/forecast/today");
        Console.WriteLine($"IsConnected до Dispose(): {client2.IsConnected}");
        client2.Dispose();
        Console.WriteLine($"IsConnected після Dispose(): {client2.IsConnected}");
        client2.Get("/forecast/tomorrow"); 
        Console.WriteLine();

       
        Console.WriteLine("=== Сценарій 3: без Dispose(), звільнення через GC ===");
        CreateWithoutDispose();

        Console.WriteLine("Примусовий виклик GC.Collect()...");
        GC.Collect();                      
        GC.WaitForPendingFinalizers();      
        Console.WriteLine("Фіналізацію завершено\n");

        Console.WriteLine("Програму завершено.");
    }

    
    static void CreateWithoutDispose()
    {
        var client3 = new HttpClient("https://api.github.com");
        client3.Get("/repos");
    }
}
