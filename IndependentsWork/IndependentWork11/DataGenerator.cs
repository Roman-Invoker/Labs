namespace IndependentWork11;

/// <summary>
/// Генерація тестових даних. Random з фіксованим seed, тому дані однакові під час кожного запуску.
/// </summary>
public static class DataGenerator
{
    private static readonly string[] Categories =
    {
        "Електроніка", "Одяг", "Книги", "Дім та сад", "Спорт", "Іграшки"
    };

    // По 10 товарів у кожній категорії (разом 60)
    private static readonly string[][] ProductNames =
    {
        new[] { "Смартфон", "Ноутбук", "Навушники", "Планшет", "Монітор", "Клавіатура", "Миша", "Зарядний пристрій", "Колонка", "Смарт-годинник" },
        new[] { "Куртка", "Футболка", "Джинси", "Светр", "Кросівки", "Шапка", "Шарф", "Сукня", "Пальто", "Шорти" },
        new[] { "Роман", "Детектив", "Підручник", "Словник", "Енциклопедія", "Збірка віршів", "Фентезі", "Біографія", "Комікс", "Кулінарна книга" },
        new[] { "Лампа", "Горщик", "Килим", "Набір посуду", "Штори", "Лопата", "Шланг", "Подушка", "Плед", "Свічки" },
        new[] { "М'яч", "Гантелі", "Килимок для йоги", "Велосипед", "Скакалка", "Рукавиці", "Ролики", "Намет", "Рюкзак", "Фляга" },
        new[] { "Конструктор", "Пазл", "Лялька", "Машинка", "Настільна гра", "М'яка іграшка", "Роботизований набір", "Кубики", "Потяг", "Фарби" }
    };

    private static readonly string[] FirstNames =
        { "Олена", "Андрій", "Марія", "Іван", "Софія", "Тарас", "Наталія", "Богдан", "Ірина", "Максим" };

    private static readonly string[] LastNames =
        { "Коваленко", "Шевчук", "Бондаренко", "Мельник", "Ткаченко", "Кравчук", "Олійник", "Поліщук" };

    private static readonly string[] Cities =
        { "Київ", "Львів", "Одеса", "Харків", "Дніпро", "Рівне", "Запоріжжя", "Вінниця" };

    /// <summary>
    /// 60 товарів, 80 клієнтів і salesCount продажів за 2024-2025 роки.
    /// </summary>
    public static Dataset Generate(int salesCount = 1000, int seed = 42)
    {
        var random = new Random(seed);

        // Товари
        var products = new List<Product>();
        int productId = 1;
        for (int c = 0; c < Categories.Length; c++)
        {
            foreach (string name in ProductNames[c])
            {
                products.Add(new Product
                {
                    Id = productId++,
                    Name = name,
                    Category = Categories[c],
                    UnitPrice = random.Next(50, 5001)
                });
            }
        }

        // Клієнти: 10 імен x 8 прізвищ = 80 унікальних поєднань
        var customers = new List<Customer>();
        for (int i = 0; i < FirstNames.Length * LastNames.Length; i++)
        {
            customers.Add(new Customer
            {
                Id = i + 1,
                Name = $"{FirstNames[i % FirstNames.Length]} {LastNames[i / FirstNames.Length]}",
                City = Cities[random.Next(Cities.Length)]
            });
        }

        // Продажі: розподіл за товарами зміщено до перших товарів (квадрат випадкового числа),
        // щоб "найпопулярніші" товари справді відрізнялися від решти
        var sales = new List<Sale>(salesCount);
        DateTime start = new DateTime(2024, 1, 1);
        for (int i = 1; i <= salesCount; i++)
        {
            int productIndex = (int)(Math.Pow(random.NextDouble(), 2) * products.Count);

            sales.Add(new Sale
            {
                Id = i,
                ProductId = products[productIndex].Id,
                Quantity = random.Next(1, 11),
                SaleDate = start.AddDays(random.Next(0, 731)), // 2024-01-01 ... 2025-12-31
                CustomerId = random.Next(1, customers.Count + 1)
            });
        }

        return new Dataset(products, customers, sales);
    }
}
