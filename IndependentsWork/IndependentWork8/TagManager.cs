namespace IndependentWork8;

/// <summary>
/// Система тегів для статей блогу (сценарій 2).
/// Dictionary: ідентифікатор статті -> множина тегів.
/// HashSet гарантує унікальність тегів і дає теоретико-множинні операції.
/// Теги порівнюються без урахування регістру.
/// </summary>
public class TagManager
{
    private readonly Dictionary<string, HashSet<string>> _articleTags =
        new Dictionary<string, HashSet<string>>();

    public void AddTag(string articleId, string tag)
    {
        string normalizedTag = NormalizeTag(tag);

        if (!_articleTags.TryGetValue(articleId, out HashSet<string>? tags))
        {
            tags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            _articleTags[articleId] = tags;
        }

        tags.Add(normalizedTag); // дублікат (з урахуванням регістру) просто ігнорується
    }

    public void RemoveTag(string articleId, string tag)
    {
        if (!_articleTags.TryGetValue(articleId, out HashSet<string>? tags))
        {
            return;
        }

        tags.Remove(NormalizeTag(tag));

        if (tags.Count == 0)
        {
            _articleTags.Remove(articleId); // у статті не лишилось тегів
        }
    }

    /// <summary>
    /// Теги однієї статті (копія: зовнішній код не змінить внутрішній стан).
    /// </summary>
    public HashSet<string> GetTags(string articleId)
    {
        return _articleTags.TryGetValue(articleId, out HashSet<string>? tags)
            ? new HashSet<string>(tags, StringComparer.OrdinalIgnoreCase)
            : new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Усі унікальні теги всіх статей (UnionWith).
    /// </summary>
    public HashSet<string> GetAllUniqueTags()
    {
        HashSet<string> result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (HashSet<string> tags in _articleTags.Values)
        {
            result.UnionWith(tags);
        }

        return result;
    }

    /// <summary>
    /// Теги, спільні для двох статей (IntersectWith).
    /// </summary>
    public HashSet<string> FindCommonTags(string articleId1, string articleId2)
    {
        HashSet<string> common = GetTags(articleId1);
        common.IntersectWith(GetTags(articleId2));
        return common;
    }

    /// <summary>
    /// Теги, що є в першій статті, але відсутні в другій (ExceptWith).
    /// </summary>
    public HashSet<string> FindTagsOnlyIn(string articleId, string otherArticleId)
    {
        HashSet<string> only = GetTags(articleId);
        only.ExceptWith(GetTags(otherArticleId));
        return only;
    }

    private static string NormalizeTag(string tag)
    {
        if (string.IsNullOrWhiteSpace(tag))
        {
            throw new ArgumentException("Тег не може бути порожнім.", nameof(tag));
        }

        return tag.Trim();
    }
}
