namespace IndependentWork8;

/// <summary>
/// Сценарій 2. Теги статей блогу: Dictionary&lt;ArticleId, HashSet&lt;Tag&gt;&gt;.
/// HashSet гарантує унікальність тегів у межах статті, а також дає швидкі теоретико-множинні операції
/// (UnionWith, IntersectWith, ExceptWith). Теги порівнюються без урахування регістру.
/// </summary>
public class TagManager
{
    private readonly Dictionary<string, HashSet<string>> _articleTags = new();

    /// <summary>Додає тег до статті (повторне додавання того самого тегу ігнорується).</summary>
    public void AddTag(string articleId, string tag)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(articleId);
        ArgumentException.ThrowIfNullOrWhiteSpace(tag);

        if (!_articleTags.TryGetValue(articleId, out HashSet<string>? tags))
        {
            tags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            _articleTags[articleId] = tags;
        }

        tags.Add(tag.Trim());
    }

    /// <summary>Видаляє тег зі статті. Якщо в статті не лишилось тегів — запис про статтю прибирається.</summary>
    public void RemoveTag(string articleId, string tag)
    {
        if (!_articleTags.TryGetValue(articleId, out HashSet<string>? tags))
        {
            return;
        }

        tags.Remove(tag.Trim());

        if (tags.Count == 0)
        {
            _articleTags.Remove(articleId);
        }
    }

    /// <summary>Усі унікальні теги по всіх статтях (об'єднання множин — UnionWith).</summary>
    public HashSet<string> GetAllUniqueTags()
    {
        var all = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (HashSet<string> tags in _articleTags.Values)
        {
            all.UnionWith(tags);
        }

        return all;
    }

    /// <summary>Спільні теги двох статей (перетин множин — IntersectWith). Повертає копію.</summary>
    public HashSet<string> FindCommonTags(string articleId1, string articleId2)
    {
        var common = CopyTags(articleId1);
        common.IntersectWith(GetTagsOrEmpty(articleId2));
        return common;
    }

    /// <summary>Теги, які є в першій статті, але відсутні в другій (різниця множин — ExceptWith). Повертає копію.</summary>
    public HashSet<string> FindTagsOnlyInFirst(string articleId1, string articleId2)
    {
        var only = CopyTags(articleId1);
        only.ExceptWith(GetTagsOrEmpty(articleId2));
        return only;
    }

    /// <summary>Теги однієї статті (копія; порожня множина, якщо статті немає).</summary>
    public HashSet<string> GetTags(string articleId) => CopyTags(articleId);

    private HashSet<string> CopyTags(string articleId) =>
        new(GetTagsOrEmpty(articleId), StringComparer.OrdinalIgnoreCase);

    private IEnumerable<string> GetTagsOrEmpty(string articleId) =>
        _articleTags.TryGetValue(articleId, out HashSet<string>? tags) ? tags : Enumerable.Empty<string>();
}
