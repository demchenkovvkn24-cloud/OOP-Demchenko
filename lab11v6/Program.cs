using System;
using System.Collections.Generic;

abstract class TestRunner
{
    public string SuiteName { get; set; }
    public int TestsPassed { get; protected set; }
    public int TestsFailed { get; protected set; }

    protected TestRunner(string suiteName)
    {
        SuiteName = suiteName;
    }

    public void Run()
    {
        Console.WriteLine($"Запуск: {SuiteName}");
        SetUp();
        ExecuteTests();
        CollectResults();
        TearDown();
        GenerateReport();
        Console.WriteLine();
    }

    protected abstract void SetUp();
    protected abstract void ExecuteTests();

    protected virtual void CollectResults()
    {
        Console.WriteLine($"Результати {SuiteName}: пройдено {TestsPassed}, провалено {TestsFailed}");
    }

    protected virtual void TearDown()
    {
        Console.WriteLine("Очищення ресурсів тестування");
    }

    protected abstract void GenerateReport();
}

class UnitTestRunner : TestRunner
{
    public UnitTestRunner(string suiteName) : base(suiteName) { }

    protected override void SetUp()
    {
        Console.WriteLine("Підготовка модульних тестів (моки та ізольоване середовище)");
    }

    protected override void ExecuteTests()
    {
        Console.WriteLine("Виконання модульних тестів...");
        TestsPassed = 15;
        TestsFailed = 0;
    }

    protected override void GenerateReport()
    {
        Console.WriteLine("Звіт модульних тестів створено");
    }
}

class IntegrationTestRunner : TestRunner
{
    public string DatabaseName { get; set; }

    public IntegrationTestRunner(string suiteName, string databaseName) : base(suiteName)
    {
        DatabaseName = databaseName;
    }

    protected override void SetUp()
    {
        Console.WriteLine($"Підключення до тестової БД: {DatabaseName}");
    }

    protected override void ExecuteTests()
    {
        Console.WriteLine("Виконання інтеграційних тестів...");
        TestsPassed = 8;
        TestsFailed = 1;
    }

    protected override void TearDown()
    {
        Console.WriteLine($"Скидання стану БД {DatabaseName} та закриття з'єднання");
    }

    protected override void GenerateReport()
    {
        Console.WriteLine("Звіт інтеграційних тестів створено");
    }
}

class E2ETestRunner : TestRunner
{
    public string Browser { get; set; }

    public E2ETestRunner(string suiteName, string browser) : base(suiteName)
    {
        Browser = browser;
    }

    protected override void SetUp()
    {
        Console.WriteLine($"Запуск браузера: {Browser}");
    }

    protected override void ExecuteTests()
    {
        Console.WriteLine("Виконання E2E тестів у користувацькому інтерфейсі...");
        TestsPassed = 4;
        TestsFailed = 2;
    }

    protected override void TearDown()
    {
        Console.WriteLine($"Закриття браузера {Browser}");
    }

    protected override void GenerateReport()
    {
        Console.WriteLine("Звіт E2E тестів з скріншотами створено");
    }
}

class Program
{
    static void Main()
    {
        List<TestRunner> runners = new List<TestRunner>
        {
            new UnitTestRunner("Модульні тести сервісу авторизації"),
            new IntegrationTestRunner("Інтеграційні тести API", "TestDb_v1"),
            new E2ETestRunner("E2E тести оформлення замовлення", "Chrome")
        };

        foreach (var runner in runners)
        {
            runner.Run();
        }
    }
}