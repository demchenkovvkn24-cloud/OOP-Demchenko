using System;
using System.Collections.Generic;

abstract class StorageService
{
    public string ServiceName { get; set; }

    protected StorageService(string serviceName)
    {
        ServiceName = serviceName;
    }

    public abstract void Save(string filename, byte[] data);
}

class LocalStorage : StorageService
{
    public long FreeSpaceBytes { get; set; }

    public LocalStorage(string serviceName, long freeSpaceBytes) : base(serviceName)
    {
        FreeSpaceBytes = freeSpaceBytes;
    }

    public override void Save(string filename, byte[] data)
    {
        if (string.IsNullOrWhiteSpace(filename))
        {
            throw new ArgumentException("Ім'я файлу не може бути порожнім.");
        }

        if (data == null || data.Length == 0)
        {
            throw new ArgumentException("Дані файлу відсутні.");
        }

        if (data.Length > FreeSpaceBytes)
        {
            throw new InvalidOperationException($"Недостатньо місця на локальному диску. Потрібно: {data.Length} B, доступно: {FreeSpaceBytes} B.");
        }

        FreeSpaceBytes -= data.Length;
        Console.WriteLine($"[{ServiceName}] Файл \"{filename}\" успішно збережено локально ({data.Length} B). Залишок місця: {FreeSpaceBytes} B.");
    }
}

class CloudStorage : StorageService
{
    public bool IsConnected { get; set; }

    public CloudStorage(string serviceName, bool isConnected) : base(serviceName)
    {
        IsConnected = isConnected;
    }

    public override void Save(string filename, byte[] data)
    {
        if (string.IsNullOrWhiteSpace(filename))
        {
            throw new ArgumentException("Ім'я файлу не може бути порожнім.");
        }

        if (data == null || data.Length == 0)
        {
            throw new ArgumentException("Дані файлу відсутні.");
        }

        if (!IsConnected)
        {
            throw new Exception($"Мережа недоступна. Не вдалося підключитися до хмарного сховища \"{ServiceName}\".");
        }

        Console.WriteLine($"[{ServiceName}] Файл \"{filename}\" успішно завантажено в хмару ({data.Length} B).");
    }
}

class FTPStorage : StorageService
{
    public string ServerAddress { get; set; }

    public FTPStorage(string serviceName, string serverAddress) : base(serviceName)
    {
        ServerAddress = serverAddress;
    }

    public override void Save(string filename, byte[] data)
    {
        if (string.IsNullOrWhiteSpace(filename))
        {
            throw new ArgumentException("Ім'я файлу не може бути порожнім.");
        }

        if (data == null || data.Length == 0)
        {
            throw new ArgumentException("Дані файлу відсутні.");
        }

        if (string.IsNullOrWhiteSpace(ServerAddress) || !ServerAddress.StartsWith("ftp://"))
        {
            throw new FormatException($"Некоректний FTP-сервер \"{ServerAddress}\". Адреса повинна починатися з 'ftp://'.");
        }

        Console.WriteLine($"[{ServiceName}] Файл \"{filename}\" успішно передано на FTP-сервер {ServerAddress}.");
    }
}

class StorageManager
{
    public void SaveAll(List<StorageService> storages, string filename, byte[] data)
    {
        Console.WriteLine($"=== Початок збереження файлу \"{filename}\" ===");
        
        foreach (var storage in storages)
        {
            try
            {
                storage.Save(filename, data);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ПОМИЛКА] [{storage.ServiceName}]: {ex.Message}");
            }
        }
    }
}

class Program
{
    static void Main()
    {
        List<StorageService> storages = new List<StorageService>
        {
            new LocalStorage("Локальний диск C:", 100),
            new CloudStorage("Google Drive", true),
            new CloudStorage("OneDrive", false),
            new FTPStorage("Корпоративний FTP", "ftp://backup.server.com"),
            new FTPStorage("Некоректний FTP", "http://invalid-ftp.com")
        };

        StorageManager manager = new StorageManager();

        byte[] smallData = new byte[50];
        manager.SaveAll(storages, "document.pdf", smallData);

        Console.WriteLine();

        byte[] largeData = new byte[200];
        manager.SaveAll(storages, "large_video.mp4", largeData);
    }
}