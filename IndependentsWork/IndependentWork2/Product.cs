namespace IndependentWork2;

/// <summary>
/// Товар інтернет-магазину. Після створення об'єкта його дані змінити не можна.
/// </summary>
public class Product
{
    private readonly int _id;
    private readonly string _name;
    private readonly decimal _price;
    private readonly string _category;
    private readonly int _stockCount;

    
    public int Id => _id;
    public string Name => _name;
    public decimal Price => _price;
    public string Category => _category;
    public int StockCount => _stockCount;

    /// <summary>
    /// Конструктор 1 (основний): приймає всі п'ять параметрів.
    /// </summary>
    public Product(int id, string name, decimal price, string category, int stockCount)
    {
        _id = id;
        _name = name;
        _price = price;
        _category = category;
        _stockCount = stockCount;
    }

    /// <summary>
    /// Конструктор 2 (швидке додавання): відомі лише id, назва та ціна.
    /// </summary>
    public Product(int id, string name, decimal price)
        : this(id, name, price, "Uncategorized", 0)
    {
    }

    /// <summary>
    /// Конструктор 3 (копіювання): створює дублікат наявного товару.
    /// </summary>
    public Product(Product other)
        : this(other.Id, other.Name, other.Price, other.Category, other.StockCount)
    {
    }

    public override string ToString()
    {
        return $"ID: {Id}, Name: {Name}, Price: {Price:C}, Category: {Category}, Stock: {StockCount}";
    }
}
