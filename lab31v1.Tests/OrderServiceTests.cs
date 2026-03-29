using Moq;
using Xunit;
using lab31v1;

namespace lab31v1.Tests;

public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _mockRepo;
    private readonly Mock<IEmailService> _mockEmail;
    private readonly OrderService _orderService;

    public OrderServiceTests()
    {
        // Створюємо моки для інтерфейсів
        _mockRepo = new Mock<IOrderRepository>();
        _mockEmail = new Mock<IEmailService>();
        
        // Передаємо моки в сервіс
        _orderService = new OrderService(_mockRepo.Object, _mockEmail.Object);
    }

    [Fact]
    public void PlaceOrder_ValidOrder_ReturnsTrue()
    {
        var order = new Order { Id = 1, CustomerEmail = "test@test.com", TotalAmount = 100 };
        // Setup: налаштовуємо мок поверляти true при збереженні
        _mockRepo.Setup(repo => repo.SaveOrder(order)).Returns(true);

        var result = _orderService.PlaceOrder(order);

        Assert.True(result);
    }

    [Fact]
    public void PlaceOrder_ValidOrder_SendsEmailConfirmation()
    {
        var order = new Order { Id = 2, CustomerEmail = "user@mail.com", TotalAmount = 50 };
        _mockRepo.Setup(repo => repo.SaveOrder(order)).Returns(true);

        _orderService.PlaceOrder(order);

        // Verify: перевіряємо, чи метод відправки email викликався рівно 1 раз
        _mockEmail.Verify(e => e.SendConfirmation("user@mail.com", "Order 2 placed successfully."), Times.Once);
    }

    [Fact]
    public void PlaceOrder_RepositoryFails_ReturnsFalse()
    {
        var order = new Order { Id = 3, CustomerEmail = "fail@mail.com", TotalAmount = 200 };
        // Setup: симулюємо збій у базі даних
        _mockRepo.Setup(repo => repo.SaveOrder(order)).Returns(false);

        var result = _orderService.PlaceOrder(order);

        Assert.False(result);
    }

    [Fact]
    public void PlaceOrder_RepositoryFails_DoesNotSendEmail()
    {
        var order = new Order { Id = 4, CustomerEmail = "noemail@mail.com", TotalAmount = 300 };
        _mockRepo.Setup(repo => repo.SaveOrder(order)).Returns(false);

        _orderService.PlaceOrder(order);

        // Verify: перевіряємо, що лист НЕ відправлявся
        _mockEmail.Verify(e => e.SendConfirmation(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void PlaceOrder_ZeroAmount_ReturnsFalse()
    {
        var order = new Order { Id = 5, CustomerEmail = "zero@mail.com", TotalAmount = 0 };

        var result = _orderService.PlaceOrder(order);

        Assert.False(result);
        // Verify: зберігання не повинно викликатись
        _mockRepo.Verify(r => r.SaveOrder(It.IsAny<Order>()), Times.Never);
    }

    [Fact]
    public void GetOrderStatus_OrderExistsAndProcessed_ReturnsProcessed()
    {
        var order = new Order { Id = 6, IsProcessed = true };
        // Setup: повертаємо конкретне замовлення по ID
        _mockRepo.Setup(repo => repo.GetOrder(6)).Returns(order);

        var status = _orderService.GetOrderStatus(6);

        Assert.Equal("Processed", status);
    }

    [Fact]
    public void GetOrderStatus_OrderExistsNotProcessed_ReturnsPending()
    {
        var order = new Order { Id = 7, IsProcessed = false };
        _mockRepo.Setup(repo => repo.GetOrder(7)).Returns(order);

        var status = _orderService.GetOrderStatus(7);

        Assert.Equal("Pending", status);
    }

    [Fact]
    public void GetOrderStatus_OrderDoesNotExist_ReturnsNotFound()
    {
        // Setup: повертаємо null, якщо замовлення не знайдено
        _mockRepo.Setup(repo => repo.GetOrder(99)).Returns((Order?)null);

        var status = _orderService.GetOrderStatus(99);

        Assert.Equal("Not Found", status);
    }
}