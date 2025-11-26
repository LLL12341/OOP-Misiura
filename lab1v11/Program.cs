using System;

namespace lab1v1
{
    // Опис класу Plane згідно з завданням
    class Plane
    {
        // --- Поля (Fields) ---
        public string airline;
        public string model;

        // --- Властивість (Property) ---
        public int Capacity { get; set; }

        // --- Конструктор (Constructor) ---
        // Викликається автоматично при створенні об'єкта (new Plane)
        public Plane(string airlineInput, string modelInput, int capacityInput)
        {
            airline = airlineInput;
            model = modelInput;
            Capacity = capacityInput;
            Console.WriteLine($"[Конструктор] ++ Створено новий літак: {airline} {model}");
        }

        // --- Деструктор (Destructor) ---
        // Викликається перед тим, як об'єкт буде видалено з пам'яті
        ~Plane()
        {
            Console.Write($"[Деструктор] -- Літак {model} авіакомпанії {airline} утилізовано.\n");
        }

        // --- Метод (Method) ---
        public void Fly()
        {
            Console.WriteLine($"   -> Літак {model} компанії '{airline}' злітає! Макс. пасажирів: {Capacity}.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Start Lab 1 ===\n");

            // 1. Створення 3-х об'єктів (екземплярів класу)
            Plane plane1 = new Plane("Boeing", "737 Max", 200);
            Plane plane2 = new Plane("WizzAir", "Airbus A321", 230);
            Plane plane3 = new Plane("Antonov", "An-225 Mriya", 6); // Екіпаж

            Console.WriteLine(); // Просто відступ

            // 2. Виклик методу Fly() для кожного об'єкта
            plane1.Fly();
            plane2.Fly();
            plane3.Fly();

            Console.WriteLine("\n=== End Lab 1 ===");

            // Цей блок коду потрібен тільки для лабораторної, 
            // щоб ви точно побачили повідомлення від Деструктора в консолі
            // (у реальних програмах викликати GC вручну не рекомендується)
            GC.Collect(); 
            GC.WaitForPendingFinalizers();
        }
    }
}