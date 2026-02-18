# Домашня Робота №2: Принцип підстановки Лісков (LSP)

Виконав: Місюра Владислав
Група: ІПЗ-3/1

---

## 1. Птахи, що не літають (Порушення через NotImplementedException)

Це найпоширеніше порушення, коли підклас не може виконати дію, яку обіцяє базовий клас.

### Код з помилкою
```csharp
public class Bird {
    public virtual void Fly() => Console.WriteLine("Птах летить");
}

public class Ostrich : Bird {
    public override void Fly() {
        // Порушення LSP: клієнт не очікує тут виключення
        throw new NotSupportedException("Страуси не літають!");
    }
}