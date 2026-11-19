using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab16V6
{
    public class Artist
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class Song
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int DurationSeconds { get; set; }
        public Artist Artist { get; set; } = null!;
    }

    public class Playlist
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<Song> Songs { get; set; } = new();
    }

    public static class SongExtensions
    {
        public static int TotalDuration(this IEnumerable<Song> songs)
        {
            return songs.Sum(s => s.DurationSeconds);
        }
    }

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var a1 = new Artist { Id = 1, Name = "Океан Ельзи" };
            var a2 = new Artist { Id = 2, Name = "The Hardkiss" };
            var a3 = new Artist { Id = 3, Name = "Бумбокс" };
            var a4 = new Artist { Id = 4, Name = "Imagine Dragons" };
            var a5 = new Artist { Id = 5, Name = "Daft Punk" };
            var a6 = new Artist { Id = 6, Name = "Linkin Park" };

            var playlists = new List<Playlist>
            {
                new Playlist
                {
                    Id = 1,
                    Name = "Український рок та інді",
                    Songs = new List<Song>
                    {
                        new Song { Id = 1, Title = "Обійми", DurationSeconds = 228, Artist = a1 },
                        new Song { Id = 2, Title = "Без бою", DurationSeconds = 259, Artist = a1 },
                        new Song { Id = 3, Title = "Журавлі", DurationSeconds = 172, Artist = a2 },
                        new Song { Id = 4, Title = "Кобра", DurationSeconds = 215, Artist = a2 },
                        new Song { Id = 5, Title = "Вахтерам", DurationSeconds = 235, Artist = a3 }
                    }
                },
                new Playlist
                {
                    Id = 2,
                    Name = "Світові хіти",
                    Songs = new List<Song>
                    {
                        new Song { Id = 6, Title = "Believer", DurationSeconds = 204, Artist = a4 },
                        new Song { Id = 7, Title = "Radioactive", DurationSeconds = 186, Artist = a4 },
                        new Song { Id = 8, Title = "Get Lucky", DurationSeconds = 248, Artist = a5 },
                        new Song { Id = 9, Title = "Instant Crush", DurationSeconds = 337, Artist = a5 },
                        new Song { Id = 10, Title = "Numb", DurationSeconds = 187, Artist = a6 }
                    }
                },
                new Playlist
                {
                    Id = 3,
                    Name = "Для тренувань (Мікс)",
                    Songs = new List<Song>
                    {
                        new Song { Id = 11, Title = "In the End", DurationSeconds = 216, Artist = a6 },
                        new Song { Id = 12, Title = "Thunder", DurationSeconds = 187, Artist = a4 },
                        new Song { Id = 13, Title = "Поліна", DurationSeconds = 242, Artist = a3 },
                        new Song { Id = 14, Title = "Кораблі", DurationSeconds = 210, Artist = a2 },
                        new Song { Id = 15, Title = "911", DurationSeconds = 209, Artist = a1 }
                    }
                }
            };

            Console.WriteLine("=== 1. Унікальні виконавці (SelectMany) ===");
            var uniqueArtists = playlists
                .SelectMany(p => p.Songs)
                .Select(s => s.Artist)
                .DistinctBy(a => a.Id)
                .ToList();

            foreach (var artist in uniqueArtists)
            {
                Console.WriteLine($"- [{artist.Id}] {artist.Name}");
            }

            Console.WriteLine("\n=== 2. Пагінація пісень (по 5 на сторінку) ===");
            var allSongs = playlists.SelectMany(p => p.Songs).ToList();
            int pageSize = 5;
            int totalPages = (int)Math.Ceiling(allSongs.Count / (double)pageSize);

            for (int page = 1; page <= totalPages; page++)
            {
                Console.WriteLine($"\n--- Сторінка {page} із {totalPages} ---");
                var pageSongs = allSongs
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize);

                foreach (var song in pageSongs)
                {
                    Console.WriteLine($"{song.Id}. {song.Artist.Name} — \"{song.Title}\" ({song.DurationSeconds} сек)");
                }
            }

            Console.WriteLine("\n=== 3. Загальна тривалість (Custom Extension) ===");
            foreach (var playlist in playlists)
            {
                int duration = playlist.Songs.TotalDuration();
                TimeSpan ts = TimeSpan.FromSeconds(duration);
                Console.WriteLine($"Плейлист \"{playlist.Name}\": {duration} сек ({ts:mm\\:ss})");
            }

            int totalAllDuration = allSongs.TotalDuration();
            TimeSpan totalTs = TimeSpan.FromSeconds(totalAllDuration);
            Console.WriteLine($"\nЗагальна тривалість усіх пісень: {totalAllDuration} сек ({totalTs:hh\\:mm\\:ss})");
        }
    }
}