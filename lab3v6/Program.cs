using System;

namespace lab3v6
{
    public class AudioPlayer : IDisposable
    {
        private string _trackName;
        private bool _isPlaying;
        private bool _disposed = false;

        public string TrackName => _trackName;
        public bool IsPlaying => _isPlaying;

        public AudioPlayer(string trackName)
        {
            _trackName = trackName;
            _isPlaying = true;
            Console.WriteLine($"[Конструктор] Завантажено трек: '{_trackName}'. Аудіо ресурс виділено.");
        }

        public void Play()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(AudioPlayer), "Неможливо відтворити: ресурс вже звільнено!");

            _isPlaying = true;
            Console.WriteLine($"[Play] Відтворення: '{_trackName}'...");
        }

        public void Stop()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(AudioPlayer), "Неможливо зупинити: ресурс вже звільнено!");

            _isPlaying = false;
            Console.WriteLine($"[Stop] Зупинено: '{_trackName}'.");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Console.WriteLine($"[Dispose(true)] Звільнення керованих ресурсів для '{_trackName}'.");
                }

                if (_isPlaying)
                {
                    _isPlaying = false;
                }
                Console.WriteLine($"[Dispose] Звільнено аудіо ресурс для '{_trackName}'.");

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~AudioPlayer()
        {
            Console.WriteLine($"[~AudioPlayer] Деструктор викликано для '{_trackName}'.");
            Dispose(false);
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== СЦЕНАРІЙ 1: Використання конструкції using ===");
            using (var player1 = new AudioPlayer("Song_01.mp3"))
            {
                player1.Play();
                player1.Stop();
            }
            Console.WriteLine();

            Console.WriteLine("=== СЦЕНАРІЙ 2: Явний виклик Dispose() без using ===");
            var player2 = new AudioPlayer("Song_02.mp3");
            player2.Play();
            player2.Dispose();
            Console.WriteLine();

            Console.WriteLine("=== СЦЕНАРІЙ 3: Без Dispose() — робота деструктора (GC.Collect) ===");
            CreateUnusedPlayer();

            Console.WriteLine("Викликаємо GC.Collect() та чекаємо фіналізації...");
            GC.Collect();
            GC.WaitForPendingFinalizers();

            Console.WriteLine("\nПрограму завершено.");
        }

        static void CreateUnusedPlayer()
        {
            var player3 = new AudioPlayer("Song_03.mp3");
            player3.Play();
        }
    }
}