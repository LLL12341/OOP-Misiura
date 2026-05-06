using System;
using System.Threading;

namespace IndependentWork23
{
    public interface IInventoryUpdater
    {
        void Update(string itemName, int count);
    }

    public class ExternalStockSystem
    {
        public void UpdateStock(string item, int quantity)
        {
            Console.WriteLine($"[ExternalStockSystem] Оновлено запаси: {item} у кількості {quantity} шт.");
        }
    }

    public class ExternalStockAdapter : IInventoryUpdater
    {
        private readonly ExternalStockSystem _externalSystem;

        public ExternalStockAdapter(ExternalStockSystem externalSystem)
        {
            _externalSystem = externalSystem;
        }

        public void Update(string itemName, int count)
        {
            _externalSystem.UpdateStock(itemName, count);
        }
    }

    // Subsystem 1
    public class ProductCatalog
    {
        public bool GetProduct(string productId)
        {
            Console.WriteLine($"[Catalog] Перевірка наявності товару '{productId}' у каталозі...");
            return true;
        }
    }

    // Subsystem 2
    public class WarehouseManager
    {
        public int GetStock(string productId)
        {
            Console.WriteLine($"[Warehouse] Отримання залишків для '{productId}'...");
            return 100;
        }
    }

    // Subsystem 3
    public class OrderProcessor
    {
        public void Process(string productId, int quantity)
        {
            Console.WriteLine($"[OrderProcessor] Оформлення замовлення на {quantity} шт. товару '{productId}'.");
        }
    }

    public class InventoryFacade
    {
        private readonly ProductCatalog _catalog;
        private readonly WarehouseManager _warehouse;
        private readonly OrderProcessor _processor;

        public InventoryFacade()
        {
            _catalog = new ProductCatalog();
            _warehouse = new WarehouseManager();
            _processor = new OrderProcessor();
        }

        public void CheckAndOrderProduct(string productId, int quantity)
        {
            Console.WriteLine($"\n--- Фасад: Початок обробки замовлення ({productId}) ---");
            if (_catalog.GetProduct(productId))
            {
                int stock = _warehouse.GetStock(productId);
                if (stock >= quantity)
                {
                    _processor.Process(productId, quantity);
                    Console.WriteLine("--- Фасад: Замовлення успішно завершено! ---");
                }
                else
                {
                    Console.WriteLine("--- Фасад: Помилка. Недостатньо товару на складі! ---");
                }
            }
            else
            {
                Console.WriteLine("--- Фасад: Помилка. Товар не знайдено у каталозі! ---");
            }
        }
    }


    public interface IProductInfo
    {
        string GetName();
        decimal GetPrice();
    }

    // RealSubject (Реальний об'єкт, який звертається до БД - ресурсоємна операція)
    public class RealProductInfo : IProductInfo
    {
        private readonly string _productId;

        public RealProductInfo(string productId)
        {
            _productId = productId;
            Console.WriteLine($"[RealProductInfo] Завантаження даних з БД для {_productId} (це займає час)...");
            Thread.Sleep(1000); // Імітація затримки звернення до БД
        }

        public string GetName() => $"Ноутбук Ігровий ({_productId})";
        public decimal GetPrice() => 45000.50m;
    }

    // Proxy (Контролює доступ до RealSubject: ліміти прав та ледаче завантаження/кешування)
    public class SecurityProductInfoProxy : IProductInfo
    {
        private RealProductInfo? _realProductInfo;
        private readonly string _productId;
        private readonly string _userRole;

        public SecurityProductInfoProxy(string productId, string userRole)
        {
            _productId = productId;
            _userRole = userRole;
        }

        public string GetName()
        {
            // Ім'я можуть бачити всі
            EnsureRealProductLoaded();
            return _realProductInfo!.GetName();
        }

        public decimal GetPrice()
        {
            // Ціну можуть бачити лише авторизовані ролі
            if (_userRole != "Admin" && _userRole != "Manager")
            {
                Console.WriteLine($"[Proxy] ВІДМОВА В ДОСТУПІ: Роль '{_userRole}' не має прав для перегляду ціни.");
                return 0;
            }

            EnsureRealProductLoaded();
            return _realProductInfo!.GetPrice();
        }

        // Ледаче завантаження (Lazy Loading) - об'єкт створюється лише при першому зверненні
        private void EnsureRealProductLoaded()
        {
            if (_realProductInfo == null)
            {
                Console.WriteLine("[Proxy] Ініціалізація реального об'єкта (Lazy Load)...");
                _realProductInfo = new RealProductInfo(_productId);
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // ---------------------------------------------------------
            Console.WriteLine("=== 1. ДЕМОНСТРАЦІЯ ADAPTER ===");
            ExternalStockSystem oldSystem = new ExternalStockSystem();
            // Клієнт працює через зрозумілий йому інтерфейс IInventoryUpdater
            IInventoryUpdater updater = new ExternalStockAdapter(oldSystem);
            updater.Update("ASUS ROG", 15);

            // ---------------------------------------------------------
            Console.WriteLine("\n=== 2. ДЕМОНСТРАЦІЯ FACADE ===");
            InventoryFacade facade = new InventoryFacade();
            // Замість того, щоб смикати 3 різні класи, клієнт робить 1 виклик
            facade.CheckAndOrderProduct("ASUS ROG", 2);

            // ---------------------------------------------------------
            Console.WriteLine("\n=== 3. ДЕМОНСТРАЦІЯ PROXY ===");
            
            Console.WriteLine("\nСценарій 3.1: Доступ для ролі 'Guest' (Без прав на ціну)");
            IProductInfo guestProxy = new SecurityProductInfoProxy("ASUS ROG", "Guest");
            Console.WriteLine($"Назва товару: {guestProxy.GetName()}");
            decimal guestPrice = guestProxy.GetPrice(); // Тут буде відмова

            Console.WriteLine("\nСценарій 3.2: Доступ для ролі 'Admin' (Повні права)");
            IProductInfo adminProxy = new SecurityProductInfoProxy("ASUS ROG", "Admin");
            Console.WriteLine($"Назва товару: {adminProxy.GetName()}");
            // Зверніть увагу, що при другому виклику (GetPrice) база даних не завантажується знову, 
            // оскільки об'єкт уже створено (своєрідне кешування)
            Console.WriteLine($"Ціна товару: {adminProxy.GetPrice()} грн");

            Console.WriteLine("\nРоботу завершено. Натисніть будь-яку клавішу...");
            Console.ReadKey();
        }
    }
}