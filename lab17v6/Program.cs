using System;
using System.IO;
using System.Net.Http;
using System.Threading;

namespace lab17v6
{
    public static class RetryHelper
    {
        public static T ExecuteWithRetry<T>(
            Func<T> operation,
            int retryCount = 3,
            TimeSpan initialDelay = default,
            Func<Exception, bool>? shouldRetry = null)
        {
            if (operation is null) throw new ArgumentNullException(nameof(operation));
            if (initialDelay == default) initialDelay = TimeSpan.FromMilliseconds(200);

            int retryAttempt = 0;

            while (true)
            {
                try
                {
                    return operation();
                }
                catch (Exception ex)
                {
                    bool canRetry = retryAttempt < retryCount && (shouldRetry == null || shouldRetry(ex));

                    if (!canRetry)
                    {
                        Console.WriteLine($"[RetryHelper] Остаточна невдача після {retryAttempt} повторів. Причина: {ex.GetType().Name} — {ex.Message}");
                        throw;
                    }

                    TimeSpan delay = TimeSpan.FromMilliseconds(initialDelay.TotalMilliseconds * Math.Pow(2, retryAttempt));
                    retryAttempt++;

                    Console.WriteLine($"[RetryHelper] Спроба #{retryAttempt} не вдалася ({ex.GetType().Name}: {ex.Message}). Повтор через {delay.TotalMilliseconds} мс...");
                    Thread.Sleep(delay);
                }
            }
        }
    }

    public class FileProcessor
    {
        private int _attempt = 0;

        public string GetLogContent(string path)
        {
            _attempt++;
            if (_attempt <= 2)
            {
                throw new FileNotFoundException($"Файл логів '{path}' тимчасово недоступний або ще формується.", path);
            }
            return $"[2026-11-20 22:15:00] INFO: Application started successfully (from {path})";
        }

        public void Reset() => _attempt = 0;
    }

    public class NetworkClient
    {
        private int _attempt = 0;

        public void SendLogToServer(string url, string logContent)
        {
            _attempt++;
            if (_attempt <= 4)
            {
                throw new HttpRequestException($"Помилка з'єднання із сервером '{url}' (503 Service Unavailable).");
            }
            Console.WriteLine($"[NetworkClient] Лог успішно доставлено на {url}: \"{logContent}\"");
        }

        public void Reset() => _attempt = 0;
    }

    internal class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var fileProcessor = new FileProcessor();
            var networkClient = new NetworkClient();

            Func<Exception, bool> retryPredicate = ex =>
                ex is FileNotFoundException || ex is HttpRequestException;

            Console.WriteLine("=== Сценарій 1: Читання файлу логів (FileProcessor) ===");
            string logContent = RetryHelper.ExecuteWithRetry(
                operation: () => fileProcessor.GetLogContent("logs/app.log"),
                retryCount: 3,
                initialDelay: TimeSpan.FromMilliseconds(200),
                shouldRetry: retryPredicate
            );
            Console.WriteLine($"Результат читання: {logContent}\n");

            Console.WriteLine("=== Сценарій 2: Відправка логів на сервер (NetworkClient) ===");
            RetryHelper.ExecuteWithRetry(
                operation: () =>
                {
                    networkClient.SendLogToServer("https://logs.example.com/api/ingest", logContent);
                    return true;
                },
                retryCount: 5,
                initialDelay: TimeSpan.FromMilliseconds(150),
                shouldRetry: retryPredicate
            );
            Console.WriteLine();

            Console.WriteLine("=== Сценарій 3: Вибіркова фільтрація винятків (shouldRetry) ===");
            try
            {
                RetryHelper.ExecuteWithRetry<string>(
                    operation: () => throw new UnauthorizedAccessException("Доступ до системного журналу заборонено."),
                    retryCount: 3,
                    initialDelay: TimeSpan.FromMilliseconds(200),
                    shouldRetry: retryPredicate
                );
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Операцію перервано без повторів: {ex.Message}");
            }
        }
    }
}