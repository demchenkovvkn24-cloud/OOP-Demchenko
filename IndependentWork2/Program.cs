using System;
using System.Globalization;

public class Product
{
    private int _id;
    private string _name;
    private decimal _price;
    private string _category;
    private int _stockCount;

    public int Id => _id;
    public string Name => _name;
    public decimal Price => _price;
    public string Category => _category;
    public int StockCount => _stockCount;

    public Product(int id, string name, decimal price, string category, int stockCount)
    {
        _id = id;
        _name = name;
        _price = price;
        _category = category;
        _stockCount = stockCount;
    }

    public Product(int id, string name, decimal price)
        : this(id, name, price, "Uncategorized", 0)
    {
    }

    public Product(Product other)
        : this(other.Id, other.Name, other.Price, other.Category, other.StockCount)
    {
    }

    public override string ToString()
    {
        return $"ID: {Id}\nНазва: {Name}\nЦіна: {Price:N2} грн\nКатегорія: {Category}\nКількість: {StockCount}";
    }
}

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        CultureInfo.CurrentCulture = new CultureInfo("uk-UA");

        Product product1 = new Product(101, "Laptop", 35000.00m, "Electronics", 5);
        Product product2 = new Product(102, "Mouse", 800.00m);
        Product product3 = new Product(product1);

        Console.WriteLine("Товар 1 (основний конструктор):");
        Console.WriteLine(product1);
        Console.WriteLine();

        Console.WriteLine("Товар 2 (скорочений конструктор):");
        Console.WriteLine(product2);
        Console.WriteLine();

        Console.WriteLine("Товар 3 (конструктор копіювання):");
        Console.WriteLine(product3);
    }
}