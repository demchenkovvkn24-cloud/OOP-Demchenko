using System;

class BankAccount
{
    private string owner;
    private string accountNumber;

    public decimal Balance { get; private set; }

    public BankAccount(string owner, string accountNumber, decimal balance)
    {
        this.owner = owner;
        this.accountNumber = accountNumber;
        Balance = balance;

        Console.WriteLine($"Створено рахунок {accountNumber} для {owner}");
    }

    public void Deposit(decimal amount)
    {
        if (amount > 0)
        {
            Balance += amount;
            Console.WriteLine($"{owner}: поповнення на {amount} грн. Баланс = {Balance} грн");
        }
    }

    public void Withdraw(decimal amount)
    {
        if (amount > 0 && amount <= Balance)
        {
            Balance -= amount;
            Console.WriteLine($"{owner}: зняття {amount} грн. Баланс = {Balance} грн");
        }
        else
        {
            Console.WriteLine($"{owner}: недостатньо коштів для зняття {amount} грн");
        }
    }

    public void PrintInfo()
    {
        Console.WriteLine($"Власник: {owner}, Рахунок: {accountNumber}, Баланс: {Balance} грн");
    }

    ~BankAccount()
    {
        Console.WriteLine($"Рахунок {accountNumber} видалено");
    }
}

class Program
{
    static void Main()
    {
        BankAccount account1 = new BankAccount("Іван Петренко", "UA001", 5000);
        BankAccount account2 = new BankAccount("Марія Коваль", "UA002", 3000);
        BankAccount account3 = new BankAccount("Олег Бондар", "UA003", 10000);

        Console.WriteLine("\nІнформація про рахунки:");

        account1.PrintInfo();
        account2.PrintInfo();
        account3.PrintInfo();

        Console.WriteLine("\nОперації:");

        account1.Deposit(1500);
        account2.Withdraw(500);
        account3.Withdraw(12000);

        Console.WriteLine("\nКінцевий стан рахунків:");

        account1.PrintInfo();
        account2.PrintInfo();
        account3.PrintInfo();
    }
}
