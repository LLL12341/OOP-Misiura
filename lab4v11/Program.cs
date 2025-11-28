using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lab4_ShoppingCart
{
    // ==========================================
    // 1. ІНТЕРФЕЙС
    // ==========================================
    // Визначає контракт: кожен товар повинен мати Ім'я, Ціну та метод опису.
    public interface IProduct
    {
        string Name { get; }
        decimal Price { get; }
        string GetInfo();
    }

    // ==========================================
    // 2. АБСТРАКТНИЙ КЛАС
    // ==========================================
    // Реалізує базову логіку, щоб не дублювати код (DRY principle).
    public abstract class BaseProduct : IProduct
    {
        public string Name { get; set; }
        public decimal Price { get; set; }

        public BaseProduct(string name, decimal price)
        {
            Name = name;
            Price = price;
        }

        // Абстрактний метод: конкретна реалізація буде у класах-спадкоємцях
        public abstract string GetInfo();
    }

    // ==========================================
    // 3. КОНКРЕТНІ РЕАЛІЗАЦІЇ (КЛАСИ)
    // ==========================================

    // Клас Їжа
    public class Food : BaseProduct
    {
        public DateTime ExpirationDate { get; set; }

        public Food(string name, decimal price, DateTime expirationDate) 
            : base(name, price)
        {
            ExpirationDate = expirationDate;
        }

        public override string GetInfo()
        {
            return $"[Їжа] {Name} — {Price} грн (Вжити до: {ExpirationDate.ToShortDateString()})";
        }
    }

    // Клас Одяг
    public class Clothes : BaseProduct
    {
        public string Size { get; set; }
        public string Material { get; set; }

        public Clothes(string name, decimal price, string size, string material) 
            : base(name, price)
        {
            Size = size;
            Material = material;
        }

        public override string GetInfo()
        {
            return $"[Одяг] {Name} — {Price} грн (Розмір: {Size}, Тканина: {Material})";
        }
    }

    // ==========================================
    // 4. КОШИК ТОВАРІВ (АГРЕГАЦІЯ)
    // ==========================================
    // Цей клас містить колекцію продуктів. Це зв'язок "має" (Has-a).
    public class ShoppingCart
    {
        // Список типу інтерфейсу дозволяє зберігати і Food, і Clothes разом
        private List<IProduct> _products;

        public ShoppingCart()
        {
            _products = new List<IProduct>();
        }

        public void AddProduct(IProduct product)
        {
            _products.Add(product);
            Console.WriteLine($"-> Додано в кошик: {product.Name}");
        }

        public void RemoveProduct(IProduct product)
        {
            _products.Remove(product);
            Console.WriteLine($"<- Видалено з кошика: {product.Name}");
        }

        public void ShowCart()
        {
            Console.WriteLine("\n=== Вміст Вашого кошика ===");
            if (_products.Count == 0)
            {
                Console.WriteLine("Кошик порожній.");
            }
            else
            {
                foreach (var item in _products)
                {
                    Console.WriteLine(item.GetInfo());
                }
            }
            Console.WriteLine("===========================\n");
        }

        // Обчислення суми
        public decimal CalculateTotal()
        {
            return _products.Sum(p => p.Price);
        }

        // Обчислення середньої ціни
        public decimal CalculateAveragePrice()
        {
            if (_products.Count == 0) return 0;
            return CalculateTotal() / _products.Count;
        }
    }

    // ==========================================
    // 5. ГОЛОВНА ПРОГРАМА
    // ==========================================
    class Program
    {
        static void Main(string[] args)
        {
            // Налаштування кодування для коректного відображення кирилиці
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine("Лабораторна робота №4: Інтерфейси та Агрегація\n");

            // 1. Створення екземпляра кошика
            ShoppingCart myCart = new ShoppingCart();

            // 2. Створення товарів
            // Ми можемо використовувати var, BaseProduct або IProduct для змінних
            var apples = new Food("Яблука Голден", 25.50m, DateTime.Now.AddDays(10));
            var bread = new Food("Хліб Бородинський", 18.00m, DateTime.Now.AddDays(2));
            
            var tshirt = new Clothes("Футболка Print", 450.00m, "L", "Бавовна");
            var jeans = new Clothes("Джинси Classic", 1200.00m, "32/34", "Денім");

            // 3. Додавання товарів у кошик
            myCart.AddProduct(apples);
            myCart.AddProduct(bread);
            myCart.AddProduct(tshirt);
            myCart.AddProduct(jeans);

            // 4. Вивід інформації
            myCart.ShowCart();

            // 5. Демонстрація обчислень
            decimal total = myCart.CalculateTotal();
            decimal average = myCart.CalculateAveragePrice();

            Console.WriteLine($"Загальна вартість замовлення: {total} грн");
            Console.WriteLine($"Середня вартість товару:      {Math.Round(average, 2)} грн");

            // 6. Приклад видалення
            Console.WriteLine("\n[Видаляємо хліб...]");
            myCart.RemoveProduct(bread);
            
            Console.WriteLine($"Нова загальна вартість:       {myCart.CalculateTotal()} грн");

            Console.WriteLine("\nНатисніть будь-яку клавішу для виходу...");
            Console.ReadKey();
        }
    }
}