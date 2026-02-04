using System;
using System.Text;

namespace lab20
{
    public enum OrderStatus
    {
        New, PendingValidation, Processed, Shipped, Delivered, Cancelled
    }

    public class Order
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Order(int id, string customerName, decimal totalAmount)
        {
            Id = id;
            CustomerName = customerName;
            TotalAmount = totalAmount;
            Status = OrderStatus.New;
        }
    }

    public interface IOrderValidator
    {
        bool IsValid(Order order);
    }

    public interface IOrderRepository
    {
        void Save(Order order);
        Order GetById(int id);
    }

    public interface IEmailService
    {
        void SendOrderConfirmation(Order order);
    }

    public class OrderValidator : IOrderValidator
    {
        public bool IsValid(Order order) => order.TotalAmount > 0;
    }

    public class InMemoryOrderRepository : IOrderRepository
    {
        public void Save(Order order)
        {
            Console.WriteLine($"[Database] Замовлення #{order.Id} для {order.CustomerName} успішно збережено.");
        }

        public Order GetById(int id) => null;
    }

    public class ConsoleEmailService : IEmailService
    {
        public void SendOrderConfirmation(Order order)
        {
            Console.WriteLine($"[Email] Сповіщення: Шановний(а) {order.CustomerName}, ваше замовлення на суму {order.TotalAmount} оброблено.");
        }
    }

    public class OrderService
    {
        private readonly IOrderValidator _validator;
        private readonly IOrderRepository _repository;
        private readonly IEmailService _emailService;

        public OrderService(IOrderValidator validator, IOrderRepository repository, IEmailService emailService)
        {
            _validator = validator;
            _repository = repository;
            _emailService = emailService;
        }

        public void ProcessOrder(Order order)
        {
            Console.WriteLine($"--- Обробка замовлення #{order.Id} ---");

            if (!_validator.IsValid(order))
            {
                Console.WriteLine($"[Результат] Помилка: Замовлення #{order.Id} має невалідну суму.");
                Console.WriteLine();
                return;
            }

            order.Status = OrderStatus.Processed;
            _repository.Save(order);
            _emailService.SendOrderConfirmation(order);

            Console.WriteLine($"[Результат] Замовлення #{order.Id} успішно завершено.\n");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            IOrderValidator validator = new OrderValidator();
            IOrderRepository repository = new InMemoryOrderRepository();
            IEmailService emailService = new ConsoleEmailService();

            var orderService = new OrderService(validator, repository, emailService);

            var validOrder = new Order(101, "Олексій", 2500.75m);
            orderService.ProcessOrder(validOrder);

            var invalidOrder = new Order(102, "Ірина", -50.00m);
            orderService.ProcessOrder(invalidOrder);

            Console.WriteLine("Натисніть будь-яку клавішу для виходу...");
            Console.ReadKey();
        }
    }
}