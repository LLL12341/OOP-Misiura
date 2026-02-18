using System;
using System.Collections.Generic;

namespace lab24
{

    // 1. Інтерфейс
    public interface INumericOperationStrategy
    {
        double Execute(double value);
    }

    // 2. реалізації стратегій
    public class SquareOperationStrategy : INumericOperationStrategy
    {
        public double Execute(double value) => Math.Pow(value, 2);
    }

    public class CubeOperationStrategy : INumericOperationStrategy
    {
        public double Execute(double value) => Math.Pow(value, 3);
    }

    public class SquareRootOperationStrategy : INumericOperationStrategy
    {
        public double Execute(double value)
        {
            if (value < 0) throw new ArgumentException("Корінь з від'ємного числа не підтримується.");
            return Math.Sqrt(value);
        }
    }

    // 3. Контекст який виконує стратегію
    public class NumericProcessor
    {
        private INumericOperationStrategy _strategy;

        public NumericProcessor(INumericOperationStrategy strategy)
        {
            _strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
        }

        public void SetStrategy(INumericOperationStrategy strategy)
        {
            _strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
            Console.WriteLine($"\n[Система] Стратегію змінено на: {strategy.GetType().Name}");
        }

        public double Process(double input)
        {
            return _strategy.Execute(input);
        }
    }

    public class ResultPublisher
    {
        // Подія, на яку будуть підписуватися спостерігачі
        public event Action<double, string> ResultCalculated;

        public void PublishResult(double result, string operationName)
        {
            // Перевіряємо, чи є підписники (Invoke викликає всі підписані методи)
            ResultCalculated?.Invoke(result, operationName);
        }
    }

    public class ConsoleLoggerObserver
    {
        public void OnResultCalculated(double result, string operationName)
        {
            Console.WriteLine($"  -> [ConsoleLogger] Операція: '{operationName}', Результат: {result}");
        }
    }

    public class HistoryLoggerObserver
    {
        public List<string> History { get; } = new List<string>();

        public void OnResultCalculated(double result, string operationName)
        {
            History.Add($"{DateTime.Now:HH:mm:ss} | {operationName}: {result}");
        }

        public void PrintHistory()
        {
            Console.WriteLine("\n=== Історія всіх операцій ===");
            foreach (var record in History)
            {
                Console.WriteLine(record);
            }
            Console.WriteLine("=============================\n");
        }
    }

    public class ThresholdNotifierObserver
    {
        private readonly double _threshold;

        public ThresholdNotifierObserver(double threshold)
        {
            _threshold = threshold;
        }

        public void OnResultCalculated(double result, string operationName)
        {
            if (result > _threshold)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"  -> [ThresholdNotifier] УВАГА! Результат {result} після '{operationName}' перевищує ліміт {_threshold}!");
                Console.ResetColor();
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var processor = new NumericProcessor(new SquareOperationStrategy());
            
            var publisher = new ResultPublisher();

            var consoleLogger = new ConsoleLoggerObserver();
            var historyLogger = new HistoryLoggerObserver();
            var thresholdNotifier = new ThresholdNotifierObserver(50.0); // Поріг = 50

            publisher.ResultCalculated += consoleLogger.OnResultCalculated;
            publisher.ResultCalculated += historyLogger.OnResultCalculated;
            publisher.ResultCalculated += thresholdNotifier.OnResultCalculated;

            // Тестування
            
            double number1 = 5;
            double result1 = processor.Process(number1);
            publisher.PublishResult(result1, $"Зведення в квадрат числа {number1}");

            double number2 = 8;
            double result2 = processor.Process(number2);
            publisher.PublishResult(result2, $"Зведення в квадрат числа {number2}");

            processor.SetStrategy(new CubeOperationStrategy());
            
            double number3 = 4;
            double result3 = processor.Process(number3);
            publisher.PublishResult(result3, $"Зведення в куб числа {number3}");

            processor.SetStrategy(new SquareRootOperationStrategy());
            
            double number4 = 144;
            double result4 = processor.Process(number4);
            publisher.PublishResult(result4, $"Квадратний корінь числа {number4}");

            historyLogger.PrintHistory();

            publisher.ResultCalculated -= consoleLogger.OnResultCalculated;
            publisher.ResultCalculated -= thresholdNotifier.OnResultCalculated;
        }
    }
}