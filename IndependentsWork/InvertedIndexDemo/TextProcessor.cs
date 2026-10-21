using System.Text.RegularExpressions;

namespace InvertedIndexDemo;

/// <summary>
/// Єдине місце нормалізації тексту. І інвертований індекс, і лінійний пошук використовують
/// саме ці методи, тому порівняння продуктивності коректне.
/// </summary>
public static class TextProcessor
{
    // \w+ - послідовності літер, цифр і підкреслення: розділові знаки відкидаються самі
    private static readonly Regex WordRegex = new Regex(@"\w+", RegexOptions.Compiled);

    private static readonly HashSet<string> StopWords = new HashSet<string>
    {
        "a", "an", "the", "is", "are", "was", "were",
        "in", "on", "at", "to", "for", "of", "and", "or"
    };

    /// <summary>
    /// Розбиває текст на слова, видаляє пунктуацію, переводить у нижній регістр.
    /// Приклад: "Hello, world!" -> ["hello", "world"]; "C#" -> ["c"].
    /// </summary>
    public static IEnumerable<string> Tokenize(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            yield break;
        }

        foreach (Match match in WordRegex.Matches(text.ToLowerInvariant()))
        {
            yield return match.Value;
        }
    }

    /// <summary>
    /// Видаляє стоп-слова (a, an, the, is, are, ...).
    /// </summary>
    public static IEnumerable<string> RemoveStopWords(IEnumerable<string> words)
    {
        return words.Where(w => !StopWords.Contains(w));
    }
}
