using System;
using System.IO;

namespace lab25
{
    public interface ILogger
    {
        void Log(string message);
    }

    public class ConsoleLogger : ILogger
    {
        public void Log(string message) => Console.WriteLine($"[Console] {message}");
    }

    public class FileLogger : ILogger
    {
        private readonly string _filePath = "system_log.txt";

        public void Log(string message)
        {
            File.AppendAllText(_filePath, $"{DateTime.Now}: {message}\n");
            // Дублюємо в консоль з позначкою, щоб візуально бачити, що працює FileLogger
            Console.WriteLine($"[File -> system_log.txt] {message}"); 
        }
    }

    public abstract class LoggerFactory
    {
        public abstract ILogger CreateLogger();
    }

    public class ConsoleLoggerFactory : LoggerFactory
    {
        public override ILogger CreateLogger() => new ConsoleLogger();
    }

    public class FileLoggerFactory : LoggerFactory
    {
        public override ILogger CreateLogger() => new FileLogger();
    }

    public class LoggerManager
    {
        private static LoggerManager _instance;
        private LoggerFactory _factory;


        private LoggerManager() { }

        public static LoggerManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new LoggerManager();
                return _instance;
            }
        }

        public void SetFactory(LoggerFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            Console.WriteLine($"\n--- LoggerManager: Фабрику змінено на {factory.GetType().Name} ---");
        }

        public void LogData(string message)
        {
            if (_factory == null)
                throw new InvalidOperationException("Фабрика логерів не встановлена!");

            ILogger logger = _factory.CreateLogger();
            logger.Log(message);
        }
    }

    public interface IDataProcessorStrategy
    {
        string Process(string data);
    }

    public class EncryptDataStrategy : IDataProcessorStrategy
    {
        public string Process(string data)
        {
            // Проста симуляція шифрування (наприклад, реверс рядка)
            char[] charArray = data.ToCharArray();
            Array.Reverse(charArray);
            return $"ENCRYPTED[{new string(charArray)}]";
        }
    }

    public class CompressDataStrategy : IDataProcessorStrategy
    {
        public string Process(string data)
        {

            return $"COMPRESSED[{data.Replace(" ", "")}]";
        }
    }

    public class DataContext
    {
        private IDataProcessorStrategy _strategy;

        public DataContext(IDataProcessorStrategy strategy)
        {
            _strategy = strategy;
        }

        public void SetStrategy(IDataProcessorStrategy strategy)
        {
            _strategy = strategy;
            Console.WriteLine($"\n--- DataContext: Стратегію змінено на {strategy.GetType().Name} ---");
        }

        public string ExecuteStrategy(string data)
        {
            return _strategy.Process(data);
        }
    }

    public class DataPublisher
    {
        public event Action<string, string> DataProcessed;

        public void PublishDataProcessed(string originalData, string processedData)
        {
            DataProcessed?.Invoke(originalData, processedData);
        }
    }

    public class ProcessingLoggerObserver
    {
        public void OnDataProcessed(string originalData, string processedData)
        {
            string logMessage = $"Обробка завершена. Оригінал: '{originalData}' | Результат: '{processedData}'";
            LoggerManager.Instance.LogData(logMessage);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            string testData = "Hello World 2026";

            var loggerManager = LoggerManager.Instance;
            var dataPublisher = new DataPublisher();
            var loggerObserver = new ProcessingLoggerObserver();
            
            dataPublisher.DataProcessed += loggerObserver.OnDataProcessed;


            Console.WriteLine(">>> СЦЕНАРІЙ 1: Повна інтеграція <<<");
            loggerManager.SetFactory(new ConsoleLoggerFactory());
            
            var dataContext = new DataContext(new EncryptDataStrategy());
            
            string result1 = dataContext.ExecuteStrategy(testData);
            dataPublisher.PublishDataProcessed(testData, result1);


            Console.WriteLine("\n>>> СЦЕНАРІЙ 2: Динамічна зміна логера <<<");
            loggerManager.SetFactory(new FileLoggerFactory());
            
            string result2 = dataContext.ExecuteStrategy(testData);
            dataPublisher.PublishDataProcessed(testData, result2);


            Console.WriteLine("\n>>> СЦЕНАРІЙ 3: Динамічна зміна стратегії <<<");
            dataContext.SetStrategy(new CompressDataStrategy());
            
            string result3 = dataContext.ExecuteStrategy(testData);
            dataPublisher.PublishDataProcessed(testData, result3);

            Console.WriteLine("\nСимуляцію успішно завершено! Перевірте файл system_log.txt у директорії проєкту.");
        }
    }
}