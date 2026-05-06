using System;

namespace lab31v1;

// Модель замовлення
public class Order
{
    public int Id { get; set; }
    public string CustomerEmail { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public bool IsProcessed { get; set; }
}

// Інтерфейс 1: Робота з базою даних
public interface IOrderRepository
{
    bool SaveOrder(Order order);
    Order? GetOrder(int id);
}

// Інтерфейс 2: Відправка email
public interface IEmailService
{
    void SendConfirmation(string email, string message);
}

// Головний сервіс, який ми будемо тестувати
public class OrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IEmailService _emailService;

    // Впровадження залежностей (Dependency Injection) через конструктор
    public OrderService(IOrderRepository orderRepository, IEmailService emailService)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
    }

    public bool PlaceOrder(Order order)
    {
        if (order == null || order.TotalAmount <= 0) return false;

        bool isSaved = _orderRepository.SaveOrder(order);
        
        if (isSaved)
        {
            order.IsProcessed = true;
            _emailService.SendConfirmation(order.CustomerEmail, $"Order {order.Id} placed successfully.");
            return true;
        }

        return false;
    }

    public string GetOrderStatus(int orderId)
    {
        var order = _orderRepository.GetOrder(orderId);
        if (order == null) return "Not Found";
        
        return order.IsProcessed ? "Processed" : "Pending";
    }
}