using System;

namespace lab2v11
{
    // --- Клас Temperature (Варіант 11) ---
    public class Temperature
    {
        // 1. Приватне поле celsius
        private double celsius;

        // Конструктор
        public Temperature(double celsius)
        {
            this.celsius = celsius;
        }

        // 2. Властивості
        // Доступ у Цельсіях
        public double Celsius
        {
            get { return celsius; }
            set { celsius = value; }
        }

        // Доступ у Фаренгейтах (обчислюється)
        public double Fahrenheit
        {
            get 
            { 
                return (celsius * 9.0 / 5.0) + 32; 
            }
            set 
            { 
                celsius = (value - 32) * 5.0 / 9.0; 
            }
        }

        // 3. Індексатор (доступ через ["C"] або ["F"])
        public double this[string unit]
        {
            get
            {
                if (unit.ToUpper() == "C") return Celsius;
                if (unit.ToUpper() == "F") return Fahrenheit;
                throw new ArgumentException("Невідома одиниця. Використовуйте 'C' або 'F'.");
            }
            set
            {
                if (unit.ToUpper() == "C") Celsius = value;
                else if (unit.ToUpper() == "F") Fahrenheit = value;
                else throw new ArgumentException("Невідома одиниця.");
            }
        }

        // 4. Перевантаження операторів
        public static bool operator >(Temperature t1, Temperature t2)
        {
            return t1.celsius > t2.celsius;
        }

        public static bool operator <(Temperature t1, Temperature t2)
        {
            return t1.celsius < t2.celsius;
        }

        public static bool operator ==(Temperature t1, Temperature t2)
        {
            if (ReferenceEquals(t1, null)) return ReferenceEquals(t2, null);
            return Math.Abs(t1.celsius - t2.celsius) < 0.001;
        }

        public static bool operator !=(Temperature t1, Temperature t2)
        {
            return !(t1 == t2);
        }

        public override bool Equals(object obj)
        {
            if (obj is Temperature other)
                return Math.Abs(this.celsius - other.celsius) < 0.001;
            return false;
        }

        public override int GetHashCode()
        {
            return celsius.GetHashCode();
        }

        public override string ToString()
        {
            return $"{celsius:F1}°C";
        }
    }

    // --- Основна програма ---
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Lab 2: Variant 11 (Temperature) ===\n");

            // 1. Створення об'єкта
            Temperature t1 = new Temperature(20); // 20 градусів C
            Console.WriteLine($"Початкова температура: {t1.Celsius}°C");

            // 2. Робота з властивістю Fahrenheit
            Console.WriteLine($"У Фаренгейтах: {t1.Fahrenheit}°F");
            
            t1.Fahrenheit = 100; // Змінюємо через Фаренгейт
            Console.WriteLine($"Змінили на 100°F. Тепер у Цельсіях: {t1.Celsius:F1}°C\n");

            // 3. Робота з індексатором
            Console.WriteLine("--- Перевірка індексатора ---");
            t1["C"] = 0; // Ставимо 0 градусів
            Console.WriteLine($"Значення через індексатор ['C']: {t1["C"]}");
            Console.WriteLine($"Значення через індексатор ['F']: {t1["F"]}\n");

            // 4. Робота з операторами
            Console.WriteLine("--- Перевірка операторів ---");
            Temperature tempA = new Temperature(10);
            Temperature tempB = new Temperature(20);

            Console.WriteLine($"TempA ({tempA.Celsius}) < TempB ({tempB.Celsius}): {tempA < tempB}");
            Console.WriteLine($"TempA ({tempA.Celsius}) > TempB ({tempB.Celsius}): {tempA > tempB}");
            
            Temperature tempC = new Temperature(10);
            Console.WriteLine($"TempA == TempC: {tempA == tempC}");
        }
    }
}