using Polly;
using Polly.CircuitBreaker;
using Polly.Timeout;
using Polly.Wrap;

namespace IndependentWork11
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("=== STARTING INDEPENDENT WORK 11: POLLY SCENARIOS ===\n");

            RunScenario1_DatabaseRetry();
            Console.WriteLine("\n--------------------------------------------------\n");
            
            RunScenario2_PaymentGatewayCircuitBreaker();
            Console.WriteLine("\n--------------------------------------------------\n");
            
            RunScenario3_RecommendationTimeoutFallback();

            Console.WriteLine("\n=== END OF WORK ===");
        }

        // ==================================================================================
        // СЦЕНАРІЙ 1: Підключення до БД (Retry + Exponential Backoff)
        // ==================================================================================
        private static void RunScenario1_DatabaseRetry()
        {
            Console.WriteLine("Сценарій 1: Доступ до БД (Retry з експоненційною затримкою)");
            
            int attempts = 0;

            // Політика: Чекати і пробувати знову (3 рази) зі зростаючою затримкою (2^n секунд)
            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetry(
                    retryCount: 3,
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                    onRetry: (exception, timeSpan, retryCount, context) =>
                    {
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [LOG] Спроба #{retryCount} не вдалася: {exception.Message}. Чекаємо {timeSpan.TotalSeconds} с...");
                    });

            try
            {
                retryPolicy.Execute(() =>
                {
                    attempts++;
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Спроба підключення до БД #{attempts}...");

                    // Імітуємо збій перші 2 рази
                    if (attempts <= 2)
                    {
                        throw new Exception("Connection refused (Transient Error)");
                    }

                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Успішне підключення до БД!");
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FAIL: {ex.Message}");
            }
        }

        // ==================================================================================
        // СЦЕНАРІЙ 2: Платіжний шлюз (Circuit Breaker)
        // ==================================================================================
        private static void RunScenario2_PaymentGatewayCircuitBreaker()
        {
            Console.WriteLine("Сценарій 2: Платіжний шлюз (Circuit Breaker)");

            // Політика: Якщо трапиться 2 помилки підряд, розмикаємо ланцюг на 5 секунд.
            var circuitBreakerPolicy = Policy
                .Handle<ApplicationException>()
                .CircuitBreaker(
                    exceptionsAllowedBeforeBreaking: 2,
                    durationOfBreak: TimeSpan.FromSeconds(5),
                    onBreak: (ex, breakDelay) =>
                    {
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [LOG] !!! ЛАНЦЮГ РОЗІМКНЕНО !!! Шлюз не відповідає. Блокування на {breakDelay.TotalSeconds} с. Помилка: {ex.Message}");
                    },
                    onReset: () =>
                    {
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [LOG] Ланцюг замкнено. Система відновилася.");
                    },
                    onHalfOpen: () =>
                    {
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [LOG] Ланцюг напіввідкритий. Тестовий запит...");
                    }
                );

            // Імітуємо серію запитів
            for (int i = 1; i <= 6; i++)
            {
                try
                {
                    Console.WriteLine($"Request #{i}: Calling Payment Gateway...");
                    circuitBreakerPolicy.Execute(() =>
                    {
                        // Імітуємо постійну помилку, поки не мине час
                        bool serviceIsDown = i <= 4; 
                        if (serviceIsDown) throw new ApplicationException("Gateway Timeout 504");
                        
                        Console.WriteLine("Payment Successful!");
                    });
                }
                catch (BrokenCircuitException)
                {
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Request #{i} BLOCKED by Circuit Breaker.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Request #{i} Failed: {ex.Message}");
                }

                // Пауза між запитами, щоб побачити роботу таймера
                Thread.Sleep(1500); 
            }
        }

        // ==================================================================================
        // СЦЕНАРІЙ 3: Сервіс рекомендацій (Timeout + Fallback)
        // ==================================================================================
        private static void RunScenario3_RecommendationTimeoutFallback()
        {
            Console.WriteLine("Сценарій 3: Сервіс рекомендацій (Timeout + Fallback)");

            // 1. Політика Timeout (нетипізована)
            var timeoutPolicy = Policy.Timeout(2, TimeoutStrategy.Pessimistic);

            // 2. Політика Fallback (типізована <string>)
            var fallbackPolicy = Policy<string>
                .Handle<TimeoutRejectedException>()
                .Or<Exception>()
                .Fallback(
                    fallbackValue: "Default Recommendations (Popular Items)", 
                    onFallback: (ex) => Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [LOG] Fallback спрацював через: {ex.GetType().Name}"));

            // 3. Об'єднання (ВИПРАВЛЕНО для сумісності типів)
            var policyWrap = fallbackPolicy.Wrap(timeoutPolicy);

            try
            {
                var result = policyWrap.Execute(() =>
                {
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Запит персональних рекомендацій (AI engine)...");
                    // Імітуємо довгу операцію (3 секунди), що більше за ліміт (2 секунди)
                    Thread.Sleep(3000); 
                    return "Personalized AI List";
                });

                Console.WriteLine($"Result: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Critical Failure: {ex.Message}");
            }
        }
    }
}