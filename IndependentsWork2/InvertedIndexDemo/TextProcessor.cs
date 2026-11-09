using System.Text.RegularExpressions;

namespace InvertedIndexDemo;

public static class TextProcessor
{
    private static readonly Regex WordRegex = new(@"\w+", RegexOptions.Compiled);

    private static readonly HashSet<string> StopWords = new()
    {
        "a", "an", "the", "is", "are", "was", "were",
        "in", "on", "at", "to", "for", "of", "and", "or"
    };

    /// <summary>
    /// Розбиває текст на слова, видаляє пунктуацію, переводить у нижній регістр.
    /// Приклад: "Hello, world!" -> ["hello", "world"].
    /// Примітка: \w+ не включає символ '#', тому "C#" перетворюється на "c".
    /// </summary>
    public static IEnumerable<string> Tokenize(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        foreach (Match match in WordRegex.Matches(text.ToLowerInvariant()))
        {
            yield return match.Value;
        }
    }

    /// <summary>Видаляє стоп-слова (a, an, the, is, are, ...).</summary>
    public static IEnumerable<string> RemoveStopWords(IEnumerable<string> words)
    {
        return words.Where(w => !StopWords.Contains(w));
    }
}
