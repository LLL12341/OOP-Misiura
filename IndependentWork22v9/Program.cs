using System;
using System.Collections.Generic;

namespace IndependentWork22
{
    public interface IComponent
    {
        void SendPacket(string data);
    }

    public class Computer : IComponent
    {
        public string IPAddress { get; set; }

        public Computer(string ipAddress)
        {
            IPAddress = ipAddress;
        }

        public void SendPacket(string data)
        {
            Console.WriteLine($"  [Computer {IPAddress}] Отримав/Відправив дані: {data}");
        }
    }

    public class Router : IComponent
    {
        public string IPAddress { get; set; }

        public Router(string ipAddress)
        {
            IPAddress = ipAddress;
        }

        public void SendPacket(string data)
        {
            Console.WriteLine($"  [Router {IPAddress}] Маршрутизує пакет: {data}");
        }
    }

    public class Network : IComponent
    {
        public string Name { get; set; }
        private List<IComponent> _devices = new List<IComponent>();

        public Network(string name)
        {
            Name = name;
        }

        public void Add(IComponent component)
        {
            _devices.Add(component);
        }

        public void Remove(IComponent component)
        {
            _devices.Remove(component);
        }

        public void SendPacket(string data)
        {
            Console.WriteLine($"\n--- Мережа '{Name}' розсилає пакет ---");
            foreach (var device in _devices)
            {
                device.SendPacket(data);
            }
        }
    }
    public abstract class NetworkDecorator : IComponent
    {
        protected IComponent _wrapper;

        public NetworkDecorator(IComponent wrapper)
        {
            _wrapper = wrapper;
        }

        public virtual void SendPacket(string data)
        {
            _wrapper.SendPacket(data);
        }
    }
    
    // Декоратор шифрування
    public class EncryptionDecorator : NetworkDecorator
    {
        public EncryptionDecorator(IComponent wrapper) : base(wrapper) { }

        public override void SendPacket(string data)
        {
            // Імітація шифрування перед відправкою
            string encryptedData = $"***ENCRYPTED_{Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(data))}***";
            Console.WriteLine("    [🔐 Шифрування] Дані зашифровано.");
            base.SendPacket(encryptedData);
        }
    }

    // Декоратор логування
    public class LoggingDecorator : NetworkDecorator
    {
        public LoggingDecorator(IComponent wrapper) : base(wrapper) { }

        public override void SendPacket(string data)
        {
            Console.WriteLine($"    [📝 ЛОГ {DateTime.Now:HH:mm:ss}] Початок передачі пакета...");
            base.SendPacket(data);
            Console.WriteLine($"    [📝 ЛОГ {DateTime.Now:HH:mm:ss}] Передачу пакета успішно завершено.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== ЕТАП 1: Створення пристроїв (Leaf) ===");
            var pc1 = new Computer("192.168.0.101");
            var pc2 = new Computer("192.168.0.102");
            var router1 = new Router("192.168.0.1");

            // Надсилаємо пакети напряму (недекоровані)
            pc1.SendPacket("Hello World!");
            router1.SendPacket("Routing Info");

            Console.WriteLine("\n=== ЕТАП 2: Створення мережі (Composite) ===");
            var localNetwork = new Network("LAN Office");
            localNetwork.Add(pc1);
            localNetwork.Add(pc2);
            localNetwork.Add(router1);

            // Розсилаємо пакет по всій мережі
            localNetwork.SendPacket("Broadcast Message");

            Console.WriteLine("\n=== ЕТАП 3: Використання Декораторів ===");
            
            // 3.1. Декоруємо окремий комп'ютер (тільки шифрування)
            IComponent securePc = new EncryptionDecorator(new Computer("10.0.0.5"));
            Console.WriteLine("\nВідправка з захищеного ПК:");
            securePc.SendPacket("Top Secret Password");

            // 3.2. Нашаровуємо декоратори: Спочатку шифруємо, потім логуємо
            IComponent secureAndLoggedPc = new LoggingDecorator(securePc);
            Console.WriteLine("\nВідправка з захищеного ПК (з логуванням):");
            secureAndLoggedPc.SendPacket("Financial Report");

            // 3.3. Декоруємо ЦІЛУ МЕРЕЖУ (Composite + Decorator)
            Console.WriteLine("\nВідправка зашифрованого пакету по всій мережі:");
            IComponent secureNetwork = new EncryptionDecorator(localNetwork);
            secureNetwork.SendPacket("Global Update Payload");

            Console.WriteLine("\nРоботу завершено. Натисніть будь-яку клавішу...");
            Console.ReadKey();
        }
    }
}