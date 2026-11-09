namespace IndependentWork11;

/// <summary>Генерує відтворювані (Random з фіксованим Seed) тестові дані для сценарію «Продажі».</summary>
public static class DataGenerator
{
    private static readonly string[] Categories = { "Electronics", "Books", "Home", "Sports", "Toys", "Clothing" };
    private static readonly string[] Cities = { "Rivne", "Kyiv", "Lviv", "Odesa", "Kharkiv", "Dnipro", "Lutsk", "Ternopil" };

    public static List<Product> GenerateProducts(Random random, int count)
    {
        var products = new List<Product>(count);

        for (int id = 1; id <= count; id++)
        {
            string category = Categories[(id - 1) % Categories.Length];
            decimal price = random.Next(50, 5001);
            products.Add(new Product(id, $"{category} item {id:00}", category, price));
        }

        return products;
    }

    public static List<Customer> GenerateCustomers(Random random, int count)
    {
        var customers = new List<Customer>(count);

        for (int id = 1; id <= count; id++)
        {
            customers.Add(new Customer(id, $"Customer {id:00}", Cities[random.Next(Cities.Length)]));
        }

        return customers;
    }

    public static List<Sale> GenerateSales(Random random, int count, int productCount, int customerCount)
    {
        var sales = new List<Sale>(count);
        var start = new DateTime(2025, 1, 1);
        int totalDays = (new DateTime(2026, 10, 31) - start).Days;

        for (int id = 1; id <= count; id++)
        {
            sales.Add(new Sale(
                Id: id,
                ProductId: random.Next(1, productCount + 1),
                Quantity: random.Next(1, 11),
                SaleDate: start.AddDays(random.Next(totalDays + 1)),
                CustomerId: random.Next(1, customerCount + 1)));
        }

        return sales;
    }
}
