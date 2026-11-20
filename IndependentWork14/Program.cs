using System;
using System.Net.Http;
using System.Threading;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;

namespace IndependentWork14
{
    internal class Program
    {
        private static int _dbAttempts = 0;
        private static int _queueAttempts = 0;

        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            RunDatabaseRetryScenario();
            RunPaymentGatewayCircuitBreakerScenario();
            RunReportGenerationTimeoutFallbackScenario();
        }

        static void RunDatabaseRetryScenario()
        {
            var pipeline = new ResiliencePipelineBuilder<string>()
                .AddRetry(new RetryStrategyOptions<string>
                {
                    ShouldHandle = new PredicateBuilder<string>().Handle<TimeoutException>(),
                    MaxRetryAttempts = 3,
                    Delay = TimeSpan.FromMilliseconds(400),
                    BackoffType = DelayBackoffType.Exponential,
                    UseJitter = true,
                    OnRetry = args =>
                    {
                        Console.WriteLine($"[Retry #{args.AttemptNumber + 1}] Error: {args.Outcome.Exception?.Message}. Delay: {args.RetryDelay.TotalMilliseconds:F0} ms");
                        return default;
                    }
                })
                .Build();

            string ExecuteDbQuery()
            {
                _dbAttempts++;
                if (_dbAttempts <= 2)
                    throw new TimeoutException("Connection pool timeout expired.");
                return "Order #1045 loaded";
            }

            var result = pipeline.Execute(ExecuteDbQuery);
            Console.WriteLine($"Result: {result}\n");
        }

        static void RunPaymentGatewayCircuitBreakerScenario()
        {
            var pipeline = new ResiliencePipelineBuilder()
                .AddCircuitBreaker(new CircuitBreakerStrategyOptions
                {
                    FailureRatio = 0.5,
                    SamplingDuration = TimeSpan.FromSeconds(10),
                    MinimumThroughput = 3,
                    BreakDuration = TimeSpan.FromSeconds(2),
                    ShouldHandle = new PredicateBuilder().Handle<HttpRequestException>(),
                    OnOpened = args =>
                    {
                        Console.WriteLine($"[Circuit OPEN] Break duration: {args.BreakDuration.TotalSeconds} s");
                        return default;
                    },
                    OnClosed = _ =>
                    {
                        Console.WriteLine("[Circuit CLOSED]");
                        return default;
                    }
                })
                .Build();

            for (int i = 1; i <= 5; i++)
            {
                try
                {
                    pipeline.Execute(() =>
                    {
                        _queueAttempts++;
                        if (_queueAttempts <= 3)
                            throw new HttpRequestException("Broker 503 Service Unavailable");
                        Console.WriteLine($"Message #{i} sent");
                    });
                }
                catch (BrokenCircuitException)
                {
                    Console.WriteLine($"Message #{i} rejected (BrokenCircuitException)");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Message #{i} failed: {ex.Message}");
                }

                if (i == 4) Thread.Sleep(2100);
            }
            Console.WriteLine();
        }

        static void RunReportGenerationTimeoutFallbackScenario()
        {
            var pipeline = new ResiliencePipelineBuilder<string>()
                .AddFallback(new Polly.Fallback.FallbackStrategyOptions<string>
                {
                    ShouldHandle = new PredicateBuilder<string>().Handle<TimeoutRejectedException>(),
                    FallbackAction = _ => Outcome.FromResultAsValueTask("Cached report (Fallback)"),
                    OnFallback = _ =>
                    {
                        Console.WriteLine("[Fallback triggered]");
                        return default;
                    }
                })
                .AddTimeout(new TimeoutStrategyOptions
                {
                    Timeout = TimeSpan.FromMilliseconds(500),
                    OnTimeout = args =>
                    {
                        Console.WriteLine($"[Timeout after {args.Timeout.TotalMilliseconds} ms]");
                        return default;
                    }
                })
                .Build();

            string report = pipeline.Execute(ct =>
            {
                for (int i = 0; i < 15; i++)
                {
                    ct.ThrowIfCancellationRequested();
                    Thread.Sleep(100);
                }
                return "Fresh report";
            });

            Console.WriteLine($"Report: {report}");
        }
    }
}