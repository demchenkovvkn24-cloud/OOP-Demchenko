using System;

public class Car
{
    private string _brand;
    private double _fuel;

    public string Brand
    {
        get { return _brand; }
        set { _brand = value; }
    }

    public double Fuel
    {
        get { return _fuel; }
    }

    public Car(string brand, double fuel)
    {
        _brand = brand;
        _fuel = fuel;
    }

    public bool CanDrive(double distance)
    {
        return _fuel >= distance * 0.1;
    }
}

public class Student
{
    private string _name;
    private double _averageGrade;

    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }

    public Student(string name, double averageGrade)
    {
        _name = name;
        _averageGrade = averageGrade;
    }

    public bool IsExcellent()
    {
        return _averageGrade >= 90;
    }
}

public class Book
{
    private string _title;
    private int _pages;

    public string Title
    {
        get { return _title; }
        set { _title = value; }
    }

    public Book(string title, int pages)
    {
        _title = title;
        _pages = pages;
    }

    public int ReadingTime()
    {
        return _pages * 2;
    }
}

class Program
{
    static void Main(string[] args)
    {
        Car car = new Car("BMW", 20);

        Console.WriteLine("Автомобіль:");
        Console.WriteLine($"Марка: {car.Brand}");
        Console.WriteLine($"Чи вистачить пального на 100 км: {car.CanDrive(100)}");
        Console.WriteLine();

        Student student = new Student("В'ячеслав", 95);

        Console.WriteLine("Студент:");
        Console.WriteLine($"Ім'я: {student.Name}");
        Console.WriteLine($"Відмінник: {student.IsExcellent()}");
        Console.WriteLine();

        Book book = new Book("C# для початківців", 250);

        Console.WriteLine("Книга:");
        Console.WriteLine($"Назва: {book.Title}");
        Console.WriteLine($"Час читання: {book.ReadingTime()} хвилин");
    }
}
