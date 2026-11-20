using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace IndependentWork12
{
    public record Product(int Id, string Name, string Category, decimal UnitPrice);
    public record Customer(int Id, string Name, string City);
    public record Sale(int Id, int ProductId, int Quantity, DateTime SaleDate, int CustomerId);

    internal class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var (products, customers, sales) = GenerateData(seed: 42);

            TopProductsByCategory(products, sales);
            TotalRevenueByCity(products, customers, sales);
            SalesByYearAndMonth(products, sales);
            TopCustomersInElectronics(products, customers, sales);
        }

        static (List<Product>, List<Customer>, List<Sale>) GenerateData(int seed)
        {
            var rnd = new Random(seed);
            var categories = new[] { "Електроніка", "Побутова техніка", "Аксесуари", "Офіс", "Аудіо" };
            var cities = new[] { "Рівне", "Київ", "Львів", "Дніпро", "Одеса", "Харків" };

            var products = Enumerable.Range(1, 60)
                .Select(i => new Product(
                    Id: i,
                    Name: $"Товар-{i:D2}",
                    Category: categories[rnd.Next(categories.Length)],
                    UnitPrice: rnd.Next(200, 15000)))
                .ToList();

            var customers = Enumerable.Range(1, 80)
                .Select(i => new Customer(
                    Id: i,
                    Name: $"Клієнт_{i:D2}",
                    City: cities[rnd.Next(cities.Length)]))
                .ToList();

            var startDate = new DateTime(2025, 1, 1);
            var sales = Enumerable.Range(1, 500)
                .Select(i => new Sale(
                    Id: i,
                    ProductId: rnd.Next(1, products.Count + 1),
                    Quantity: rnd.Next(1, 10),
                    SaleDate: startDate.AddDays(rnd.Next(0, 650)),
                    CustomerId: rnd.Next(1, customers.Count + 1)))
                .ToList();

            return (products, customers, sales);
        }

        static void TopProductsByCategory(List<Product> products, List<Sale> sales)
        {
            var sw = Stopwatch.StartNew();

            var query = sales
                .GroupBy(s => s.ProductId)
                .Select(g => new { ProductId = g.Key, TotalQty = g.Sum(x => x.Quantity) })
                .Join(products,
                      stat => stat.ProductId,
                      prod => prod.Id,
                      (stat, prod) => new { prod.Category, prod.Name, stat.TotalQty })
                .GroupBy(x => x.Category)
                .Select(catGroup => new
                {
                    Category = catGroup.Key,
                    Top3 = catGroup.OrderByDescending(x => x.TotalQty).Take(3).ToList()
                })
                .ToList();

            sw.Stop();
            Console.WriteLine($"Pipeline 1 ({sw.Elapsed.TotalMilliseconds:F3} ms):");
            foreach (var cat in query)
            {
                Console.WriteLine($"Category: {cat.Category}");
                foreach (var item in cat.Top3)
                    Console.WriteLine($"  - {item.Name}: {item.TotalQty}");
            }
            Console.WriteLine();
        }

        static void TotalRevenueByCity(List<Product> products, List<Customer> customers, List<Sale> sales)
        {
            var sw = Stopwatch.StartNew();

            var query = sales
                .Join(products, s => s.ProductId, p => p.Id, (s, p) => new { s.CustomerId, Revenue = s.Quantity * p.UnitPrice })
                .Join(customers, sp => sp.CustomerId, c => c.Id, (sp, c) => new { c.City, sp.Revenue })
                .GroupBy(x => x.City)
                .Select(g => new { City = g.Key, TotalRevenue = g.Sum(x => x.Revenue) })
                .OrderByDescending(x => x.TotalRevenue)
                .ToList();

            sw.Stop();
            Console.WriteLine($"Pipeline 2 ({sw.Elapsed.TotalMilliseconds:F3} ms):");
            foreach (var row in query)
                Console.WriteLine($"{row.City,-10}: {row.TotalRevenue:N2}");
            Console.WriteLine();
        }

        static void SalesByYearAndMonth(List<Product> products, List<Sale> sales)
        {
            var sw = Stopwatch.StartNew();

            var query = sales
                .Join(products, s => s.ProductId, p => p.Id, (s, p) => new { s.SaleDate, s.Quantity, Revenue = s.Quantity * p.UnitPrice })
                .GroupBy(x => new { x.SaleDate.Year, x.SaleDate.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    TotalUnits = g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x => x.Revenue)
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .Take(6)
                .ToList();

            sw.Stop();
            Console.WriteLine($"Pipeline 3 ({sw.Elapsed.TotalMilliseconds:F3} ms):");
            foreach (var row in query)
                Console.WriteLine($"{row.Year}-{row.Month:D2} | Units: {row.TotalUnits,-4} | Revenue: {row.TotalRevenue:N2}");
            Console.WriteLine();
        }

        static void TopCustomersInElectronics(List<Product> products, List<Customer> customers, List<Sale> sales)
        {
            var sw = Stopwatch.StartNew();

            var electronicsIds = products
                .Where(p => p.Category == "Електроніка")
                .ToDictionary(p => p.Id, p => p.UnitPrice);

            var query = sales
                .Where(s => electronicsIds.ContainsKey(s.ProductId))
                .Select(s => new { s.CustomerId, Spent = s.Quantity * electronicsIds[s.ProductId] })
                .GroupBy(x => x.CustomerId)
                .Select(g => new { CustomerId = g.Key, TotalSpent = g.Sum(x => x.Spent), OrdersCount = g.Count() })
                .Join(customers, g => g.CustomerId, c => c.Id, (g, c) => new { c.Name, c.City, g.TotalSpent, g.OrdersCount })
                .OrderByDescending(x => x.TotalSpent)
                .Take(5)
                .ToList();

            sw.Stop();
            Console.WriteLine($"Pipeline 4 ({sw.Elapsed.TotalMilliseconds:F3} ms):");
            foreach (var c in query)
                Console.WriteLine($"{c.Name} ({c.City}): {c.TotalSpent:N2} (orders: {c.OrdersCount})");
        }
    }
}