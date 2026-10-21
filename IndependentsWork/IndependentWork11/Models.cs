namespace IndependentWork11;

// ---------- Сутності сценарію "Продажі" ----------

public class Sale
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public DateTime SaleDate { get; set; }
    public int CustomerId { get; set; }
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
}

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
}

/// <summary>
/// Усі три колекції разом.
/// </summary>
public record Dataset(List<Product> Products, List<Customer> Customers, List<Sale> Sales);

// ---------- Результати пайплайнів ----------

public record ProductQuantity(string Name, int Quantity);

public record CategoryTop(string Category, List<ProductQuantity> TopProducts);

public record CityRevenue(string City, decimal Revenue, int SalesCount);

public record MonthlySales(int Year, int Month, int Quantity, decimal Revenue);

public record TopCustomer(string Name, string City, decimal TotalSpent, int OrdersCount, string FavoriteCategory);
