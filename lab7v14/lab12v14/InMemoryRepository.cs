public class InMemoryRepository<T> : IRepository<T> where T : IEntity
{
    private readonly List<T> _items = new();

    public T? GetById(int id) => _items.FirstOrDefault(x => x.Id == id);

    public IEnumerable<T> GetAll() => _items.ToList();

    public void Add(T item)
    {
        ArgumentNullException.ThrowIfNull(item);
        if (_items.Any(x => x.Id == item.Id))
            throw new InvalidOperationException($"Елемент з Id={item.Id} уже існує.");
        _items.Add(item);
    }

    public void Update(T item)
    {
        ArgumentNullException.ThrowIfNull(item);
        var index = _items.FindIndex(x => x.Id == item.Id);
        if (index < 0)
            throw new KeyNotFoundException($"Елемент з Id={item.Id} не знайдено.");
        _items[index] = item;
    }

    public void Delete(int id)
    {
        var index = _items.FindIndex(x => x.Id == id);
        if (index < 0)
            throw new KeyNotFoundException($"Елемент з Id={id} не знайдено.");
        _items.RemoveAt(index);
    }
}
