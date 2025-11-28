using System;
using System.Collections.Generic;
using System.Linq; // Потрібен для зручних обчислень (Sum, Average)

namespace lab4v11
{
    // --- 1. Інтерфейс (Контракт) ---
    // Кожен товар зобов'язаний мати Ціну та Назву
    public interface IProduct
    {
        string Name { get; }
        double Price { get; }
        string GetInfo();
    }

    // --- 2. Абстрактний клас ---
    // Базова реалізація для уникнення дублювання коду
    public abstract class BaseProduct : IProduct
    {
        public string Name { get; protected set; }
        public double Price { get; protected set; }

        public BaseProduct(string name, double price)
        {
            Name = name;
            Price = price;
        }

        public abstract string GetInfo();
    }

    // --- 3. Реалізація: Продукти харчування ---
    public class Food : BaseProduct
    {
        public double WeightKg { get; set; } // Вага в кг

        public Food(string name, double price, double weight) : base(name, price)
        {
            WeightKg = weight;
        }

        public override string GetInfo()
        {
            return $"🍎 Їжа: {Name} ({WeightKg} кг) - {Price} грн";
        }
    }

    // --- 4. Реалізація: Одяг ---
    public class Clothes : BaseProduct
    {
        public string Size { get; set; } // Розмір (S, M, L...)

        public Clothes(string name, double price, string size) : base(name, price)
        {
            Size = size;
        }

        public override string GetInfo()
        {
            return $"👕 Одяг: {Name} (Розмір: {Size}) - {Price} грн";
        }
    }

    // --- 5. Клас Кошик (Агрегація / Обчислення) ---
    public class ShoppingCart
    {
        // Агрегація: Кошик МІСТИТЬ список IProduct
        private List<IProduct> _items = new List<IProduct>();

        public void AddToCart(IProduct product)
        {
            _items.Add(product);
            Console.WriteLine($"[+] Додано до кошика: {product.Name}");
        }

        public void PrintReceipt()
        {
            Console.WriteLine("\n--- Вміст кошика ---");
            foreach (var item in _items)
            {
                Console.WriteLine(item.GetInfo());
            }
        }

        // Обчислення 1: Сума замовлення
        public double GetTotalSum()
        {
            return _items.Sum(item => item.Price);
        }

        // Обчислення 2: Середня ціна
        public double GetAveragePrice()
        {
            if (_items.Count == 0) return 0;
            return _items.Average(item => item.Price);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Lab 4: Shopping Cart (Variant 11) ===\n");

            // Створення кошика
            ShoppingCart myCart = new ShoppingCart();

            // Створення товарів
            Food apple = new Food("Яблука Голден", 35.50, 1.5);
            Food bread = new Food("Хліб Житній", 24.00, 0.5);
            
            Clothes tShirt = new Clothes("Футболка Nike", 850.00, "M");
            Clothes jeans = new Clothes("Джинси Levi's", 2200.00, "32/34");

            // Додавання товарів (Агрегація)
            myCart.AddToCart(apple);
            myCart.AddToCart(bread);
            myCart.AddToCart(tShirt);
            myCart.AddToCart(jeans);

            // Вивід чеку
            myCart.PrintReceipt();

            // Виконання обчислень
            Console.WriteLine("\n--- Фінансовий звіт ---");
            Console.WriteLine($"Загальна сума замовлення: {myCart.GetTotalSum():F2} грн");
            Console.WriteLine($"Середня ціна товару:      {myCart.GetAveragePrice():F2} грн");
        }
    }
}