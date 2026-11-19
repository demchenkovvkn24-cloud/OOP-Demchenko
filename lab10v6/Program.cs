using System;
using System.Collections.Generic;

interface IConverter<TInput, TOutput>
{
    TOutput Convert(TInput input);
}

class StringToIntConverter : IConverter<string, int>
{
    public int Convert(string input)
    {
        if (int.TryParse(input, out int result))
        {
            return result;
        }
        throw new FormatException($"Неможливо перетворити {input} у int");
    }
}

class StringToBoolConverter : IConverter<string, bool>
{
    public bool Convert(string input)
    {
        if (bool.TryParse(input, out bool result))
        {
            return result;
        }
        if (input == "1" || input.Equals("yes", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }
        if (input == "0" || input.Equals("no", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }
        throw new FormatException($"Неможливо перетворити {input} у bool");
    }
}

abstract class DataProcessor
{
    protected string rawData = string.Empty;
    protected string transformedData = string.Empty;

    public abstract void LoadData();
    public abstract void TransformData();

    public void SaveProcessedData()
    {
        Console.WriteLine($"Збережено: {transformedData}");
    }

    public void ProcessPipeline()
    {
        LoadData();
        TransformData();
        SaveProcessedData();
    }
}

class CsvProcessor : DataProcessor
{
    private readonly string filePath;

    public CsvProcessor(string filePath)
    {
        this.filePath = filePath;
    }

    public override void LoadData()
    {
        rawData = "Іван, 25, Київ";
        Console.WriteLine($"Зчитано з {filePath}: {rawData}");
    }

    public override void TransformData()
    {
        transformedData = rawData.Replace(",", " ");
        Console.WriteLine("Дані CSV перетворено");
    }
}

class JsonProcessor : DataProcessor
{
    private readonly string filePath;

    public JsonProcessor(string filePath)
    {
        this.filePath = filePath;
    }

    public override void LoadData()
    {
        rawData = "Олексій, 22";
        Console.WriteLine($"Зчитано з {filePath}: {rawData}");
    }

    public override void TransformData()
    {
        transformedData = rawData.ToUpper();
        Console.WriteLine("Дані JSON перетворено");
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("Конвертація");
        
        List<IConverter<string, int>> intConverters = new List<IConverter<string, int>>
        {
            new StringToIntConverter()
        };

        foreach (var converter in intConverters)
        {
            Console.WriteLine(converter.Convert("12345"));
        }

        List<IConverter<string, bool>> boolConverters = new List<IConverter<string, bool>>
        {
            new StringToBoolConverter()
        };

        foreach (var converter in boolConverters)
        {
            Console.WriteLine(converter.Convert("true"));
        }

        Console.WriteLine();
        Console.WriteLine("Обробка даних");

        List<DataProcessor> processors = new List<DataProcessor>
        {
            new CsvProcessor("data.csv"),
            new JsonProcessor("config.json")
        };

        foreach (var processor in processors)
        {
            processor.ProcessPipeline();
            Console.WriteLine();
        }
    }
}