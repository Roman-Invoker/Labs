namespace InvertedIndexDemo;

/// <summary>
/// Інвертований індекс: слово -> множина ідентифікаторів документів, що його містять.
/// </summary>
public class InvertedIndex
{
    // Слово -> множина ID документів
    private readonly Dictionary<string, HashSet<int>> _index;

    // ID -> документ (для швидкого отримання)
    private readonly Dictionary<int, Document> _documents;

    private readonly bool _removeStopWords;

    /// <param name="removeStopWords">Чи виключати стоп-слова з індексу та з пошукових запитів.</param>
    public InvertedIndex(bool removeStopWords = false)
    {
        _index = new Dictionary<string, HashSet<int>>();
        _documents = new Dictionary<int, Document>();
        _removeStopWords = removeStopWords;
    }

    public int DocumentCount => _documents.Count;

    public int UniqueWords => _index.Keys.Count;

    /// <summary>Додає документ до індексу.</summary>
    public void AddDocument(Document doc)
    {
        ArgumentNullException.ThrowIfNull(doc);

        // 1. Зберегти документ
        _documents[doc.Id] = doc;

        // 2. Розбити Content на слова (нижній регістр, без пунктуації)
        foreach (string word in Normalize(doc.Content))
        {
            // 3. Додати doc.Id до _index[word]
            if (!_index.TryGetValue(word, out HashSet<int>? ids))
            {
                ids = new HashSet<int>();
                _index[word] = ids;
            }

            ids.Add(doc.Id);
        }
    }

    /// <summary>Шукає документи, що містять слово.</summary>
    public List<Document> SearchSingleWord(string word)
    {
        List<string> tokens = Normalize(word).ToList();

        if (tokens.Count == 0 || !_index.TryGetValue(tokens[0], out HashSet<int>? ids))
        {
            return new List<Document>();
        }

        return ToDocuments(ids);
    }

    /// <summary>Шукає документи, що містять ВСІ слова (AND) — перетин множин через IntersectWith.</summary>
    public List<Document> SearchMultipleWords(params string[] words)
    {
        List<string> tokens = words.SelectMany(Normalize).Distinct().ToList();

        if (tokens.Count == 0)
        {
            return new List<Document>();
        }

        HashSet<int>? result = null;

        foreach (string token in tokens)
        {
            if (!_index.TryGetValue(token, out HashSet<int>? ids))
            {
                return new List<Document>(); // одного зі слів ніде немає — AND порожній
            }

            if (result is null)
            {
                result = new HashSet<int>(ids); // копія, щоб не змінювати індекс
            }
            else
            {
                result.IntersectWith(ids);
            }

            if (result.Count == 0)
            {
                break;
            }
        }

        return ToDocuments(result!);
    }

    /// <summary>Шукає документи, що містять БУДЬ-ЯКЕ слово (OR) — об'єднання множин через UnionWith.</summary>
    public List<Document> SearchAnyWord(params string[] words)
    {
        var result = new HashSet<int>();

        foreach (string token in words.SelectMany(Normalize).Distinct())
        {
            if (_index.TryGetValue(token, out HashSet<int>? ids))
            {
                result.UnionWith(ids);
            }
        }

        return ToDocuments(result);
    }

    private IEnumerable<string> Normalize(string text)
    {
        IEnumerable<string> tokens = TextProcessor.Tokenize(text);
        return _removeStopWords ? TextProcessor.RemoveStopWords(tokens) : tokens;
    }

    private List<Document> ToDocuments(IEnumerable<int> ids)
    {
        return ids.OrderBy(id => id).Select(id => _documents[id]).ToList();
    }
}
