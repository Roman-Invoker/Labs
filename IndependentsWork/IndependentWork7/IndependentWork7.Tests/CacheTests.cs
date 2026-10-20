namespace IndependentWork7.Tests;

public class CacheTests
{
    private static Cache<int, string> CreateCache(int capacity)
    {
        return new Cache<int, string>(capacity);
    }

    // ---------------- Конструктор ----------------

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Constructor_NonPositiveCapacity_ThrowsArgumentOutOfRangeException(int capacity)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Cache<int, string>(capacity));
    }

    [Fact]
    public void Constructor_ValidCapacity_CreatesEmptyCache()
    {
        Cache<int, string> cache = CreateCache(3);

        Assert.Equal(0, cache.Count);
    }

    // ---------------- Add ----------------

    [Fact]
    public void Add_NewItem_ItemIsStored()
    {
        Cache<int, string> cache = CreateCache(3);

        cache.Add(1, "one");

        Assert.True(cache.ContainsKey(1));
        Assert.Equal("one", cache.Get(1));
    }

    [Fact]
    public void Add_SeveralItems_AllItemsAreStored()
    {
        Cache<int, string> cache = CreateCache(3);

        cache.Add(1, "one");
        cache.Add(2, "two");
        cache.Add(3, "three");

        Assert.Equal("one", cache.Get(1));
        Assert.Equal("two", cache.Get(2));
        Assert.Equal("three", cache.Get(3));
    }

    [Fact]
    public void Add_ExistingKey_UpdatesValue()
    {
        Cache<int, string> cache = CreateCache(3);
        cache.Add(1, "old");

        cache.Add(1, "new");

        Assert.Equal("new", cache.Get(1));
        Assert.Equal(1, cache.Count);
    }

    [Fact]
    public void Add_ExistingKeyWhenCacheIsFull_DoesNotEvictAnything()
    {
        Cache<int, string> cache = CreateCache(2);
        cache.Add(1, "one");
        cache.Add(2, "two");

        cache.Add(1, "updated");

        Assert.Equal(2, cache.Count);
        Assert.True(cache.ContainsKey(1));
        Assert.True(cache.ContainsKey(2));
    }

    [Fact]
    public void Add_ExistingKey_DoesNotChangeEvictionOrder()
    {
        Cache<int, string> cache = CreateCache(2);
        cache.Add(1, "one");
        cache.Add(2, "two");
        cache.Add(1, "updated"); // 1 залишається найстарішим

        cache.Add(3, "three");

        Assert.False(cache.ContainsKey(1));
        Assert.True(cache.ContainsKey(2));
        Assert.True(cache.ContainsKey(3));
    }

    // ---------------- Get ----------------

    [Fact]
    public void Get_ExistingKey_ReturnsValue()
    {
        Cache<string, int> cache = new Cache<string, int>(2);
        cache.Add("a", 42);

        int value = cache.Get("a");

        Assert.Equal(42, value);
    }

    [Fact]
    public void Get_MissingKey_ThrowsKeyNotFoundException()
    {
        Cache<int, string> cache = CreateCache(3);
        cache.Add(1, "one");

        Assert.Throws<KeyNotFoundException>(() => cache.Get(99));
    }

    [Fact]
    public void Get_OnEmptyCache_ThrowsKeyNotFoundException()
    {
        Cache<int, string> cache = CreateCache(3);

        Assert.Throws<KeyNotFoundException>(() => cache.Get(1));
    }

    [Fact]
    public void Get_EvictedKey_ThrowsKeyNotFoundException()
    {
        Cache<int, string> cache = CreateCache(2);
        cache.Add(1, "one");
        cache.Add(2, "two");
        cache.Add(3, "three"); // витісняє 1

        Assert.Throws<KeyNotFoundException>(() => cache.Get(1));
    }

    [Fact]
    public void Get_DoesNotChangeEvictionOrder()
    {
        // Ключова відмінність FIFO від LRU: читання не «освіжає» елемент
        Cache<int, string> cache = CreateCache(2);
        cache.Add(1, "one");
        cache.Add(2, "two");

        cache.Get(1);
        cache.Add(3, "three");

        Assert.False(cache.ContainsKey(1)); // 1 найстарший, хоч його щойно читали
        Assert.True(cache.ContainsKey(2));
        Assert.True(cache.ContainsKey(3));
    }

    // ---------------- ContainsKey ----------------

    [Fact]
    public void ContainsKey_ExistingKey_ReturnsTrue()
    {
        Cache<int, string> cache = CreateCache(3);
        cache.Add(1, "one");

        Assert.True(cache.ContainsKey(1));
    }

    [Fact]
    public void ContainsKey_MissingKey_ReturnsFalse()
    {
        Cache<int, string> cache = CreateCache(3);
        cache.Add(1, "one");

        Assert.False(cache.ContainsKey(2));
    }

    [Fact]
    public void ContainsKey_EvictedKey_ReturnsFalse()
    {
        Cache<int, string> cache = CreateCache(1);
        cache.Add(1, "one");
        cache.Add(2, "two");

        Assert.False(cache.ContainsKey(1));
    }

    // ---------------- Count ----------------

    [Fact]
    public void Count_EmptyCache_ReturnsZero()
    {
        Cache<int, string> cache = CreateCache(3);

        Assert.Equal(0, cache.Count);
    }

    [Fact]
    public void Count_AfterAddingItems_ReturnsNumberOfItems()
    {
        Cache<int, string> cache = CreateCache(5);

        cache.Add(1, "one");
        cache.Add(2, "two");

        Assert.Equal(2, cache.Count);
    }

    [Fact]
    public void Count_NeverExceedsCapacity()
    {
        Cache<int, string> cache = CreateCache(3);

        for (int i = 1; i <= 10; i++)
        {
            cache.Add(i, $"value{i}");
            Assert.True(cache.Count <= 3);
        }

        Assert.Equal(3, cache.Count);
    }

    // ---------------- Політика витіснення FIFO ----------------

    [Fact]
    public void Add_WhenCacheIsFull_RemovesOldestItem()
    {
        Cache<int, string> cache = CreateCache(3);
        cache.Add(1, "one");
        cache.Add(2, "two");
        cache.Add(3, "three");

        cache.Add(4, "four");

        Assert.False(cache.ContainsKey(1));
    }

    [Fact]
    public void Add_WhenCacheIsFull_KeepsNewerItemsAndNewItem()
    {
        Cache<int, string> cache = CreateCache(3);
        cache.Add(1, "one");
        cache.Add(2, "two");
        cache.Add(3, "three");

        cache.Add(4, "four");

        Assert.Equal("two", cache.Get(2));
        Assert.Equal("three", cache.Get(3));
        Assert.Equal("four", cache.Get(4));
    }

    [Fact]
    public void Add_WhenCacheIsFull_CountStaysEqualToCapacity()
    {
        Cache<int, string> cache = CreateCache(3);
        cache.Add(1, "one");
        cache.Add(2, "two");
        cache.Add(3, "three");

        cache.Add(4, "four");

        Assert.Equal(3, cache.Count);
    }

    [Fact]
    public void Add_ManyItems_EvictsInInsertionOrder()
    {
        Cache<int, string> cache = CreateCache(3);

        for (int i = 1; i <= 6; i++)
        {
            cache.Add(i, $"value{i}");
        }

        Assert.False(cache.ContainsKey(1));
        Assert.False(cache.ContainsKey(2));
        Assert.False(cache.ContainsKey(3));
        Assert.Equal("value4", cache.Get(4));
        Assert.Equal("value5", cache.Get(5));
        Assert.Equal("value6", cache.Get(6));
    }

    [Fact]
    public void Add_WithCapacityOne_ReplacesPreviousItem()
    {
        Cache<int, string> cache = CreateCache(1);

        cache.Add(1, "one");
        cache.Add(2, "two");

        Assert.False(cache.ContainsKey(1));
        Assert.Equal("two", cache.Get(2));
        Assert.Equal(1, cache.Count);
    }

    [Fact]
    public void Add_EvictedKeyAddedAgain_IsTreatedAsNewestItem()
    {
        Cache<int, string> cache = CreateCache(2);
        cache.Add(1, "one");
        cache.Add(2, "two");
        cache.Add(3, "three"); // витісняє 1; порядок: 2, 3

        cache.Add(1, "again"); // витісняє 2; порядок: 3, 1

        Assert.False(cache.ContainsKey(2));
        Assert.True(cache.ContainsKey(3));
        Assert.Equal("again", cache.Get(1));
    }

    // ---------------- Узагальненість ----------------

    private record Product(string Name);

    [Fact]
    public void Cache_WorksWithCustomReferenceType()
    {
        Cache<int, Product> cache = new Cache<int, Product>(2);
        Product laptop = new Product("Laptop");

        cache.Add(1, laptop);

        Assert.Same(laptop, cache.Get(1));
    }
}
