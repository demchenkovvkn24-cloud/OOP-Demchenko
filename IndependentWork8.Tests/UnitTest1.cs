using System.Collections.Generic;
using IndependentWork8;
using Xunit;

namespace IndependentWork8.Tests
{
    public class CacheTests
    {
        [Fact]
        public void Add_And_Get_ShouldStoreAndRetrieveValue()
        {
            var cache = new Cache<int, string>(3);
            cache.Add(1, "One");

            Assert.Equal("One", cache.Get(1));
        }

        [Fact]
        public void Count_ShouldReturnCurrentNumberOfItems()
        {
            var cache = new Cache<int, string>(3);
            Assert.Equal(0, cache.Count);

            cache.Add(1, "One");
            cache.Add(2, "Two");
            Assert.Equal(2, cache.Count);
        }

        [Fact]
        public void ContainsKey_ShouldReturnTrueIfExistsAndFalseOtherwise()
        {
            var cache = new Cache<string, int>(2);
            cache.Add("A", 10);

            Assert.True(cache.ContainsKey("A"));
            Assert.False(cache.ContainsKey("B"));
        }

        [Fact]
        public void Add_WhenCapacityExceeded_ShouldEvictOldestElement_FIFO()
        {
            var cache = new Cache<int, string>(2);
            cache.Add(1, "First");
            cache.Add(2, "Second");
            cache.Add(3, "Third");

            Assert.Equal(2, cache.Count);
            Assert.False(cache.ContainsKey(1));
            Assert.True(cache.ContainsKey(2));
            Assert.True(cache.ContainsKey(3));
        }

        [Fact]
        public void Get_NonExistentKey_ShouldThrowKeyNotFoundException()
        {
            var cache = new Cache<int, string>(2);
            cache.Add(1, "First");

            Assert.Throws<KeyNotFoundException>(() => cache.Get(99));
        }
    }
}