namespace IndependentWork11;

/// <summary>LINQ-пайплайни аналітики продажів. Кожен пайплайн — окремий метод.</summary>
public static class SalesPipelines
{
    /// <summary>
    /// Пайплайн 1. Топ-3 найпопулярніші (за кількістю проданих одиниць) продукти в кожній категорії.
    /// GroupBy (продажі за продуктом) -> Sum -> Join (з каталогом) -> GroupBy (за категорією) -> OrderByDescending -> Take(3).
    /// </summary>
    public static List<CategoryTop> TopProductsByCategory(
        IReadOnlyList<Sale> sales, IReadOnlyList<Product> products)
    {
        return sales
            .GroupBy(s => s.ProductId)
            .Select(g => new { ProductId = g.Key, Quantity = g.Sum(s => s.Quantity) })
            .Join(products,
                total => total.ProductId,
                product => product.Id,
                (total, product) => new { product.Category, product.Name, total.Quantity })
            .GroupBy(x => x.Category)
            .OrderBy(g => g.Key)
            .Select(g => new CategoryTop(
                g.Key,
                g.OrderByDescending(x => x.Quantity)
                 .ThenBy(x => x.Name)
                 .Take(3)
                 .Select(x => new ProductQuantity(x.Name, x.Quantity))
                 .ToList()))
            .ToList();
    }

    /// <summary>
    /// Пайплайн 2. Загальний дохід за містами клієнтів.
    /// Join (продажі + товари) -> Join (+ клієнти) -> дохід = Quantity * UnitPrice -> GroupBy (місто) -> Sum -> OrderByDescending.
    /// </summary>
    public static List<CityRevenue> RevenueByCity(
        IReadOnlyList<Sale> sales, IReadOnlyList<Product> products, IReadOnlyList<Customer> customers)
    {
        return sales
            .Join(products, s => s.ProductId, p => p.Id, (s, p) => new { s.CustomerId, Revenue = s.Quantity * p.UnitPrice })
            .Join(customers, x => x.CustomerId, c => c.Id, (x, c) => new { c.City, x.Revenue })
            .GroupBy(x => x.City)
            .Select(g => new CityRevenue(g.Key, g.Sum(x => x.Revenue)))
            .OrderByDescending(x => x.Revenue)
            .ToList();
    }

    /// <summary>
    /// Пайплайн 3. Продажі за роками та місяцями: кількість одиниць і дохід.
    /// Join (з товарами) -> GroupBy (рік, місяць) -> Sum -> OrderBy (рік) -> ThenBy (місяць).
    /// </summary>
    public static List<MonthlySales> SalesByYearMonth(
        IReadOnlyList<Sale> sales, IReadOnlyList<Product> products)
    {
        return sales
            .Join(products, s => s.ProductId, p => p.Id,
                (s, p) => new { s.SaleDate.Year, s.SaleDate.Month, s.Quantity, Revenue = s.Quantity * p.UnitPrice })
            .GroupBy(x => new { x.Year, x.Month })
            .Select(g => new MonthlySales(g.Key.Year, g.Key.Month, g.Sum(x => x.Quantity), g.Sum(x => x.Revenue)))
            .OrderBy(x => x.Year)
            .ThenBy(x => x.Month)
            .ToList();
    }

    /// <summary>
    /// Пайплайн 4 (додатковий). Топ-5 клієнтів за сумою витрат та їхня улюблена категорія.
    /// Join (товари) -> GroupBy (клієнт) -> Sum + внутрішній GroupBy (категорія) -> Join (клієнти) -> OrderByDescending -> Take(5).
    /// </summary>
    public static List<CustomerSpending> TopCustomersWithFavoriteCategory(
        IReadOnlyList<Sale> sales, IReadOnlyList<Product> products, IReadOnlyList<Customer> customers)
    {
        return sales
            .Join(products, s => s.ProductId, p => p.Id,
                (s, p) => new { s.CustomerId, p.Category, s.Quantity, Revenue = s.Quantity * p.UnitPrice })
            .GroupBy(x => x.CustomerId)
            .Select(g => new
            {
                CustomerId = g.Key,
                Total = g.Sum(x => x.Revenue),
                FavoriteCategory = g
                    .GroupBy(x => x.Category)
                    .OrderByDescending(cg => cg.Sum(x => x.Quantity))
                    .ThenBy(cg => cg.Key)
                    .First().Key
            })
            .Join(customers, x => x.CustomerId, c => c.Id,
                (x, c) => new CustomerSpending(c.Name, c.City, x.Total, x.FavoriteCategory))
            .OrderByDescending(x => x.TotalSpent)
            .Take(5)
            .ToList();
    }
}
