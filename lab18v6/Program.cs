using System;
using System.Collections.Generic;

namespace lab18v6
{
    public class ImageProcessingException : Exception
    {
        public string ImageName { get; }
        public string Operation { get; }

        public ImageProcessingException(string imageName, string operation, string message)
            : base(message)
        {
            ImageName = imageName;
            Operation = operation;
        }

        public ImageProcessingException(string imageName, string operation, string message, Exception innerException)
            : base(message, innerException)
        {
            ImageName = imageName;
            Operation = operation;
        }
    }

    public class Result<T>
    {
        public bool IsSuccess { get; }
        public T Value { get; }
        public string Error { get; }

        private Result(T value, bool isSuccess, string error)
        {
            Value = value;
            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result<T> Success(T value) => new(value, true, string.Empty);

        public static Result<T> Failure(string error) => new(default!, false, error);

        public static Result<T> Failure(Exception ex) => new(default!, false, ex.Message);
    }

    public class ImageProcessor
    {
        private static readonly HashSet<string> SupportedFilters = new(StringComparer.OrdinalIgnoreCase)
        {
            "Grayscale", "Sepia", "Blur", "Sharpen"
        };

        public string CurrentImageName { get; set; } = "photo_01.jpg";

        public byte[] ResizeImage(byte[] imageData, int width, int height)
        {
            if (imageData == null || imageData.Length < 4)
            {
                throw new ImageProcessingException(
                    CurrentImageName,
                    nameof(ResizeImage),
                    "Дані зображення пошкоджені або мають некоректний заголовок."
                );
            }

            if (width <= 0 || height <= 0)
            {
                throw new ImageProcessingException(
                    CurrentImageName,
                    nameof(ResizeImage),
                    $"Некоректні цільові розміри: {width}x{height}."
                );
            }

            return new byte[] { 0xFF, 0xD8, (byte)(width % 256), (byte)(height % 256) };
        }

        public Result<bool> ApplyFilter(byte[] imageData, string filterName)
        {
            if (imageData == null || imageData.Length == 0)
            {
                return Result<bool>.Failure("Не передано дані зображення для накладання фільтра.");
            }

            if (string.IsNullOrWhiteSpace(filterName) || !SupportedFilters.Contains(filterName))
            {
                return Result<bool>.Failure($"Невідомий фільтр '{filterName}'. Доступні: {string.Join(", ", SupportedFilters)}.");
            }

            return Result<bool>.Success(true);
        }
    }

    internal class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var processor = new ImageProcessor { CurrentImageName = "avatar.png" };
            byte[] validImage = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A };
            byte[] corruptedImage = { 0x00 };

            Console.WriteLine("=== 1. Зміна розміру зображення (ResizeImage + try-catch) ===");
            try
            {
                var resized = processor.ResizeImage(validImage, 800, 600);
                Console.WriteLine($"Успіх: розмір зображення змінено (байтів: {resized.Length}).");

                processor.ResizeImage(corruptedImage, 800, 600);
            }
            catch (ImageProcessingException ex)
            {
                Console.WriteLine($"[ImageProcessingException] Файл: {ex.ImageName} | Операція: {ex.Operation} | Помилка: {ex.Message}");
            }
            Console.WriteLine();

            Console.WriteLine("=== 2. Застосування фільтрів (ApplyFilter + Result<T>) ===");
            var validFilterResult = processor.ApplyFilter(validImage, "Sepia");
            PrintFilterResult("Sepia", validFilterResult);

            var unknownFilterResult = processor.ApplyFilter(validImage, "NeonGlow");
            PrintFilterResult("NeonGlow", unknownFilterResult);
        }

        static void PrintFilterResult(string filter, Result<bool> result)
        {
            if (result.IsSuccess)
                Console.WriteLine($"Фільтр '{filter}': успішно застосовано (IsSuccess = {result.IsSuccess}).");
            else
                Console.WriteLine($"Фільтр '{filter}': відхилено (IsSuccess = {result.IsSuccess}, Error = \"{result.Error}\").");
        }
    }
}