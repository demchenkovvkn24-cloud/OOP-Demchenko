using System;
using System.Collections.Generic;

class Dish
{
    public string Name { get; set; }

    public Dish(string name)
    {
        Name = name;
    }

    public virtual void Prepare()
    {
        Console.WriteLine($"Готується страва: {Name}");
    }
}

class Pizza : Dish
{
    public string Toppings { get; set; }

    public Pizza(string name, string toppings) : base(name)
    {
        Toppings = toppings;
    }

    public override void Prepare()
    {
        Console.WriteLine($"Готується піца \"{Name}\" з начинкою: {Toppings}");
    }
}

class Soup : Dish
{
    public string BrothType { get; set; }

    public Soup(string name, string brothType) : base(name)
    {
        BrothType = brothType;
    }

    public override void Prepare()
    {
        Console.WriteLine($"Готується суп \"{Name}\" на бульйоні: {BrothType}");
    }
}

class Salad : Dish
{
    public string Dressing { get; set; }

    public Salad(string name, string dressing) : base(name)
    {
        Dressing = dressing;
    }

    public override void Prepare()
    {
        Console.WriteLine($"Готується салат \"{Name}\" із заправкою: {Dressing}");
    }
}

class Program
{
    static void Main()
    {
        List<Dish> dishes = new List<Dish>
        {
            new Pizza("Пепероні", "Сир моцарела, пепероні, томатний соус"),
            new Soup("Борщ", "Яловичий"),
            new Salad("Цезар", "Майонезно-часниковий соус Цезар"),
            new Pizza("Чотири сири", "Моцарела, ґорґонцола, пармезан, ементаль"),
            new Salad("Грецький", "Оливкова олія")
        };

        foreach (var dish in dishes)
        {
            dish.Prepare();
        }

        Console.WriteLine("\n=== Список всіх приготованих страв ===");
        List<string> preparedDishes = new List<string>();
        foreach (var dish in dishes)
        {
            preparedDishes.Add(dish.Name);
        }

        foreach (var name in preparedDishes)
        {
            Console.WriteLine($"- {name}");
        }
    }
}