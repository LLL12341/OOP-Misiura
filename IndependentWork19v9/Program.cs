using System;

namespace IndependentWork19
{
    public interface ICache
    {
        void CacheData(string data);
    }

    public class InMemoryCache : ICache
    {
        public void CacheData(string data)
        {
            Console.WriteLine($"[IN-MEMORY] Збережено в локальній пам'яті: {data}");
        }
    }

    public class RedisCache : ICache
    {
        public void CacheData(string data)
        {
            Console.WriteLine($"[REDIS] Збережено в розподіленому кеші: {data}");
        }
    }

    public class MemcachedCache : ICache
    {
        public void CacheData(string data)
        {
            Console.WriteLine($"[MEMCACHED] Збережено в Memcached: {data}");
        }
    }

    public abstract class CacheFactory
    {
        protected abstract ICache CreateCache();

        public void ExecuteCache(string data)
        {
            ICache cache = CreateCache();
            cache.CacheData(data);
        }
    }

    public class InMemoryCacheFactory : CacheFactory
    {
        protected override ICache CreateCache()
        {
            return new InMemoryCache();
        }
    }

    public class RedisCacheFactory : CacheFactory
    {
        protected override ICache CreateCache()
        {
            return new RedisCache();
        }
    }

    public class MemcachedCacheFactory : CacheFactory
    {
        protected override ICache CreateCache()
        {
            return new MemcachedCache();
        }
    }

    public class CacheManager
    {
        private static CacheManager? _instance;
        private static readonly object _lock = new object();
        private CacheFactory? _currentFactory;

        // Приватний конструктор, щоб запобігти створенню об'єктів ззовні
        private CacheManager() { }

        // Глобальна точка доступу до єдиного екземпляра
        public static CacheManager Instance
        {
            get
            {
                // Double-check locking для потокобезпечності
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new CacheManager();
                        }
                    }
                }
                return _instance;
            }
        }

        // Встановлення поточної фабрики
        public void SetCacheFactory(CacheFactory factory)
        {
            _currentFactory = factory;
        }

        // Делегування роботи з кешем поточній фабриці
        public void Cache(string data)
        {
            if (_currentFactory == null)
            {
                Console.WriteLine("[ПОМИЛКА] Фабрика кешування не встановлена!");
                return;
            }
            _currentFactory.ExecuteCache(data);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            
            // Отримуємо єдиний екземпляр CacheManager
            CacheManager cacheManager = CacheManager.Instance;

            Console.WriteLine("=== Тестування InMemoryCache ===");
            cacheManager.SetCacheFactory(new InMemoryCacheFactory());
            cacheManager.Cache("Налаштування профілю користувача");
            cacheManager.Cache("Тимчасові дані сесії\n");

            Console.WriteLine("=== Тестування RedisCache ===");
            cacheManager.SetCacheFactory(new RedisCacheFactory());
            cacheManager.Cache("Кошик товарів #1045");
            cacheManager.Cache("Токен авторизації JWT\n");

            Console.WriteLine("=== Тестування MemcachedCache ===");
            cacheManager.SetCacheFactory(new MemcachedCacheFactory());
            cacheManager.Cache("Результат запиту до БД: список статей");
            cacheManager.Cache("Статистика відвідувань сайту за добу\n");

            Console.WriteLine("Роботу завершено. Натисніть будь-яку клавішу...");
            Console.ReadKey();
        }
    }
}