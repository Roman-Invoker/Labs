namespace InvertedIndexDemo;

/// <summary>
/// Інвертований індекс: слово -> множина ідентифікаторів документів, де воно зустрічається.
/// </summary>
public class InvertedIndex
{
   
    private readonly Dictionary<string, HashSet<int>> _index;

   
    private readonly Dictionary<int, Document> _documents;

       private readonly bool _removeStopWords;

    public InvertedIndex(bool removeStopWords = false)
    {
        _index = new Dictionary<string, HashSet<int>>();
        _documents = new Dictionary<int, Document>();
        _removeStopWords = removeStopWords;
    }

    public int DocumentCount => _documents.Count;
    public int UniqueWords => _index.Keys.Count;

    /// <summary>
    /// Додає документ до індексу.
    /// </summary>
    /// <exception cref="ArgumentException">Документ з таким Id уже додано.</exception>
    public void AddDocument(Document doc)
    {
        ArgumentNullException.ThrowIfNull(doc);

        if (_documents.ContainsKey(doc.Id))
        {
            throw new ArgumentException($"Документ з Id {doc.Id} уже додано.", nameof(doc));
        }

       
        _documents[doc.Id] = doc;

       
        foreach (string word in Normalize(doc.Content))
        {
            if (!_index.TryGetValue(word, out HashSet<int>? ids))
            {
                ids = new HashSet<int>();
                _index[word] = ids;
            }

            ids.Add(doc.Id);
        }
    }

    /// <summary>
    /// Шукає документи, що містять слово. Запит нормалізується так само, як і документи
    /// (наприклад, "C#" -> "c").
    /// </summary>
    public List<Document> SearchSingleWord(string word)
    {
        ArgumentNullException.ThrowIfNull(word);
        return SearchMultipleWords(word);
    }

    /// <summary>
    /// Шукає документи, що містять ВСІ слова (AND) - через HashSet.IntersectWith.
    /// </summary>
    public List<Document> SearchMultipleWords(params string[] words)
    {
        ArgumentNullException.ThrowIfNull(words);

        List<string> terms = words.SelectMany(w => Normalize(w)).Distinct().ToList();
        if (terms.Count == 0)
        {
            return new List<Document>();
        }

        
        var postings = new List<HashSet<int>>();
        foreach (string term in terms)
        {
            if (!_index.TryGetValue(term, out HashSet<int>? ids))
            {
                return new List<Document>();
            }
            postings.Add(ids);
        }

        
        postings.Sort((a, b) => a.Count.CompareTo(b.Count));

        var result = new HashSet<int>(postings[0]); 
        for (int i = 1; i < postings.Count && result.Count > 0; i++)
        {
            result.IntersectWith(postings[i]);
        }

        return ToDocuments(result);
    }

    /// <summary>
    /// Шукає документи, що містять БУДЬ-ЯКЕ слово (OR) - через HashSet.UnionWith.
    /// </summary>
    public List<Document> SearchAnyWord(params string[] words)
    {
        ArgumentNullException.ThrowIfNull(words);

        var result = new HashSet<int>();
        foreach (string term in words.SelectMany(w => Normalize(w)).Distinct())
        {
            if (_index.TryGetValue(term, out HashSet<int>? ids))
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

    
    private List<Document> ToDocuments(HashSet<int> ids)
    {
        return ids.OrderBy(id => id).Select(id => _documents[id]).ToList();
    }
}
