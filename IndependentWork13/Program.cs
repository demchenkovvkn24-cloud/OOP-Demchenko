using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace IndependentWork13
{
    internal class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            BenchmarkPerformance();
            DemonstrateSideEffects();
        }

        static double HeavyMath(int x)
        {
            double acc = x;
            for (int i = 1; i <= 40; i++)
            {
                acc = Math.Sqrt(acc * acc + i) * Math.Sin(i) + Math.Cos(acc);
            }
            return acc;
        }

        static void BenchmarkPerformance()
        {
            int[] sizes = { 1_000_000, 5_000_000, 10_000_000 };
            var rnd = new Random(123);

            foreach (int size in sizes)
            {
                var data = Enumerable.Range(0, size).Select(_ => rnd.Next(1, 1000)).ToList();

                var sw = Stopwatch.StartNew();
                var linqRes = data.Where(x => x % 2 == 0).Select(HeavyMath).Count();
                sw.Stop();
                long linqMs = sw.ElapsedMilliseconds;

                sw.Restart();
                var plinqRes = data.AsParallel().Where(x => x % 2 == 0).Select(HeavyMath).Count();
                sw.Stop();
                long plinqMs = sw.ElapsedMilliseconds;

                Console.WriteLine($"Items: {size,10:N0} | LINQ: {linqMs,5} ms | PLINQ: {plinqMs,5} ms | Speedup: {(double)linqMs / Math.Max(1, plinqMs):F2}x");
            }

            var smallData = Enumerable.Range(0, 100_000).ToList();
            var swCheap = Stopwatch.StartNew();
            var c1 = smallData.Select(x => x + 1).ToList();
            swCheap.Stop();
            long cheapLinq = swCheap.ElapsedTicks;

            swCheap.Restart();
            var c2 = smallData.AsParallel().Select(x => x + 1).ToList();
            swCheap.Stop();
            long cheapPlinq = swCheap.ElapsedTicks;
            Console.WriteLine($"Cheap op (100,000 items): LINQ = {cheapLinq} ticks, PLINQ = {cheapPlinq} ticks\n");
        }

        static void DemonstrateSideEffects()
        {
            var numbers = Enumerable.Range(1, 200_000).ToList();

            for (int run = 1; run <= 3; run++)
            {
                int unsafeCounter = 0;
                var _ = numbers.AsParallel().Select(x =>
                {
                    unsafeCounter++;
                    return x;
                }).ToList();

                Console.WriteLine($"Run #{run} (unsafe): expected 200000, got {unsafeCounter}");
            }

            int safeCounter = 0;
            object locker = new();
            var safeBag = new ConcurrentBag<int>();

            var result = numbers.AsParallel().Select(x =>
            {
                lock (locker)
                {
                    safeCounter++;
                }
                if (x % 50_000 == 0) safeBag.Add(x);
                return x;
            }).ToList();

            Console.WriteLine($"Safe (lock): expected 200000, got {safeCounter}");
            Console.WriteLine($"ConcurrentBag count: {safeBag.Count}");
        }
    }
}