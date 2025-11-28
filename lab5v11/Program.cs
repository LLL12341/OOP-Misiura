using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lab5_PriceList
{

    public class ProductNotFoundException : Exception
    {
        public ProductNotFoundException(string code) 
            : base($"Помилка: Товар з кодом '{code}' не знайдено у прайс-листі.") { }
    }

    public class InvalidDataException : Exception
    {
        public InvalidDataException(string message) : base($"Помилка даних: {message}") { }
    }

    public interface IIdentifiable
    {
        string Code { get; }
    }

    // Сутність: Товар у прайс-листі
    public class PriceItem : IIdentifiable
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }

        public PriceItem(string code, string name, decimal price)
        {
            if (string.IsNullOrWhiteSpace(code)) throw new InvalidDataException("Код не може бути порожнім.");
            if (price < 0) throw new InvalidDataException("Ціна не може бути від'ємною.");

            Code = code;
            Name = name;
            Price = price;
        }

        public override string ToString() => $"[{Code}] {Name} - {Price} грн";
    }

    public class CartItem
    {
        public PriceItem Product { get; private set; }
        public int Quantity { get; private set; }

        public CartItem(PriceItem product, int quantity)
        {
            if (product == null) throw new ArgumentNullException(nameof(product));
            if (quantity <= 0) throw new InvalidDataException("Кількість товару має бути більшою за 0.");

            Product = product;
            Quantity = quantity;
        }

        public decimal TotalPrice => Product.Price * Quantity;

        public override string ToString()
        {
            return $"{Product.Name} (x{Quantity}) = {TotalPrice} грн";
        }
    }

    public class Repository<T> where T : IIdentifiable
    {
        private List<T> _items = new List<T>();

        public void Add(T item)
        {
            if (_items.Any(x => x.Code == item.Code))
            {
                throw new InvalidDataException($"Елемент з кодом {item.Code} вже існує.");
            }
            _items.Add(item);
        }

        public T GetByCode(string code)
        {
            var item = _items.FirstOrDefault(x => x.Code == code);
            if (item == null)
            {
                throw new ProductNotFoundException(code);
            }
            return item;
        }

        public IEnumerable<T> GetAll() => _items;
    }

    public class ShoppingCart
    {
        private List<CartItem> _items = new List<CartItem>();

        public void AddToCart(PriceItem product, int quantity)
        {
            var existingItem = _items.FirstOrDefault(i => i.Product.Code == product.Code);
            if (existingItem != null)
            {

                _items.Add(new CartItem(product, quantity)); 
            }
            else
            {
                _items.Add(new CartItem(product, quantity));
            }
            Console.WriteLine($"-> Додано в кошик: {product.Name}, {quantity} шт.");
        }

        public void ShowReceipt()
        {
            Console.WriteLine("\n--- ЧЕК ---");
            if (_items.Count == 0) { Console.WriteLine("Кошик порожній"); return; }

            foreach (var item in _items)
            {
                Console.WriteLine(item);
            }
            Console.WriteLine("-----------");
        }

        public decimal GetTotalAmount()
        {
            return _items.Sum(x => x.TotalPrice);
        }

        public decimal GetAverageItemPrice()
        {
            if (_items.Count == 0) return 0;
            return _items.Average(x => x.Product.Price); 
        }

        public decimal GetTotalWithDiscount(decimal discountPercent)
        {
            decimal total = GetTotalAmount();
            decimal discountAmount = total * (discountPercent / 100);
            return total - discountAmount;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            
            Repository<PriceItem> priceList = new Repository<PriceItem>();

            try
            {
                priceList.Add(new PriceItem("A001", "Ноутбук", 25000m));
                priceList.Add(new PriceItem("A002", "Мишка", 800m));
                priceList.Add(new PriceItem("B005", "Клавіатура", 1500m));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка ініціалізації: {ex.Message}");
            }

            ShoppingCart cart = new ShoppingCart();
            
            Console.WriteLine("=== Симуляція покупок ===");

            try 
            {
                var laptop = priceList.GetByCode("A001");
                cart.AddToCart(laptop, 1);

                var mouse = priceList.GetByCode("A002");
                cart.AddToCart(mouse, 2);
            }
            catch (ProductNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
            }


            Console.WriteLine("\n[Тест] Спроба купити неіснуючий товар:");
            try
            {
                var ghostItem = priceList.GetByCode("Z999");
                cart.AddToCart(ghostItem, 1);
            }
            catch (ProductNotFoundException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"CAUGHT EXCEPTION: {ex.Message}");
                Console.ResetColor();
            }

            Console.WriteLine("\n[Тест] Спроба купити -5 клавіатур:");
            try
            {
                var kb = priceList.GetByCode("B005");
                cart.AddToCart(kb, -5);
            }
            catch (InvalidDataException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"CAUGHT EXCEPTION: {ex.Message}");
                Console.ResetColor();
            }

            cart.ShowReceipt();

            decimal total = cart.GetTotalAmount();
            decimal avgPrice = cart.GetAverageItemPrice();
            decimal discountPercent = 7.0m;
            decimal totalDiscounted = cart.GetTotalWithDiscount(discountPercent);

            Console.WriteLine($"Сума без знижки:      {total} грн");
            Console.WriteLine($"Середня ціна позиції: {Math.Round(avgPrice, 2)} грн");
            Console.WriteLine($"Зі знижкою {discountPercent}%:       {Math.Round(totalDiscounted, 2)} грн");

            Console.ReadKey();
        }
    }
}