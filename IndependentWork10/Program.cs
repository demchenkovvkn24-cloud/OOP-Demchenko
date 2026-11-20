using System;
using System.Collections.Generic;

namespace IndependentWork10
{
    public static class CollectionUtils
    {
        public static IEnumerable<TResult> Map<TSource, TResult>(
            IEnumerable<TSource> collection,
            Func<TSource, TResult> mapper)
        {
            if (collection is null) throw new ArgumentNullException(nameof(collection));
            if (mapper is null) throw new ArgumentNullException(nameof(mapper));

            var result = new List<TResult>();
            foreach (var item in collection)
            {
                result.Add(mapper(item));
            }
            return result;
        }

        public static T MaxBy<T, TKey>(
            IEnumerable<T> collection,
            Func<T, TKey> keySelector)
            where TKey : IComparable<TKey>
        {
            if (collection is null) throw new ArgumentNullException(nameof(collection));
            if (keySelector is null) throw new ArgumentNullException(nameof(keySelector));

            T maxItem = default!;
            TKey maxKey = default!;
            bool first = true;

            foreach (var item in collection)
            {
                TKey key = keySelector(item);
                if (first || key.CompareTo(maxKey) > 0)
                {
                    maxKey = key;
                    maxItem = item;
                    first = false;
                }
            }

            if (first)
                throw new InvalidOperationException("Collection is empty.");

            return maxItem;
        }

        public static IEnumerable<T> Filter<T>(
            IEnumerable<T> collection,
            Predicate<T> predicate)
        {
            if (collection is null) throw new ArgumentNullException(nameof(collection));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));

            var result = new List<T>();
            foreach (var item in collection)
            {
                if (predicate(item))
                    result.Add(item);
            }
            return result;
        }

        public static void ForEach<T>(
            IEnumerable<T> collection,
            Action<T> action)
        {
            if (collection is null) throw new ArgumentNullException(nameof(collection));
            if (action is null) throw new ArgumentNullException(nameof(action));

            foreach (var item in collection)
            {
                action(item);
            }
        }
    }

    public class Product
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }

    internal class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var products = new List<Product>
            {
                new Product { Name = "Laptop", Price = 25000 },
                new Product { Name = "Mouse", Price = 500 },
                new Product { Name = "Keyboard", Price = 1500 }
            };

            var names = CollectionUtils.Map(products, p => p.Name);
            Console.WriteLine("Names: " + string.Join(", ", names));

            var mostExpensive = CollectionUtils.MaxBy(products, p => p.Price);
            Console.WriteLine($"Most expensive: {mostExpensive.Name} - {mostExpensive.Price}");

            var numbers = new List<int> { 3, 14, 7, 22, 9, 40 };
            var evenNumbers = CollectionUtils.Filter(numbers, n => n % 2 == 0);
            var squares = CollectionUtils.Map(evenNumbers, n => n * n);
            CollectionUtils.ForEach(squares, sq => Console.Write($"{sq} "));
            Console.WriteLine();

            var cities = new List<string> { "Рівне", "Київ", "Львів", "Івано-Франківськ" };
            var longestCity = CollectionUtils.MaxBy(cities, c => c.Length);
            var longCities = CollectionUtils.Filter(cities, c => c.Length > 5);
            Console.WriteLine($"Longest city: {longestCity}");
            Console.WriteLine("Cities > 5 chars: " + string.Join(", ", longCities));
        }
    }
}