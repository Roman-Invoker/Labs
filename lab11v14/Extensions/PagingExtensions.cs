namespace lab17v14.Extensions;

public static class PagingExtensions
{
    /// <summary>
    /// Повертає елементи вказаної сторінки (нумерація сторінок з 1) через Skip/Take.
    /// </summary>
    public static IEnumerable<T> GetPage<T>(this IEnumerable<T> source, int pageNumber, int pageSize)
    {
        ArgumentNullException.ThrowIfNull(source);
        if (pageNumber < 1) throw new ArgumentOutOfRangeException(nameof(pageNumber));
        if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize));

        return source.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }

    /// <summary>Кількість сторінок для колекції заданого розміру.</summary>
    public static int PageCount<T>(this IEnumerable<T> source, int pageSize)
    {
        ArgumentNullException.ThrowIfNull(source);
        if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize));

        return (int)Math.Ceiling(source.Count() / (double)pageSize);
    }
}
