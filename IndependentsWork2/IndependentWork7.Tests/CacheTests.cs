using IndependentWork7;
using Xunit;

namespace IndependentWork7.Tests;

public class CacheTests
{
    // ---------- Конструктор ----------

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Constructor_NonPositiveCapacity_Throws(int capacity)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Cache<string, int>(capacity));
    }

    // ---------- Add ----------

    [Fact]
    public void Add_NewItem_IsStored()
    {
        var cache = new Cache<string, int>(3);

        cache.Add("a", 1);

        Assert.True(cache.ContainsKey("a"));
        Assert.Equal(1, cache.Get("a"));
    }

    [Fact]
    public void Add_MultipleItemsWithinCapacity_KeepsAll()
    {
        var cache = new Cache<string, int>(3);

        cache.Add("a", 1);
        cache.Add("b", 2);
        cache.Add("c", 3);

        Assert.Equal(1, cache.Get("a"));
        Assert.Equal(2, cache.Get("b"));
        Assert.Equal(3, cache.Get("c"));
    }

    [Fact]
    public void Add_ExistingKey_UpdatesValueWithoutChangingCount()
    {
        var cache = new Cache<string, int>(2);
        cache.Add("a", 1);

        cache.Add("a", 100);

        Assert.Equal(1, cache.Count);
        Assert.Equal(100, cache.Get("a"));
    }

    [Fact]
    public void Add_ExistingKeyWhenFull_DoesNotEvictAnything()
    {
        var cache = new Cache<string, int>(2);
        cache.Add("a", 1);
        cache.Add("b", 2);

        cache.Add("a", 10);

        Assert.Equal(2, cache.Count);
        Assert.True(cache.ContainsKey("a"));
        Assert.True(cache.ContainsKey("b"));
    }

    [Fact]
    public void Add_NullKey_Throws()
    {
        var cache = new Cache<string, int>(2);

        Assert.Throws<ArgumentNullException>(() => cache.Add(null!, 1));
    }

    // ---------- Get ----------

    [Fact]
    public void Get_ExistingKey_ReturnsValue()
    {
        var cache = new Cache<int, string>(2);
        cache.Add(7, "seven");

        Assert.Equal("seven", cache.Get(7));
    }

    [Fact]
    public void Get_MissingKey_ThrowsKeyNotFoundException()
    {
        var cache = new Cache<string, int>(2);

        Assert.Throws<KeyNotFoundException>(() => cache.Get("missing"));
    }

    // ---------- Переповнення (FIFO) ----------

    [Fact]
    public void Add_WhenFull_EvictsOldestItem()
    {
        var cache = new Cache<string, int>(3);
        cache.Add("a", 1);
        cache.Add("b", 2);
        cache.Add("c", 3);

        cache.Add("d", 4);

        Assert.False(cache.ContainsKey("a")); // найстаріший витіснено
        Assert.True(cache.ContainsKey("b"));
        Assert.True(cache.ContainsKey("c"));
        Assert.True(cache.ContainsKey("d"));
    }

    [Fact]
    public void Add_WhenFull_EvictedKeyThrowsOnGet()
    {
        var cache = new Cache<string, int>(1);
        cache.Add("a", 1);

        cache.Add("b", 2);

        Assert.Throws<KeyNotFoundException>(() => cache.Get("a"));
        Assert.Equal(2, cache.Get("b"));
    }

    [Fact]
    public void Add_ManyItems_EvictsInInsertionOrder()
    {
        var cache = new Cache<int, string>(2);

        cache.Add(1, "one");
        cache.Add(2, "two");
        cache.Add(3, "three"); // витісняє 1
        cache.Add(4, "four");  // витісняє 2

        Assert.False(cache.ContainsKey(1));
        Assert.False(cache.ContainsKey(2));
        Assert.True(cache.ContainsKey(3));
        Assert.True(cache.ContainsKey(4));
    }

    [Fact]
    public void Get_DoesNotAffectEvictionOrder_FifoNotLru()
    {
        var cache = new Cache<string, int>(2);
        cache.Add("a", 1);
        cache.Add("b", 2);

        _ = cache.Get("a"); // у FIFO читання не «омолоджує» елемент
        cache.Add("c", 3);

        Assert.False(cache.ContainsKey("a"));
        Assert.True(cache.ContainsKey("b"));
        Assert.True(cache.ContainsKey("c"));
    }

    [Fact]
    public void Add_UpdateExistingKey_DoesNotChangeEvictionOrder()
    {
        var cache = new Cache<string, int>(2);
        cache.Add("a", 1);
        cache.Add("b", 2);

        cache.Add("a", 10); // оновлення значення, але "a" лишається найстарішим
        cache.Add("c", 3);

        Assert.False(cache.ContainsKey("a"));
        Assert.True(cache.ContainsKey("b"));
        Assert.True(cache.ContainsKey("c"));
    }

    // ---------- ContainsKey ----------

    [Fact]
    public void ContainsKey_ExistingKey_ReturnsTrue()
    {
        var cache = new Cache<string, int>(2);
        cache.Add("a", 1);

        Assert.True(cache.ContainsKey("a"));
    }

    [Fact]
    public void ContainsKey_MissingKey_ReturnsFalse()
    {
        var cache = new Cache<string, int>(2);

        Assert.False(cache.ContainsKey("a"));
    }

    [Fact]
    public void ContainsKey_AfterEviction_ReturnsFalse()
    {
        var cache = new Cache<string, int>(1);
        cache.Add("a", 1);
        cache.Add("b", 2);

        Assert.False(cache.ContainsKey("a"));
    }

    // ---------- Count ----------

    [Fact]
    public void Count_NewCache_IsZero()
    {
        var cache = new Cache<string, int>(3);

        Assert.Equal(0, cache.Count);
    }

    [Fact]
    public void Count_IncreasesWithEachNewKey()
    {
        var cache = new Cache<string, int>(5);

        cache.Add("a", 1);
        Assert.Equal(1, cache.Count);

        cache.Add("b", 2);
        Assert.Equal(2, cache.Count);
    }

    [Fact]
    public void Count_NeverExceedsCapacity()
    {
        var cache = new Cache<int, int>(3);

        for (int i = 0; i < 10; i++)
        {
            cache.Add(i, i * i);
        }

        Assert.Equal(3, cache.Count);
    }
}
