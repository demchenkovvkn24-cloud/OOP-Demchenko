using System;

namespace IndependentWork8
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var cache = new Cache<string, string>(3);

            cache.Add("user_1", "Олександр");
            cache.Add("user_2", "Марія");
            cache.Add("user_3", "Іван");

            Console.WriteLine($"Count: {cache.Count}");
            Console.WriteLine($"user_1: {cache.Get("user_1")}");

            cache.Add("user_4", "Олена");

            Console.WriteLine($"Count after overflow: {cache.Count}");
            Console.WriteLine($"Contains user_1: {cache.ContainsKey("user_1")}");
            Console.WriteLine($"Contains user_4: {cache.ContainsKey("user_4")}");
        }
    }
}