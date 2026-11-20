using IndependentWork7;
using Xunit;

namespace IndependentWork7.Tests;

public class CacheTests
{
    [Fact]
    public void Add_NewItem_ItemIsStored()
    {
        var cache = new Cache<string, int>(2);

        cache.Add("one", 1);

        Assert.True(cache.ContainsKey("one"));
        Assert.Equal(1, cache.Count);
    }

    [Fact]
    public void Add_ExistingKey_UpdatesValueWithoutChangingCount()
    {
        var cache = new Cache<string, int>(2);
        cache.Add("one", 1);

        cache.Add("one", 100);

        Assert.Equal(100, cache.Get("one"));
        Assert.Equal(1, cache.Count);
    }

    [Fact]
    public void Get_ExistingKey_ReturnsValue()
    {
        var cache = new Cache<int, string>(2);
        cache.Add(1, "first");
        cache.Add(2, "second");

        Assert.Equal("first", cache.Get(1));
        Assert.Equal("second", cache.Get(2));
    }

    [Fact]
    public void Get_MissingKey_ThrowsKeyNotFoundException()
    {
        var cache = new Cache<string, string>(2);

        Assert.Throws<KeyNotFoundException>(() => cache.Get("missing"));
    }

    [Fact]
    public void Add_OverCapacity_RemovesOldestItem()
    {
        var cache = new Cache<string, int>(2);
        cache.Add("a", 1);
        cache.Add("b", 2);

        cache.Add("c", 3);

        Assert.False(cache.ContainsKey("a"));
        Assert.True(cache.ContainsKey("b"));
        Assert.True(cache.ContainsKey("c"));
    }

    [Fact]
    public void Add_OverCapacityMultipleTimes_EvictsInFifoOrder()
    {
        var cache = new Cache<int, int>(3);
        for (int i = 1; i <= 5; i++)
        {
            cache.Add(i, i * 10);
        }

        Assert.False(cache.ContainsKey(1));
        Assert.False(cache.ContainsKey(2));
        Assert.True(cache.ContainsKey(3));
        Assert.True(cache.ContainsKey(4));
        Assert.True(cache.ContainsKey(5));
    }

    [Fact]
    public void Get_DoesNotAffectEvictionOrder()
    {
        var cache = new Cache<string, int>(2);
        cache.Add("a", 1);
        cache.Add("b", 2);

        cache.Get("a");
        cache.Add("c", 3);

        Assert.False(cache.ContainsKey("a"));
        Assert.True(cache.ContainsKey("b"));
    }

    [Fact]
    public void ContainsKey_ExistingKey_ReturnsTrue()
    {
        var cache = new Cache<string, int>(2);
        cache.Add("x", 1);

        Assert.True(cache.ContainsKey("x"));
    }

    [Fact]
    public void ContainsKey_MissingKey_ReturnsFalse()
    {
        var cache = new Cache<string, int>(2);

        Assert.False(cache.ContainsKey("x"));
    }

    [Fact]
    public void Count_EmptyCache_IsZero()
    {
        var cache = new Cache<string, int>(3);

        Assert.Equal(0, cache.Count);
    }

    [Fact]
    public void Count_NeverExceedsCapacity()
    {
        var cache = new Cache<int, int>(3);
        for (int i = 0; i < 10; i++)
        {
            cache.Add(i, i);
        }

        Assert.Equal(3, cache.Count);
    }

    [Fact]
    public void Constructor_NonPositiveCapacity_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Cache<int, int>(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new Cache<int, int>(-1));
    }
}