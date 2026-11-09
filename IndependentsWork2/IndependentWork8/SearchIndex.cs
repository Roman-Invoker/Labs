using System.Text.RegularExpressions;

namespace IndependentWork8;

/// <summary>
/// Сценарій 3 (додатковий). Простий інвертований індекс: слово -> множина документів, де воно зустрічається.
/// </summary>
public class SearchIndex
{
    private static readonly Regex WordRegex = new(@"\w+", RegexOptions.Compiled);

    private readonly Dictionary<string, HashSet<int>> _invertedIndex = new();

    public int UniqueWords => _invertedIndex.Count;

    public void AddDocument(int docId, string content)
    {
        ArgumentNullException.ThrowIfNull(content);

        foreach (string word in Tokenize(content))
        {
            if (!_invertedIndex.TryGetValue(word, out HashSet<int>? docs))
            {
                docs = new HashSet<int>();
                _invertedIndex[word] = docs;
            }

            docs.Add(docId);
        }
    }

    /// <summary>Документи, що містять слово (копія множини).</summary>
    public HashSet<int> Search(string word)
    {
        string normalized = word.ToLowerInvariant();
        return _invertedIndex.TryGetValue(normalized, out HashSet<int>? docs) ? new HashSet<int>(docs) : new HashSet<int>();
    }

    /// <summary>Документи, що містять ВСІ слова (перетин множин).</summary>
    public HashSet<int> SearchMultiple(params string[] words)
    {
        if (words.Length == 0)
        {
            return new HashSet<int>();
        }

        HashSet<int> result = Search(words[0]);

        for (int i = 1; i < words.Length && result.Count > 0; i++)
        {
            result.IntersectWith(Search(words[i]));
        }

        return result;
    }

    private static IEnumerable<string> Tokenize(string text)
    {
        foreach (Match match in WordRegex.Matches(text.ToLowerInvariant()))
        {
            yield return match.Value;
        }
    }
}
