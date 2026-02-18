# Домашня робота №3: Принципи ISP та DIP

Виконав: Місюра Владислав
Група: ІПЗ-3/1

---

## 1. Принцип розділення інтерфейсу (ISP — Interface Segregation Principle)

Принцип ISP стверджує: "Клієнти не повинні залежати від методів, які вони не використовують". Замість одного великого (товстого) інтерфейсу краще створити кілька вузькоспеціалізованих.

### Приклад порушення ISP
Припустимо, у нас є інтерфейс для багатофункціонального пристрою (БФП).

```csharp
public interface IMultiFunctionDevice {
    void Print();
    void Scan();
    void Fax();
}

public class SimplePrinter : IMultiFunctionDevice {
    public void Print() => Console.WriteLine("Друк документів...");
    
    // Порушення: звичайний принтер не вміє сканувати чи надсилати факс
    public void Scan() => throw new NotImplementedException();
    public void Fax() => throw new NotImplementedException();
}