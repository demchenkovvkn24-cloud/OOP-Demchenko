using System;
using System.Collections.Generic;

namespace IndependentWork8
{
    public class Cache<TKey, TValue> where TKey : notnull
    {
        private readonly int _capacity;
        private readonly Dictionary<TKey, TValue> _cache;
        private readonly Queue<TKey> _keysOrder;

        public int Count => _cache.Count;

        public Cache(int capacity)
        {
            if (capacity <= 0)
                throw new ArgumentOutOfRangeException(nameof(capacity));

            _capacity = capacity;
            _cache = new Dictionary<TKey, TValue>(capacity);
            _keysOrder = new Queue<TKey>(capacity);
        }

        public void Add(TKey key, TValue value)
        {
            if (_cache.ContainsKey(key))
            {
                _cache[key] = value;
                return;
            }

            if (_cache.Count >= _capacity)
            {
                TKey oldestKey = _keysOrder.Dequeue();
                _cache.Remove(oldestKey);
            }

            _cache.Add(key, value);
            _keysOrder.Enqueue(key);
        }

        public TValue Get(TKey key)
        {
            if (_cache.TryGetValue(key, out TValue value))
            {
                return value;
            }

            throw new KeyNotFoundException($"Key '{key}' was not found.");
        }

        public bool ContainsKey(TKey key) => _cache.ContainsKey(key);
    }
}