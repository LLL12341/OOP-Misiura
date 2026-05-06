using System;
using System.Collections.Generic;
using Xunit;

namespace IndependentWork21
{
    // ==========================================
    // СИСТЕМА ДЛЯ ТЕСТУВАННЯ (SUT - System Under Test)
    // ==========================================

    // 1. STRATEGY: Інтерфейси та конкретні стратегії
    public interface IProcessorStrategy
    {
        string Process(string data);
    }

    public class TempStrategy : IProcessorStrategy
    {
        public string Process(string data) => $"[TEMP_PROCESSED] {data}";
    }

    public class PressureStrategy : IProcessorStrategy
    {
        public string Process(string data) => $"[PRESSURE_PROCESSED] {data}";
    }

    // 2. FACTORY: Фабрика для створення стратегій
    public static class StrategyFactory
    {
        public static IProcessorStrategy Create(string sensorType)
        {
            return sensorType.ToLower() switch
            {
                "temperature" => new TempStrategy(),
                "pressure" => new PressureStrategy(),
                _ => throw new ArgumentException($"Невідомий тип сенсора: {sensorType}")
            };
        }
    }

    // 3. SINGLETON + 4. OBSERVER (Subject): Головний конвеєр обробки
    public class DataPipeline
    {
        private static DataPipeline? _instance;
        private static readonly object _lock = new object();

        private IProcessorStrategy? _strategy;

        // Подія для Observer
        public event Action<string>? OnDataProcessed;

        private DataPipeline() { }

        public static DataPipeline Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null) _instance = new DataPipeline();
                    }
                }
                return _instance;
            }
        }

        // Встановлення стратегії
        public void SetStrategy(IProcessorStrategy strategy)
        {
            _strategy = strategy;
        }

        // Обробка даних і сповіщення підписників
        public void Execute(string rawData)
        {
            if (_strategy == null)
            {
                throw new InvalidOperationException("Стратегія обробки не встановлена!");
            }

            string result = _strategy.Process(rawData);
            OnDataProcessed?.Invoke(result); // Сповіщаємо Observer'ів
        }

        // Метод для очищення стану (тільки для тестів, щоб Singleton не афектував інші тести)
        public void ResetForTests()
        {
            _strategy = null;
            OnDataProcessed = null;
        }
    }

    // OBSERVER: Конкретний спостерігач для тестів
    public class TestObserver
    {
        public List<string> ReceivedLogs { get; } = new List<string>();

        public void HandleData(string data)
        {
            ReceivedLogs.Add(data);
        }
    }

    // ==========================================
    // ІНТЕГРАЦІЙНІ ТЕСТИ
    // ==========================================

    public class PipelineIntegrationTests : IDisposable
    {
        public PipelineIntegrationTests()
        {
            // Очищуємо Singleton перед кожним тестом
            DataPipeline.Instance.ResetForTests();
        }

        public void Dispose()
        {
            DataPipeline.Instance.ResetForTests();
        }

        // --- ПОЗИТИВНІ СЦЕНАРІЇ (3 шт) ---

        [Fact]
        public void Test1_FullPipeline_ValidData_ShouldProcessAndNotify()
        {
            // Arrange
            var pipeline = DataPipeline.Instance;
            var observer = new TestObserver();
            pipeline.OnDataProcessed += observer.HandleData;

            // Створюємо стратегію через Factory і встановлюємо в Singleton
            var strategy = StrategyFactory.Create("temperature");
            pipeline.SetStrategy(strategy);

            // Act
            pipeline.Execute("24.5C");

            // Assert
            Assert.Single(observer.ReceivedLogs);
            Assert.Equal("[TEMP_PROCESSED] 24.5C", observer.ReceivedLogs[0]);
        }

        [Fact]
        public void Test2_StrategyChangeAtRuntime_ShouldApplyNewLogicAndNotify()
        {
            // Arrange
            var pipeline = DataPipeline.Instance;
            var observer = new TestObserver();
            pipeline.OnDataProcessed += observer.HandleData;

            // Act: Використовуємо першу стратегію
            pipeline.SetStrategy(StrategyFactory.Create("temperature"));
            pipeline.Execute("20C");

            // Змінюємо стратегію в runtime (Strategy pattern)
            pipeline.SetStrategy(StrategyFactory.Create("pressure"));
            pipeline.Execute("1015hPa");

            // Assert
            Assert.Equal(2, observer.ReceivedLogs.Count);
            Assert.Equal("[TEMP_PROCESSED] 20C", observer.ReceivedLogs[0]);
            Assert.Equal("[PRESSURE_PROCESSED] 1015hPa", observer.ReceivedLogs[1]);
        }

        [Fact]
        public void Test3_SingletonStability_MultipleReferencesShouldShareState()
        {
            // Arrange
            var ref1 = DataPipeline.Instance;
            var ref2 = DataPipeline.Instance;
            var observer = new TestObserver();
            
            // Підписуємось через перше посилання
            ref1.OnDataProcessed += observer.HandleData;
            ref1.SetStrategy(StrategyFactory.Create("pressure"));

            // Act: Викликаємо виконання через друге посилання
            ref2.Execute("990hPa");

            // Assert
            Assert.Same(ref1, ref2); // Перевірка Singleton
            Assert.Single(observer.ReceivedLogs);
            Assert.Equal("[PRESSURE_PROCESSED] 990hPa", observer.ReceivedLogs[0]);
        }

        // --- НЕГАТИВНІ/ГРАНИЧНІ СЦЕНАРІЇ (2 шт) ---

        [Fact]
        public void Test4_ExecuteWithoutStrategy_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var pipeline = DataPipeline.Instance;
            // Стратегію спеціально НЕ встановлюємо

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => pipeline.Execute("SomeData"));
            Assert.Equal("Стратегія обробки не встановлена!", exception.Message);
        }

        [Fact]
        public void Test5_FactoryUnknownType_ShouldThrowArgumentException()
        {
            // Arrange & Act
            var action = () => StrategyFactory.Create("unknown_sensor");

            // Assert
            var exception = Assert.Throws<ArgumentException>(action);
            Assert.Contains("Невідомий тип сенсора", exception.Message);
        }
    }
}