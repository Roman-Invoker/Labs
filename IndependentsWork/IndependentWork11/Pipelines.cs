namespace IndependentWork11;

/// <summary>
/// LINQ-пайплайни аналітики. Кожен метод повертає матеріалізований список (ToList),
/// тому вимірювання часу охоплює реальне виконання, а не лише побудову відкладеного запиту.
/// </summary>
public static class Pipelines
{
    /// <summary>
    /// Пайплайн 1: найпопулярніші продукти за категоріями.
    /// GroupBy (за товаром) -> Select (сума кількості) -> Join (з товарами) -> GroupBy (за категорією)
    /// -> OrderByDescending/ThenBy/Take (топ-N у кожній категорії).
    /// </summary>
    public static List<CategoryTop> TopProductsByCategory(
        IEnumerable<Sale> sales, IEnumerable<Product> products, int top = 3)
    {
        return sales
            .GroupBy(s => s.ProductId)
            .Select(g => new { ProductId = g.Key, TotalQuantity = g.Sum(s => s.Quantity) })
            .Join(products,
                  sold => sold.ProductId,
                  p => p.Id,
                  (sold, p) => new { p.Name, p.Category, sold.TotalQuantity })
            .GroupBy(x => x.Category)
            .Select(g => new CategoryTop(
                g.Key,
                g.OrderByDescending(x => x.TotalQuantity)
                 .ThenBy(x => x.Name, StringComparer.Ordinal) // стабільний порядок за однакової кількості
                 .Take(top)
                 .Select(x => new ProductQuantity(x.Name, x.TotalQuantity))
                 .ToList()))
            .OrderBy(c => c.Category, StringComparer.Ordinal)
            .ToList();
    }

    /// <summary>
    /// Пайплайн 2: загальний дохід за містами клієнтів.
    /// Join (продажі + товари) -> Join (+ клієнти) -> GroupBy (місто) -> Sum/Count -> OrderByDescending.
    /// </summary>
    public static List<CityRevenue> RevenueByCity(
        IEnumerable<Sale> sales, IEnumerable<Product> products, IEnumerable<Customer> customers)
    {
        return sales
            .Join(products,
                  s => s.ProductId,
                  p => p.Id,
                  (s, p) => new { s.CustomerId, Revenue = s.Quantity * p.UnitPrice })
            .Join(customers,
                  x => x.CustomerId,
                  c => c.Id,
                  (x, c) => new { c.City, x.Revenue })
            .GroupBy(x => x.City)
            .Select(g => new CityRevenue(g.Key, g.Sum(x => x.Revenue), g.Count()))
            .OrderByDescending(r => r.Revenue)
            .ThenBy(r => r.City, StringComparer.Ordinal)
            .ToList();
    }

    /// <summary>
    /// Пайплайн 3: продажі за місяцями й роками.
    /// Join (для ціни) -> GroupBy за складеним ключем (рік, місяць) -> Sum -> OrderBy/ThenBy.
    /// </summary>
    public static List<MonthlySales> SalesByMonth(
        IEnumerable<Sale> sales, IEnumerable<Product> products)
    {
        return sales
            .Join(products,
                  s => s.ProductId,
                  p => p.Id,
                  (s, p) => new { s.SaleDate, s.Quantity, Revenue = s.Quantity * p.UnitPrice })
            .GroupBy(x => new { x.SaleDate.Year, x.SaleDate.Month })
            .Select(g => new MonthlySales(
                g.Key.Year,
                g.Key.Month,
                g.Sum(x => x.Quantity),
                g.Sum(x => x.Revenue)))
            .OrderBy(m => m.Year)
            .ThenBy(m => m.Month)
            .ToList();
    }

    /// <summary>
    /// Пайплайн 4 (власний): топ-N клієнтів за сумою покупок із їхньою улюбленою категорією.
    /// Join -> GroupBy (клієнт) -> Select зі вкладеним GroupBy (улюблена категорія)
    /// -> OrderByDescending -> Take -> Join (з клієнтами для імені та міста).
    /// Join зберігає порядок зовнішньої послідовності, тому порядок топ-N не змінюється.
    /// </summary>
    public static List<TopCustomer> TopCustomersWithFavoriteCategory(
        IEnumerable<Sale> sales, IEnumerable<Product> products, IEnumerable<Customer> customers, int top = 5)
    {
        return sales
            .Join(products,
                  s => s.ProductId,
                  p => p.Id,
                  (s, p) => new { s.CustomerId, p.Category, s.Quantity, Revenue = s.Quantity * p.UnitPrice })
            .GroupBy(x => x.CustomerId)
            .Select(g => new
            {
                CustomerId = g.Key,
                Total = g.Sum(x => x.Revenue),
                Orders = g.Count(),
                FavoriteCategory = g
                    .GroupBy(x => x.Category)
                    .OrderByDescending(cg => cg.Sum(x => x.Quantity))
                    .ThenBy(cg => cg.Key, StringComparer.Ordinal)
                    .First().Key
            })
            .OrderByDescending(x => x.Total)
            .ThenBy(x => x.CustomerId)
            .Take(top)
            .Join(customers,
                  x => x.CustomerId,
                  c => c.Id,
                  (x, c) => new TopCustomer(c.Name, c.City, x.Total, x.Orders, x.FavoriteCategory))
            .ToList();
    }

    /// <summary>
    /// Для порівняння: пайплайн 2 без LINQ (цикли та словники). Ті самі результати,
    /// але без проміжних послідовностей та анонімних типів.
    /// </summary>
    public static List<CityRevenue> RevenueByCityManual(
        IEnumerable<Sale> sales, IEnumerable<Product> products, IEnumerable<Customer> customers)
    {
        var priceById = new Dictionary<int, decimal>();
        foreach (Product p in products)
        {
            priceById[p.Id] = p.UnitPrice;
        }

        var cityById = new Dictionary<int, string>();
        foreach (Customer c in customers)
        {
            cityById[c.Id] = c.City;
        }

        var revenueByCity = new Dictionary<string, decimal>();
        var countByCity = new Dictionary<string, int>();

        foreach (Sale s in sales)
        {
            // Семантика внутрішнього Join: продаж без товару чи клієнта пропускається
            if (!priceById.TryGetValue(s.ProductId, out decimal price) ||
                !cityById.TryGetValue(s.CustomerId, out string? city))
            {
                continue;
            }

            revenueByCity.TryGetValue(city, out decimal revenue);
            revenueByCity[city] = revenue + s.Quantity * price;

            countByCity.TryGetValue(city, out int count);
            countByCity[city] = count + 1;
        }

        var result = new List<CityRevenue>(revenueByCity.Count);
        foreach (var pair in revenueByCity)
        {
            result.Add(new CityRevenue(pair.Key, pair.Value, countByCity[pair.Key]));
        }

        result.Sort((a, b) =>
        {
            int byRevenue = b.Revenue.CompareTo(a.Revenue);
            return byRevenue != 0 ? byRevenue : string.CompareOrdinal(a.City, b.City);
        });

        return result;
    }
}
