using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Text;

namespace Lab7_RetryPattern
{

    public static class RetryHelper
    {
        public static T ExecuteWithRetry<T>(
            Func<T> operation, 
            int retryCount = 3, 
            TimeSpan initialDelay = default, 
            Func<Exception, bool> shouldRetry = null)
        {
            if (initialDelay == default) initialDelay = TimeSpan.FromSeconds(1);

            for (int attempt = 0; attempt <= retryCount; attempt++)
            {
                try
                {
                    return operation();
                }
                catch (Exception ex)
                {

                    if (attempt == retryCount)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"[Error] Всі {retryCount + 1} спроб вичерпано. Остання помилка: {ex.Message}");
                        Console.ResetColor();
                        throw;
                    }

                    if (shouldRetry != null && !shouldRetry(ex))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"[Critical] Помилка '{ex.GetType().Name}' не підлягає повтору.");
                        Console.ResetColor();
                        throw;
                    }

                    double backoffMultiplier = Math.Pow(2, attempt);
                    TimeSpan delay = TimeSpan.FromSeconds(initialDelay.TotalSeconds * backoffMultiplier);

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"[Retry] Невдача (Спроба {attempt + 1}/{retryCount + 1}). Помилка: {ex.Message}");
                    Console.WriteLine($"        Очікування {delay.TotalSeconds} с перед наступною спробою...");
                    Console.ResetColor();

                    Thread.Sleep(delay);
                }
            }


            throw new Exception("Невідома помилка в RetryHelper");
        }
    }

    public class FileProcessor
    {
        private int _attempts = 0;

        public List<string> LoadProductNames(string path)
        {
            _attempts++;
            Console.WriteLine($"-> [FileProcessor] Спроба читання файлу '{path}' (виклик №{_attempts})...");

            if (_attempts <= 2)
            {
                throw new FileNotFoundException($"Файл '{path}' тимчасово недоступний або заблокований.");
            }


            return new List<string> { "Ноутбук", "Смартфон", "Клавіатура", "Монітор" };
        }
    }

    public class NetworkClient
    {
        private int _attempts = 0;

        public List<string> GetProductsFromApi(string url)
        {
            _attempts++;
            Console.WriteLine($"-> [NetworkClient] GET запит до '{url}' (виклик №{_attempts})...");

            if (_attempts <= 3)
            {

                throw new HttpRequestException($"Помилка підключення до API (503 Service Unavailable).");
            }


            return new List<string> { "Apple", "Samsung", "Dell", "HP" };
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("=== Lab 7: Retry Pattern & Exception Handling ===\n");

            Console.WriteLine("--- Тест 1: Читання файлу (збоїть 2 рази) ---");
            var fileProcessor = new FileProcessor();

            try
            {
                List<string> productsFromFile = RetryHelper.ExecuteWithRetry(
                    operation: () => fileProcessor.LoadProductNames("products.txt"),
                    retryCount: 3,
                    initialDelay: TimeSpan.FromSeconds(0.5),
                    shouldRetry: (ex) => ex is FileNotFoundException || ex is IOException
                );

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Успіх! Отримано товарів з файлу: " + string.Join(", ", productsFromFile));
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FATAL: Не вдалося прочитати файл. {ex.Message}");
            }

            Console.WriteLine(new string('-', 40));
            Console.WriteLine();

            Console.WriteLine("--- Тест 2: API Запит (збоїть 3 рази) ---");
            var netClient = new NetworkClient();

            try
            {
                List<string> productsFromApi = RetryHelper.ExecuteWithRetry(
                    operation: () => netClient.GetProductsFromApi("https://api.shop.com/products"),
                    retryCount: 5,
                    initialDelay: TimeSpan.FromSeconds(1),
                    shouldRetry: (ex) => ex is HttpRequestException
                );

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Успіх! Отримано товарів з API: " + string.Join(", ", productsFromApi));
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FATAL: API недоступне. {ex.Message}");
            }

            Console.WriteLine("\nНатисніть будь-яку клавішу...");
            Console.ReadKey();
        }
    }
}