using System;

namespace OOP_Demchenko
{
    public class LessonDuration
    {
        private readonly int _totalSeconds;

        public LessonDuration(int hours, int minutes, int seconds)
        {
            if (hours < 0)
                throw new ArgumentException("Години не можуть бути від'ємними.");
            if (minutes is < 0 or > 59)
                throw new ArgumentOutOfRangeException(nameof(minutes), "Хвилини повинні бути в межах 0-59.");
            if (seconds is < 0 or > 59)
                throw new ArgumentOutOfRangeException(nameof(seconds), "Секунди повинні бути в межах 0-59.");

            _totalSeconds = hours * 3600 + minutes * 60 + seconds;
        }

        private LessonDuration(int totalSec)
        {
            _totalSeconds = Math.Max(0, totalSec);
        }

        public int Hours => _totalSeconds / 3600;
        public int Minutes => (_totalSeconds % 3600) / 60;
        public int Seconds => _totalSeconds % 60;

        public static LessonDuration Zero => new LessonDuration(0);

        public int this[int index]
        {
            get
            {
                return index switch
                {
                    0 => Hours,
                    1 => Minutes,
                    2 => Seconds,
                    _ => throw new IndexOutOfRangeException("Індекс має бути від 0 до 2.")
                };
            }
        }

        public static LessonDuration operator +(LessonDuration first, LessonDuration second)
        {
            if (first is null || second is null)
                throw new ArgumentNullException("Об'єкти не можуть бути null");

            return new LessonDuration(first._totalSeconds + second._totalSeconds);
        }

        public static LessonDuration operator -(LessonDuration first, LessonDuration second)
        {
            if (first is null || second is null)
                throw new ArgumentNullException("Об'єкти не можуть бути null");

            int diff = first._totalSeconds - second._totalSeconds;
            return new LessonDuration(Math.Max(0, diff));
        }

        public static bool operator ==(LessonDuration? left, LessonDuration? right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }

        public static bool operator !=(LessonDuration? left, LessonDuration? right) => !(left == right);

        public override bool Equals(object? obj)
        {
            return obj is LessonDuration other && _totalSeconds == other._totalSeconds;
        }

        public override int GetHashCode() => _totalSeconds.GetHashCode();

        public override string ToString()
        {
            return $"{Hours:D2} год {Minutes:D2} хв {Seconds:D2} сек";
        }
    }

    internal class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== Перевірка роботи класу LessonDuration ===");

            var lecture = new LessonDuration(1, 30, 0);
            var breakTime = new LessonDuration(0, 15, 45);

            Console.WriteLine($"Лекція: {lecture}");
            Console.WriteLine($"Перерва: {breakTime}");
            Console.WriteLine($"Нульова тривалість: {LessonDuration.Zero}");

            Console.WriteLine();
            Console.WriteLine("--- Доступ через індексатор ---");
            Console.WriteLine($"Години: {lecture[0]}");
            Console.WriteLine($"Хвилини: {lecture[1]}");
            Console.WriteLine($"Секунди: {lecture[2]}");

            Console.WriteLine();
            Console.WriteLine("--- Арифметика ---");
            var totalTime = lecture + breakTime;
            Console.WriteLine($"Сума: {totalTime}");

            var remaining = lecture - breakTime;
            Console.WriteLine($"Різниця: {remaining}");

            Console.WriteLine();
            Console.WriteLine("--- Порівняння ---");
            var duplicate = new LessonDuration(1, 30, 0);
            Console.WriteLine($"Рівність однакові: {lecture == duplicate}");
            Console.WriteLine($"Рівність різні: {lecture == breakTime}");

            Console.WriteLine();
            Console.WriteLine("--- Перевірка валідації ---");
            try
            {
                var badLesson = new LessonDuration(1, 99, 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Спіймано виняток: {ex.Message}");
            }
        }
    }
}
