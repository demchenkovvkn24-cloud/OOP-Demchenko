using System;
using System.Collections.Generic;
using System.Linq;

public class IntStack
{
    private List<int> _items = new List<int>();

    public int Count => _items.Count;

    public int this[int index]
    {
        get
        {
            if (index < 0 || index >= _items.Count)
                throw new IndexOutOfRangeException();
            return _items[index];
        }
        set
        {
            if (index < 0 || index >= _items.Count)
                throw new IndexOutOfRangeException();
            _items[index] = value;
        }
    }

    public void Push(int item)
    {
        _items.Add(item);
    }

    public int Pop()
    {
        if (_items.Count == 0)
            throw new InvalidOperationException();

        int lastIndex = _items.Count - 1;
        int item = _items[lastIndex];
        _items.RemoveAt(lastIndex);
        return item;
    }

    public int Peek()
    {
        if (_items.Count == 0)
            throw new InvalidOperationException();

        return _items[_items.Count - 1];
    }

    public static IntStack operator +(IntStack? a, IntStack? b)
    {
        IntStack result = new IntStack();
        if (a != null)
        {
            foreach (var item in a._items)
                result.Push(item);
        }
        if (b != null)
        {
            foreach (var item in b._items)
                result.Push(item);
        }
        return result;
    }

    public static bool operator ==(IntStack? a, IntStack? b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a is null || b is null) return false;
        return a.Equals(b);
    }

    public static bool operator !=(IntStack? a, IntStack? b)
    {
        return !(a == b);
    }

    public override bool Equals(object? obj)
    {
        if (obj is IntStack other)
        {
            return _items.SequenceEqual(other._items);
        }
        return false;
    }

    public override int GetHashCode()
    {
        int hash = 17;
        foreach (var item in _items)
        {
            hash = hash * 31 + item.GetHashCode();
        }
        return hash;
    }

    public override string ToString()
    {
        if (_items.Count == 0)
            return "Empty";

        return $"[{string.Join(", ", _items)}]";
    }
}

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        IntStack stack1 = new IntStack();
        stack1.Push(10);
        stack1.Push(20);
        stack1.Push(30);

        IntStack stack2 = new IntStack();
        stack2.Push(40);
        stack2.Push(50);

        Console.WriteLine("Стек 1:");
        Console.WriteLine(stack1);
        Console.WriteLine();

        Console.WriteLine("Стек 2:");
        Console.WriteLine(stack2);
        Console.WriteLine();

        Console.WriteLine($"Елемент [0]: {stack1[0]}");
        stack1[0] = 15;
        Console.WriteLine($"Після зміни [0]: {stack1}");
        Console.WriteLine();

        IntStack combined = stack1 + stack2;
        Console.WriteLine("Об'єднання:");
        Console.WriteLine(combined);
        Console.WriteLine();

        IntStack stack3 = new IntStack();
        stack3.Push(15);
        stack3.Push(20);
        stack3.Push(30);

        Console.WriteLine("Порівняння:");
        Console.WriteLine($"stack1 == stack3: {stack1 == stack3}");
        Console.WriteLine($"stack1 == stack2: {stack1 == stack2}");
    }
}