using System;
using System.ComponentModel.DataAnnotations;

public class UserProfile
{
    private string _username = string.Empty;
    private int _age;

    public string Username
    {
        get => _username;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Ім'я користувача не може бути порожнім!");
            }
            _username = value;
        }
    }

    public int Age
    {
        get => _age;
        set
        {
            if (value < 0 || value > 120)
            {
                throw new ArgumentOutOfRangeException("Вік повинен бути від 0 до 120 років!");
            }
            _age = value;
        }
    }

    public string Email { get; set; } = string.Empty;

    public UserProfile(string username, int age, string email)
    {
        Username = username;
        Age = age;
        Email = email;
    }

    public override string ToString()
    {
        return $"Користувач: {Username}\nВік: {Age}\nEmail: {Email}";
    }
}

public class OrderItem
{
    [Range(1, 1000, ErrorMessage = "Кількість товару має бути від 1 до 1000!")]
    public int Quantity { get; set; }

    [Range(0.01, 1000000.0, ErrorMessage = "Ціна повинна бути більшою за 0!")]
    public decimal Price { get; set; }

    public decimal TotalPrice => Quantity * Price;

    public OrderItem(int quantity, decimal price)
    {
        Quantity = quantity;
        Price = price;
    }

    public override string ToString()
    {
        return $"Кількість: {Quantity}\nЦіна за одиницю: {Price:N2} грн\nЗагальна вартість: {TotalPrice:N2} грн";
    }
}

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        UserProfile user = new UserProfile("slavik_sdk", 17, "slavik@example.com");
        Console.WriteLine("Профіль користувача:");
        Console.WriteLine(user);
        Console.WriteLine();

        try
        {
            user.Age = -5;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка валідації: {ex.Message}");
            Console.WriteLine();
        }

        OrderItem item = new OrderItem(3, 1250.50m);
        Console.WriteLine("Позиція замовлення:");
        Console.WriteLine(item);
    }
}