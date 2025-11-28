using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lab6_Lambda
{
    // Клас Товар
    public class Product
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }

        public override string ToString() => $"{Name} ({Category}) — {Price} грн";
    }

    class Program
    {
        // 1. Оголошення власного делегата
        public delegate double MathOperation(double a, double b);

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("=== Лабораторна робота №6: Делегати та LINQ ===\n");

            Console.WriteLine("--- 1. Демонстрація делегатів та лямбд ---");

            MathOperation oldStyleAdd = delegate (double a, double b)
            {
                return a + b;
            };
            Console.WriteLine($"Анонімний метод (5 + 3): {oldStyleAdd(5, 3)}");

            // Лямбда-вираз (новий синтаксис =>)
            MathOperation lambdaMult = (x, y) => x * y;
            Console.WriteLine($"Лямбда-вираз (5 * 3):    {lambdaMult(5, 3)}");

            // Вбудований делегат Action<T> (нічого не повертає, просто робить дію)
            Action<string> printColor = msg => 
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"[Action]: {msg}");
                Console.ResetColor();
            };
            printColor("Привіт! Це вивід через Action<>");

            // Вбудований делегат Func<T, Result> (приймає int, int -> повертає int)
            Func<int, int, int> addNumbers = (a, b) => a + b;
            Console.WriteLine($"Func (10 + 20): {addNumbers(10, 20)}");

            // Вбудований делегат Predicate<T> (повертає true/false)
            Predicate<int> isPositive = num => num > 0;
            Console.WriteLine($"Predicate (Чи 10 > 0?): {isPositive(10)}");


            // Робота з колекцією Product (Завдання 1)
            Console.WriteLine("\n--- 2. Робота з колекцією товарів (LINQ) ---");

            List<Product> products = new List<Product>
            {
                new Product { Name = "Ноутбук", Price = 25000, Category = "Електроніка" },
                new Product { Name = "Мишка", Price = 500, Category = "Електроніка" },
                new Product { Name = "Хліб", Price = 25, Category = "Продукти" },
                new Product { Name = "Молоко", Price = 35, Category = "Продукти" },
                new Product { Name = "Стіл", Price = 3000, Category = "Меблі" },
                new Product { Name = "Смартфон", Price = 15000, Category = "Електроніка" }
            };

            Console.WriteLine("Усі товари:");
            products.ForEach(p => Console.WriteLine($" - {p}"));

            var expensiveItems = products.Where(p => p.Price > 1000);
            
            printColor("\nТовари дорожчі за 1000 грн:");
            foreach (var item in expensiveItems) Console.WriteLine(item);

            var cheapNames = products
                .Where(p => p.Price < 1000)
                .OrderBy(p => p.Price)
                .Select(p => p.Name);

            Console.WriteLine($"\nНайдешевші товари: {string.Join(", ", cheapNames)}");

            decimal maxPrice = products.Max(p => p.Price);
            
            var mostExpensiveProduct = products.First(p => p.Price == maxPrice);
            
            decimal avgPrice = products.Average(p => p.Price);


            var categories = products.Select(p => p.Category).Distinct();
            string categoryList = categories.Aggregate((current, next) => current + ", " + next);

            Console.WriteLine("\n--- Статистика ---");
            Console.WriteLine($"Найдорожчий товар:  {mostExpensiveProduct.Name} ({maxPrice} грн)");
            Console.WriteLine($"Середня ціна:       {Math.Round(avgPrice, 2)} грн");
            Console.WriteLine($"Категорії товарів:  {categoryList}");

            Console.ReadKey();
        }
    }
}