using System;
using System.Collections.Generic;

namespace lab21
{
    public interface IShippingStrategy
    {
        decimal CalculateCost(decimal distance, decimal weight);
    }

    public class StandardShippingStrategy : IShippingStrategy
    {
        public decimal CalculateCost(decimal distance, decimal weight) => 
            distance * 1.5m + weight * 0.5m;
    }

    public class ExpressShippingStrategy : IShippingStrategy
    {
        public decimal CalculateCost(decimal distance, decimal weight) => 
            (distance * 2.5m + weight * 1.0m) + 50m;
    }

    public class InternationalShippingStrategy : IShippingStrategy
    {
        public decimal CalculateCost(decimal distance, decimal weight)
        {
            decimal baseCost = distance * 5.0m + weight * 2.0m;
            return baseCost * 1.15m; // + 15% податку
        }
    }

    public class NightShippingStrategy : IShippingStrategy
    {
        public decimal CalculateCost(decimal distance, decimal weight)
        {

            return (distance * 1.5m + weight * 0.5m) + 100m;
        }
    }

    public static class ShippingStrategyFactory
    {
        public static IShippingStrategy CreateStrategy(string deliveryType)
        {
            return deliveryType.ToLower() switch
            {
                "standard" => new StandardShippingStrategy(),
                "express" => new ExpressShippingStrategy(),
                "international" => new InternationalShippingStrategy(),
                "night" => new NightShippingStrategy(), // Додано в межах розширення
                _ => throw new ArgumentException("Невідомий тип доставки")
            };
        }
    }

    public class DeliveryService
    {
        public decimal CalculateDeliveryCost(decimal distance, decimal weight, IShippingStrategy strategy)
        {
            return strategy.CalculateCost(distance, weight);
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            DeliveryService service = new DeliveryService();

            Console.WriteLine("--- Система розрахунку вартості доставки ---");
            
            try 
            {
                Console.Write("Введіть тип доставки (Standard, Express, International, Night): ");
                string type = Console.ReadLine();

                Console.Write("Введіть відстань (км): ");
                decimal dist = decimal.Parse(Console.ReadLine());

                Console.Write("Введіть вагу (кг): ");
                decimal wght = decimal.Parse(Console.ReadLine());

                IShippingStrategy strategy = ShippingStrategyFactory.CreateStrategy(type);

                decimal cost = service.CalculateDeliveryCost(dist, wght, strategy);

                Console.WriteLine($"\nРезультат: Вартість доставки '{type}' складає: {cost:F2} грн.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }

            Console.WriteLine("\nНатисніть будь-яку клавішу для виходу...");
            Console.ReadKey();
        }
    }
}