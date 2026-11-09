namespace IndependentWork12;

/// <summary>
/// Операція над колекцією: умова відбору (Where) та перетворення (Select).
/// </summary>
public record Operation(string Name, string Pipeline, Func<int, bool> Predicate, Func<int, double> Selector);

public static class Operations
{
    public static readonly Operation[] All =
    {
       
        new("Перевірка на просте число", "Where(IsPrime).Select(Math.Sqrt)",
            IsPrime, x => Math.Sqrt(x)),

      
        new("Складні математичні перетворення", "Where(непарне).Select(HeavyMath)",
            x => (x & 1) == 1, HeavyMath),

       
        new("Дешева операція (контроль)", "Where(парне).Select(x * 2)",
            x => x % 2 == 0, x => x * 2.0)
    };

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
