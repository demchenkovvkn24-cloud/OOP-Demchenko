using System;

namespace lab2v6
{
    public class BankAccount
    {
        private string _owner;
        private string _accountNumber;
        private decimal _balance;

        public string Owner
        {
            get => _owner;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Ім'я власника не може бути порожнім.");
                _owner = value;
            }
        }

        public string AccountNumber
        {
            get => _accountNumber;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Номер рахунку не може бути порожнім.");
                _accountNumber = value;
            }
        }

        public decimal Balance => _balance;

        public BankAccount(string owner, string accountNumber, decimal initialBalance)
        {
            Owner = owner;
            AccountNumber = accountNumber;
            
            if (initialBalance < 0)
                throw new ArgumentException("Початковий баланс не може бути від'ємним.");
            
            _balance = initialBalance;
            Console.WriteLine($"[Конструктор] Створено рахунок {AccountNumber} для {Owner} з балансом {_balance} грн.");
        }

        public BankAccount(string owner, string accountNumber) 
            : this(owner, accountNumber, 0m)
        {
            Console.WriteLine($"[Конструктор з this()] Додаткова ініціалізація для {Owner}.");
        }

        public void Deposit(decimal amount)
        {
            if (amount <= 0)
            {
                Console.WriteLine("Сума поповнення повинна бути більшою за 0.");
                return;
            }
            _balance += amount;
            Console.WriteLine($"Успішно поповнено на {amount} грн. Поточний баланс: {_balance} грн.");
        }

        public void Withdraw(decimal amount)
        {
            if (amount <= 0)
            {
                Console.WriteLine("Сума зняття повинна бути більшою за 0.");
                return;
            }
            if (amount > _balance)
            {
                Console.WriteLine($"Недостатньо коштів! Спроба зняти {amount} грн при балансі {_balance} грн.");
                return;
            }
            _balance -= amount;
            Console.WriteLine($"Успішно знято {amount} грн. Поточний баланс: {_balance} грн.");
        }

        ~BankAccount()
        {
            Console.WriteLine($"[Деструктор] Об'єкт рахунку {AccountNumber} ({Owner}) знищено збирачем сміття.");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== Створення об'єктів ===");
            
            CreateAndUseAccounts();

            Console.WriteLine("\n=== Кінець Main, примусовий запуск Garbage Collector ===");
            
            GC.Collect();
            GC.WaitForPendingFinalizers();

            Console.WriteLine("Програму завершено.");
        }

        static void CreateAndUseAccounts()
        {
            BankAccount acc1 = new BankAccount("Іван Петренко", "UA1234567890", 1500.50m);
            acc1.Deposit(500m);
            acc1.Withdraw(200m);

            Console.WriteLine();

            BankAccount acc2 = new BankAccount("Марія Коваль", "UA0987654321");
            acc2.Deposit(1000m);
            acc2.Withdraw(1500m);
        }
    }
}