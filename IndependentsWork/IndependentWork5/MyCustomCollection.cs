using System.Collections;

namespace IndependentWork5;

/// <summary>
/// Власна колекція рядків. Реалізація IEnumerable&lt;string&gt; дозволяє перебирати її через foreach.
/// </summary>
public class MyCustomCollection : IEnumerable<string>
{
    private readonly List<string> _items = new List<string>();

    public void Add(string item)
    {
        _items.Add(item);
    }

    // Узагальнений GetEnumerator(): саме його використовує foreach
    public IEnumerator<string> GetEnumerator()
    {
        foreach (string item in _items)
        {
            yield return item;
        }
    }

    // Неузагальнений GetEnumerator() вимагає інтерфейс IEnumerable (базовий для IEnumerable<T>)
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
