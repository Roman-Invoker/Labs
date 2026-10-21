namespace Lab14v14;

/// <summary>Власний клас для демонстрації роботи з колекцією об'єктів.</summary>
public class Product
{
    public Product(string name, decimal price, string category)
    {
        Name = name;
        Price = price;
        Category = category;
    }

    public string Name { get; }
    public decimal Price { get; }
    public string Category { get; }
}
