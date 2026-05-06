using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace lab28v10
{
    // Клас 1: Учасник
    public class Participant
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"{FirstName} {LastName} ({Email})";
        }
    }

    // Клас 2: Місце проведення
    public class Location
    {
        public string City { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"{City}, {Address}";
        }
    }

    // Клас 3: Івент (Подія)
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public Location EventLocation { get; set; } = new Location();
        public List<Participant> Participants { get; set; } = new List<Participant>();

        public override string ToString()
        {
            return $"[{Id}] {Title} | Дата: {Date.ToShortDateString()} | Місце: {EventLocation} | Учасників: {Participants.Count}";
        }
    }

    // Клас 4: Репозиторій
    public class EventRepository
    {
        private List<Event> _events;

        public EventRepository()
        {
            _events = new List<Event>();
        }

        public void Add(Event newEvent)
        {
            _events.Add(newEvent);
        }

        public List<Event> GetAll()
        {
            return _events;
        }

        public Event? GetById(int id)
        {
            return _events.FirstOrDefault(e => e.Id == id);
        }

        public async Task SaveToFileAsync(string filename)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            using FileStream createStream = File.Create(filename);
            await JsonSerializer.SerializeAsync(createStream, _events, options);
        }

        public async Task LoadFromFileAsync(string filename)
        {
            if (!File.Exists(filename))
            {
                _events = new List<Event>();
                return;
            }

            using FileStream openStream = File.OpenRead(filename);
            var loadedEvents = await JsonSerializer.DeserializeAsync<List<Event>>(openStream);
            
            if (loadedEvents != null)
            {
                _events = loadedEvents;
            }
        }
    }

    // Точка входу
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            string filePath = "events.json";

            Console.WriteLine("=== Етап 1: Створення даних та збереження у JSON ===\n");
            
            EventRepository originalRepo = new EventRepository();

            var p1 = new Participant { Id = 1, FirstName = "Іван", LastName = "Франко", Email = "ivan.franko@example.com" };
            var p2 = new Participant { Id = 2, FirstName = "Леся", LastName = "Українка", Email = "lesya.ukr@example.com" };
            var p3 = new Participant { Id = 3, FirstName = "Тарас", LastName = "Шевченко", Email = "taras.shevchenko@example.com" };

            var event1 = new Event
            {
                Id = 101,
                Title = "Конференція IT-розробників",
                Date = new DateTime(2023, 11, 15),
                EventLocation = new Location { City = "Київ", Address = "вул. Хрещатик, 1" },
                Participants = { p1, p2 }
            };

            var event2 = new Event
            {
                Id = 102,
                Title = "Майстер-клас з C#",
                Date = new DateTime(2023, 12, 05),
                EventLocation = new Location { City = "Львів", Address = "вул. Франка, 15" },
                Participants = { p1, p3 }
            };

            originalRepo.Add(event1);
            originalRepo.Add(event2);

            await originalRepo.SaveToFileAsync(filePath);
            Console.WriteLine($"[Успіх] Дані збережено у файл: {filePath}\n");

            Console.WriteLine("=== Етап 2: Завантаження даних з JSON у новий репозиторій ===\n");
            
            EventRepository loadedRepo = new EventRepository();
            await loadedRepo.LoadFromFileAsync(filePath);

            var allEvents = loadedRepo.GetAll();
            Console.WriteLine("Завантажені Івенти:");
            foreach (var ev in allEvents)
            {
                Console.WriteLine(ev.ToString());
                Console.WriteLine("Список учасників:");
                foreach (var participant in ev.Participants)
                {
                    Console.WriteLine($" - {participant}");
                }
                Console.WriteLine();
            }

            Console.WriteLine("=== Етап 3: Демонстрація пошуку за ID (GetById) ===\n");
            int searchId = 102;
            var foundEvent = loadedRepo.GetById(searchId);
            if (foundEvent != null)
            {
                Console.WriteLine($"Знайдено подію з ID {searchId}: {foundEvent.Title}");
            }
            else
            {
                Console.WriteLine($"Подію з ID {searchId} не знайдено.");
            }

            Console.WriteLine("\nРоботу завершено. Натисніть будь-яку клавішу...");
            Console.ReadKey();
        }
    }
}