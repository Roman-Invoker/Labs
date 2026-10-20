namespace IndependentWork8;

/// <summary>
/// Простий пошуковий індекс (сценарій 3).
/// Інвертований індекс: слово -> множина документів, де воно зустрічається.
/// Dictionary дає швидкий доступ до слова, HashSet не допускає дублікатів ідентифікаторів
/// і дозволяє перетинати множини для пошуку за кількома словами.
/// </summary>
public class SearchIndex
{
    private static readonly char[] Separators =
        { ' ', ',', '.', '!', '?', ';', ':', '(', ')', '"', '-', '\n', '\r', '\t' };

    private readonly Dictionary<string, HashSet<int>> _invertedIndex =
        new Dictionary<string, HashSet<int>>(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Кількість унікальних слів в індексі.
    /// </summary>
    public int WordCount
    {
        get { return _invertedIndex.Count; }
    }

    public void AddDocument(int docId, string content)
    {
        string[] words = content.Split(Separators, StringSplitOptions.RemoveEmptyEntries);

        foreach (string word in words)
        {
            if (!_invertedIndex.TryGetValue(word, out HashSet<int>? documents))
            {
                documents = new HashSet<int>();
                _invertedIndex[word] = documents;
            }

            documents.Add(docId); // повторення слова в документі не створює дубліката
        }
    }

    /// <summary>
    /// Документи, що містять слово (копія множини).
    /// </summary>
    public HashSet<int> Search(string word)
    {
        return _invertedIndex.TryGetValue(word, out HashSet<int>? documents)
            ? new HashSet<int>(documents)
            : new HashSet<int>();
    }

    /// <summary>
    /// Документи, що містять УСІ вказані слова (перетин множин).
    /// </summary>
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
}
