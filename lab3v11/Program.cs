using System;
using System.Collections.Generic;

namespace lab3v11
{
    // --- Базовий клас ---
    class Instrument
    {
        public string Name { get; set; }
        public int SongDurationMinutes { get; set; } // Тривалість однієї пісні

        // Конструктор базового класу
        public Instrument(string name, int duration)
        {
            Name = name;
            SongDurationMinutes = duration;
        }

        // Віртуальний метод (можна перевизначити у нащадках)
        public virtual void Play()
        {
            Console.WriteLine($"Інструмент {Name} грає музику.");
        }
    }

    // --- Похідний клас: Гітара ---
    class Guitar : Instrument
    {
        public int StringCount { get; set; } // Унікальне поле для гітари

        // Конструктор з викликом base(...)
        public Guitar(string name, int duration, int strings) : base(name, duration)
        {
            StringCount = strings;
        }

        // Перевизначення методу (Override)
        public override void Play()
        {
            Console.WriteLine($"🎸 Гітара '{Name}' ({StringCount} струн) грає рок-баладу. (Тривалість: {SongDurationMinutes} хв)");
        }
    }

    // --- Похідний клас: Піаніно ---
    class Piano : Instrument
    {
        public string Type { get; set; } // Наприклад: "Рояль", "Електронне"

        // Конструктор з викликом base(...)
        public Piano(string name, int duration, string type) : base(name, duration)
        {
            Type = type;
        }

        // Перевизначення методу (Override)
        public override void Play()
        {
            Console.WriteLine($"🎹 Піаніно '{Name}' ({Type}) виконує класичну сонату. (Тривалість: {SongDurationMinutes} хв)");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Lab 3: Inheritance & Polymorphism (Music) ===\n");

            // 1. Створення колекції об'єктів (Поліморфізм)
            // Ми кладемо різні інструменти в один список типу Instrument
            List<Instrument> orchestra = new List<Instrument>
            {
                new Guitar("Fender Stratocaster", 4, 6),
                new Piano("Yamaha", 5, "Рояль"),
                new Guitar("Gibson Les Paul", 3, 6),
                new Piano("Casio", 4, "Синтезатор")
            };

            // 2. Обчислення
            int totalDuration = 0;
            int songsCount = 0;

            Console.WriteLine("--- Початок концерту ---\n");

            // Проходимось по списку. Кожен об'єкт викличе СВІЙ метод Play
            foreach (var instrument in orchestra)
            {
                instrument.Play(); // Поліморфний виклик
                
                totalDuration += instrument.SongDurationMinutes;
                songsCount++;
            }

            Console.WriteLine("\n--- Статистика концерту ---");
            Console.WriteLine($"Кількість зіграних композицій: {songsCount}");
            Console.WriteLine($"Загальна тривалість концерту: {totalDuration} хвилин");
        }
    }
}