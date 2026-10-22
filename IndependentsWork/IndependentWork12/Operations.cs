namespace IndependentWork12;

/// <summary>
/// Операція над колекцією: умова відбору (Where) та перетворення (Select).
/// </summary>
public record Operation(string Name, string Pipeline, Func<int, bool> Predicate, Func<int, double> Selector);

public static class Operations
{
    public static readonly Operation[] All =
    {
        // Дорога перевірка Where + дешеве Select
        new("Перевірка на просте число", "Where(IsPrime).Select(Math.Sqrt)",
            IsPrime, x => Math.Sqrt(x)),

        // Дешевий Where + дорогий Select (кілька десятків викликів Sqrt/Sin на елемент)
        new("Складні математичні перетворення", "Where(непарне).Select(HeavyMath)",
            x => (x & 1) == 1, HeavyMath),

        // Контроль: операція настільки дешева, що паралелізація не має окупитися
        new("Дешева операція (контроль)", "Where(парне).Select(x * 2)",
            x => x % 2 == 0, x => x * 2.0)
    };

    /// <summary>
    /// Перевірка на просте число діленням на непарні числа до кореня.
    /// </summary>
    public static bool IsPrime(int n)
    {
        if (n < 2)
        {
            return false;
        }

        if (n % 2 == 0)
        {
            return n == 2;
        }

        for (int i = 3; (long)i * i <= n; i += 2)
        {
            if (n % i == 0)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Штучно складне математичне перетворення: 20 ітерацій Sqrt і Sin.
    /// Детерміноване й без NaN (підкореневий вираз завжди невід'ємний).
    /// </summary>
    public static double HeavyMath(int x)
    {
        double r = x;
        for (int i = 0; i < 20; i++)
        {
            r = Math.Sqrt(r * r + i) + Math.Sin(r);
        }
        return r;
    }
}
