using System.Globalization;
using IndependentWork2;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        CultureInfo.CurrentCulture = new CultureInfo("uk-UA");

        Console.WriteLine("Створення товарів");

        
        Product laptop = new Product(101, "Laptop", 35000m, "Electronics", 15);
        Console.WriteLine($"Товар 1 (основний конструктор): {laptop}");

        
        Product mouse = new Product(102, "Mouse", 800m);
        Console.WriteLine($"Товар 2 (скорочений конструктор): {mouse}");

        
        Product original = new Product(101, "Laptop", 35000m, "Electronics", 15);
        Product copy = new Product(original);
        Console.WriteLine($"Товар 3 (конструктор копіювання): {copy}");

        Console.WriteLine();
        Console.WriteLine($"Копія і оригінал - різні об'єкти: {!ReferenceEquals(original, copy)}");
    }
}
