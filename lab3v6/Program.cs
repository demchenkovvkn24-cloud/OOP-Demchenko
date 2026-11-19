using System;

namespace Lab3v6
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
            _trackName = string.IsNullOrWhiteSpace(trackName) ? "Unknown Track" : trackName;
            _isPlaying = false;
            Console.WriteLine($"[AudioPlayer]: Відкрито аудіо ресурс для треку \"{_trackName}\".");
        }

        public void Play()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(AudioPlayer), "Неможливо відтворити: аудіо ресурс уже звільнено!");
            }

            _isPlaying = true;
            Console.WriteLine($"[AudioPlayer]: Відтворення треку \"{_trackName}\".");
        }

        public void Stop()
        {
            if (_disposed) return;

            _isPlaying = false;
            Console.WriteLine($"[AudioPlayer]: Зупинено відтворення треку \"{_trackName}\".");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Console.WriteLine($"[Dispose(true)]: Звільнення керованих ресурсів для \"{_trackName}\".");
                }

                if (_isPlaying)
                {
                    Stop();
                }

                Console.WriteLine($"[Dispose]: Звільнено аудіо ресурс треку \"{_trackName}\".");
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
            Console.WriteLine($"[Деструктор]: Виклик деструктора для треку \"{_trackName}\".");
            Dispose(false);
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== Сценарій 1: Використання оператора using ===");
            using (AudioPlayer player1 = new AudioPlayer("Song_1.mp3"))
            {
                player1.Play();
            }
            Console.WriteLine();

            Console.WriteLine("=== Сценарій 2: Явний виклик Dispose() ===");
            AudioPlayer player2 = new AudioPlayer("Song_2.mp3");
            player2.Play();
            player2.Dispose();
            Console.WriteLine();

            Console.WriteLine("=== Сценарій 3: Без Dispose() (робота GC та деструктора) ===");
            CreateAndForgetObject();

            Console.WriteLine("Примусовий запуск збирача сміття (GC)...");
            GC.Collect();
            GC.WaitForPendingFinalizers();

            Console.WriteLine("\nРоботу програми завершено.");
        }

        static void CreateAndForgetObject()
        {
            AudioPlayer player3 = new AudioPlayer("Song_3.mp3");
            player3.Play();
        }
    }
}
