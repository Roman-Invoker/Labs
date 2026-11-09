namespace IndependentWork11;

/// <summary>Продаж: який товар, скільки одиниць, коли і кому.</summary>
public record Sale(int Id, int ProductId, int Quantity, DateTime SaleDate, int CustomerId);

/// <summary>Товар каталогу.</summary>
public record Product(int Id, string Name, string Category, decimal UnitPrice);

/// <summary>Клієнт.</summary>
public record Customer(int Id, string Name, string City);

// Типи результатів пайплайнів
public record ProductQuantity(string Name, int Quantity);

public record CategoryTop(string Category, List<ProductQuantity> TopProducts);

public record CityRevenue(string City, decimal Revenue);

public record MonthlySales(int Year, int Month, int Units, decimal Revenue);

public record CustomerSpending(string Name, string City, decimal TotalSpent, string FavoriteCategory);
