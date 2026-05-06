using System;

namespace IndependentWork20
{
    public interface IDataProcessorStrategy
    {
        void Process(string data);
    }

    public class TemperatureProcessingStrategy : IDataProcessorStrategy
    {
        public void Process(string data)
        {
            Console.WriteLine($"[STRATEGY] Обробка даних температури: {data} °C");
        }
    }

    public class PressureProcessingStrategy : IDataProcessorStrategy
    {
        public void Process(string data)
        {
            Console.WriteLine($"[STRATEGY] Обробка даних атмосферного тиску: {data} гПа");
        }
    }

    public class HumidityProcessingStrategy : IDataProcessorStrategy
    {
        public void Process(string data)
        {
            Console.WriteLine($"[STRATEGY] Обробка даних вологості повітря: {data} %");
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
        }

        public void ExecuteProcessing(string data)
        {
            if (_strategy == null)
            {
                Console.WriteLine("Стратегія не встановлена!");
                return;
            }
            _strategy.Process(data);
        }
    }


    public class DataPublisher
    {
        public event Action<string>? DataProcessed;

        public void PublishDataProcessed(string data)
        {
            DataProcessed?.Invoke(data);
        }
    }

    public class ConsoleOutputObserver
    {
        public void OnDataProcessed(string data)
        {
            Console.WriteLine($"[OBSERVER: Console] На екран виведено нові дані сенсора: {data}");
        }
    }

    public class DatabaseLoggerObserver
    {
        public void OnDataProcessed(string data)
        {
            Console.WriteLine($"[OBSERVER: Database] Збереження даних у базу даних: {data}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== ІНІЦІАЛІЗАЦІЯ СИСТЕМИ ===\n");

            // 1. Створюємо видавця
            DataPublisher publisher = new DataPublisher();

            // 2. Створюємо спостерігачів
            ConsoleOutputObserver consoleObserver = new ConsoleOutputObserver();
            DatabaseLoggerObserver databaseObserver = new DatabaseLoggerObserver();

            // 3. Підписуємо спостерігачів на подію DataProcessed
            publisher.DataProcessed += consoleObserver.OnDataProcessed;
            publisher.DataProcessed += databaseObserver.OnDataProcessed;

           
            DataContext context = new DataContext(new TemperatureProcessingStrategy());


            Console.WriteLine("=== СЦЕНАРІЙ 1: Температурний сенсор ===");
            string tempData = "+24.5";
            context.ExecuteProcessing(tempData); // Strategy
            publisher.PublishDataProcessed($"T={tempData}"); // Observer
            Console.WriteLine();


            Console.WriteLine("=== СЦЕНАРІЙ 2: Сенсор тиску (зміна стратегії) ===");
            context.SetStrategy(new PressureProcessingStrategy());
            string pressureData = "1015";
            context.ExecuteProcessing(pressureData);
            publisher.PublishDataProcessed($"P={pressureData}");
            Console.WriteLine();


            Console.WriteLine("=== СЦЕНАРІЙ 3: Сенсор вологості (зміна стратегії) ===");
            context.SetStrategy(new HumidityProcessingStrategy());
            string humidityData = "45";
            context.ExecuteProcessing(humidityData);
            publisher.PublishDataProcessed($"H={humidityData}");
            Console.WriteLine();

            Console.WriteLine("Роботу завершено. Натисніть будь-яку клавішу...");
            Console.ReadKey();
        }
    }
}