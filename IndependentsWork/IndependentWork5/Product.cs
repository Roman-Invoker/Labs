namespace IndependentWork5;

public class Product
{
    public string Name { get; }
    public decimal Price { get; }

    public Product(string name, decimal price)
    {
        Name = name;
        Price = price;
    }

    public override string ToString()
    {
        return $"Товар \"{Name}\", ціна {Price:F2} грн";
    }
}
