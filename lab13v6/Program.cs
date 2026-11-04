using System;
using System.Collections.Generic;

namespace Lab13
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

    public class Car
    {
        public string Model { get; set; }
        public int Year { get; set; }
        public double EngineSize { get; set; }

        public Car(string model, int year, double engineSize)
        {
            Model = model;
            Year = year;
            EngineSize = engineSize;
        }

        public override string ToString() => $"{Model} ({Year} р.) - {EngineSize}L";
    }

    internal class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var numbers = new List<int> { 15, 42, 7, 42, 99, 15, 23, 88, 4 };
            Console.WriteLine("Map (ділення на 2.0): " + string.Join(", ", FunctionalUtils.Map(numbers, x => x / 2.0)));
            Console.WriteLine("Filter (непарні > 20): " + string.Join(", ", FunctionalUtils.Filter(numbers, x => x % 2 != 0 && x > 20)));
            Console.WriteLine("MaxBy: " + FunctionalUtils.MaxBy(numbers, x => x));
            Console.WriteLine("Count (кратні 3): " + FunctionalUtils.Count(numbers, x => x % 3 == 0));
            Console.WriteLine("Distinct: " + string.Join(", ", FunctionalUtils.Distinct(numbers)));

            Console.WriteLine();

            var cities = new List<string> { "Рівне", "Київ", "Львів", "Рівне", "Одеса", "Київ", "Харків" };
            Console.WriteLine("Map (довжини назв): " + string.Join(", ", FunctionalUtils.Map(cities, c => c.Length)));
            Console.WriteLine("Filter (закінчуються на 'в'): " + string.Join(", ", FunctionalUtils.Filter(cities, c => c.EndsWith("в"))));
            Console.WriteLine("MaxBy (найдовша назва): " + FunctionalUtils.MaxBy(cities, c => c.Length));
            Console.WriteLine("Count (назву з 5+ літер): " + FunctionalUtils.Count(cities, c => c.Length >= 5));
            Console.WriteLine("Distinct: " + string.Join(", ", FunctionalUtils.Distinct(cities)));

            Console.WriteLine();

            var garage = new List<Car>
            {
                new Car("BMW X5", 2021, 3.0),
                new Car("Audi A6", 2018, 2.0),
                new Car("Tesla Model 3", 2023, 0.0),
                new Car("Toyota Camry", 2018, 2.5),
                new Car("Porsche 911", 2022, 3.8)
            };

            Console.WriteLine("Моделі: " + string.Join(", ", FunctionalUtils.Map(garage, c => c.Model)));
            Console.WriteLine("Найновіше авто (MaxBy по року): " + FunctionalUtils.MaxBy(garage, c => c.Year));
            Console.WriteLine("Авто від 2020 року (Filter): " + string.Join("; ", FunctionalUtils.Filter(garage, c => c.Year >= 2020)));
            Console.WriteLine("Кількість авто з об'ємом > 2.0L: " + FunctionalUtils.Count(garage, c => c.EngineSize > 2.0));

            var years = FunctionalUtils.Map(garage, c => c.Year);
            Console.WriteLine("Унікальні роки випуску: " + string.Join(", ", FunctionalUtils.Distinct(years)));
        }
    }
}