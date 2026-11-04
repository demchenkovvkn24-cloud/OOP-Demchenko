using System;
using System.Collections.Generic;

namespace Lab12
{
    public static class FunctionalUtils
    {
        public static List<TResult> Map<T, TResult>(List<T> list, Func<T, TResult> mapper)
        {
            var res = new List<TResult>();
            foreach (var item in list) res.Add(mapper(item));
            return res;
        }

        public static List<T> Filter<T>(List<T> list, Func<T, bool> predicate)
        {
            var res = new List<T>();
            foreach (var item in list) if (predicate(item)) res.Add(item);
            return res;
        }

        public static T MaxBy<T, TKey>(List<T> list, Func<T, TKey> keySelector) where TKey : IComparable<TKey>
        {
            if (list == null || list.Count == 0) throw new InvalidOperationException();

            T maxItem = list[0];
            TKey maxKey = keySelector(maxItem);

            foreach (var item in list)
            {
                TKey key = keySelector(item);
                if (key.CompareTo(maxKey) > 0)
                {
                    maxKey = key;
                    maxItem = item;
                }
            }
            return maxItem;
        }

        public static int Count<T>(List<T> list, Func<T, bool> predicate)
        {
            int count = 0;
            foreach (var item in list) if (predicate(item)) count++;
            return count;
        }

        public static List<T> Distinct<T>(List<T> list)
        {
            var res = new List<T>();
            var set = new HashSet<T>();
            foreach (var item in list) if (set.Add(item)) res.Add(item);
            return res;
        }
    }

    public class Product
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }

        public Product(string name, decimal price, string category)
        {
            Name = name;
            Price = price;
            Category = category;
        }

        public override string ToString() => $"{Name} ({Category}) - ${Price}";
    }

    internal class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // 1. Числа
            var nums = new List<int> { 5, 2, 8, 2, 10, 5, 3, 8, 12, 1 };
            Console.WriteLine("Map (x10): " + string.Join(", ", FunctionalUtils.Map(nums, x => x * 10)));
            Console.WriteLine("Filter (>4): " + string.Join(", ", FunctionalUtils.Filter(nums, x => x > 4)));
            Console.WriteLine("MaxBy: " + FunctionalUtils.MaxBy(nums, x => x));
            Console.WriteLine("Count (парні): " + FunctionalUtils.Count(nums, x => x % 2 == 0));
            Console.WriteLine("Distinct: " + string.Join(", ", FunctionalUtils.Distinct(nums)));

            Console.WriteLine();

            // 2. Рядки
            var words = new List<string> { "Apple", "Banana", "Apple", "Cherry", "Banana", "Date" };
            Console.WriteLine("Map (upper): " + string.Join(", ", FunctionalUtils.Map(words, w => w.ToUpper())));
            Console.WriteLine("Filter (Starts with B): " + string.Join(", ", FunctionalUtils.Filter(words, w => w.StartsWith("B"))));
            Console.WriteLine("MaxBy (len): " + FunctionalUtils.MaxBy(words, w => w.Length));
            Console.WriteLine("Count (len 5): " + FunctionalUtils.Count(words, w => w.Length == 5));
            Console.WriteLine("Distinct: " + string.Join(", ", FunctionalUtils.Distinct(words)));

            Console.WriteLine();

            // 3. Об'єкти
            var items = new List<Product>
            {
                new Product("Ноутбук", 1200m, "Електроніка"),
                new Product("Мишка", 25.5m, "Аксесуари"),
                new Product("Монітор", 350m, "Електроніка"),
                new Product("Клавіатура", 85m, "Аксесуари")
            };

            Console.WriteLine("Назви: " + string.Join(", ", FunctionalUtils.Map(items, p => p.Name)));
            Console.WriteLine("Найдешевший/найдорожчий (MaxBy): " + FunctionalUtils.MaxBy(items, p => p.Price));
            Console.WriteLine("Кількість в категорії Електроніка: " + FunctionalUtils.Count(items, p => p.Category == "Електроніка"));
            
            var categories = FunctionalUtils.Map(items, p => p.Category);
            Console.WriteLine("Категорії: " + string.Join(", ", FunctionalUtils.Distinct(categories)));
        }
    }
}