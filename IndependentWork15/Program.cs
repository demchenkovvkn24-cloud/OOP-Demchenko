using System;
using System.Threading;

namespace IndependentWork15
{
    public class RetryPolicy
    {
        private readonly int _maxRetries;
        private readonly TimeSpan _initialDelay;
        private readonly double _jitterFactor;
        private readonly Random _random = new();

        public RetryPolicy(int maxRetries, TimeSpan initialDelay, double jitterFactor = 0.2)
        {
            if (maxRetries < 0)
                throw new ArgumentOutOfRangeException(nameof(maxRetries));
            if (initialDelay < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(initialDelay));
            if (jitterFactor < 0 || jitterFactor > 1)
                throw new ArgumentOutOfRangeException(nameof(jitterFactor));

            _maxRetries = maxRetries;
            _initialDelay = initialDelay;
            _jitterFactor = jitterFactor;
        }

        public T Execute<T>(Func<T> operation, Func<Exception, bool>? shouldRetry = null)
        {
            if (operation is null)
                throw new ArgumentNullException(nameof(operation));

            int retryAttempt = 0;

            while (true)
            {
                try
                {
                    return operation();
                }
                catch (Exception ex)
                {
                    bool canRetry = retryAttempt < _maxRetries && (shouldRetry == null || shouldRetry(ex));
                    if (!canRetry)
                    {
                        Console.WriteLine($"[RetryPolicy] Failed after {retryAttempt} retries: {ex.Message}");
                        throw;
                    }

                    double multiplier = Math.Pow(2, retryAttempt) * (1 + _random.NextDouble() * _jitterFactor * 2 - _jitterFactor);
                    TimeSpan delay = TimeSpan.FromTicks((long)(_initialDelay.Ticks * multiplier));

                    retryAttempt++;
                    Console.WriteLine($"[RetryPolicy] Attempt #{retryAttempt} failed ({ex.GetType().Name}: {ex.Message}). Retrying in {delay.TotalMilliseconds:F0} ms...");
                    Thread.Sleep(delay);
                }
            }
        }
    }

    internal class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var policy = new RetryPolicy(maxRetries: 3, initialDelay: TimeSpan.FromMilliseconds(300), jitterFactor: 0.2);

            int attemptCounter = 0;
            string data = policy.Execute(
                operation: () =>
                {
                    attemptCounter++;
                    if (attemptCounter < 3)
                        throw new TimeoutException("Packet loss");
                    return "Data loaded successfully";
                },
                shouldRetry: ex => ex is TimeoutException
            );
            Console.WriteLine($"Result: {data}\n");

            try
            {
                policy.Execute<int>(() => throw new InvalidOperationException("503 Service Unavailable"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Caught in Main: {ex.GetType().Name} -> {ex.Message}");
            }
        }
    }
}