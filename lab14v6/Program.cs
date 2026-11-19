using System;
using System.Collections.Generic;

namespace lab14v6
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("1. HashSet: Унікальні категорії товарів");
            HashSet<string> categories = new HashSet<string>();

            categories.Add("Електроніка");
            categories.Add("Одяг");
            categories.Add("Побутова техніка");
            bool isAddedDuplicate = categories.Add("Електроніка");

            Console.WriteLine($"Додавання дублікату 'Електроніка': {(isAddedDuplicate ? "Успішно" : "Відхилено (дублікат)")}");
            Console.WriteLine($"Поточна кількість унікальних категорій: {categories.Count}");

            Console.WriteLine("\nПерелік усіх категорій у HashSet:");
            foreach (var category in categories)
            {
                Console.WriteLine($" - {category}");
            }

            string searchCategory = "Одяг";
            Console.WriteLine($"\nПеревірка наявності '{searchCategory}': {categories.Contains(searchCategory)}");

            categories.Remove("Побутова техніка");
            Console.WriteLine("Після видалення 'Побутова техніка':");
            foreach (var category in categories)
            {
                Console.WriteLine($" - {category}");
            }

            Console.WriteLine("\n2. Queue: Черга замовлень на поповнення (FIFO)");
            Queue<string> restockQueue = new Queue<string>();

            restockQueue.Enqueue("Замовлення #101: Ноутбуки (10 шт.)");
            restockQueue.Enqueue("Замовлення #102: Смартфони (25 шт.)");
            restockQueue.Enqueue("Замовлення #103: Монітори (5 шт.)");

            Console.WriteLine($"Кількість замовлень у черзі: {restockQueue.Count}");
            Console.WriteLine($"Наступне замовлення на обробку (Peek): {restockQueue.Peek()}");

            Console.WriteLine("\nПочергова обробка замовлень (Dequeue):");
            while (restockQueue.Count > 0)
            {
                string currentOrder = restockQueue.Dequeue();
                Console.WriteLine($" [Оброблено]: {currentOrder}");
            }

            Console.WriteLine("\n3. Stack: Історія операцій (LIFO / Undo)");
            Stack<string> operationHistory = new Stack<string>();

            operationHistory.Push("Операція 1: Додано 50 шт. товару 'Миша бездротова'");
            operationHistory.Push("Операція 2: Списано 2 шт. товару 'Клавіатура'");
            operationHistory.Push("Операція 3: Переміщено 10 шт. товару 'Монітор' на склад B");

            Console.WriteLine($"Остання виконана операція (Peek): {operationHistory.Peek()}");

            Console.WriteLine("\nВідміна останніх операцій (Pop):");
            while (operationHistory.Count > 0)
            {
                string undoneAction = operationHistory.Pop();
                Console.WriteLine($" [Скасовано/Undo]: {undoneAction}");
            }

            Console.WriteLine("\nРоботу програми завершено успішно");
        }
    }
}