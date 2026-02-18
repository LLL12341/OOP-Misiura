using System;
namespace lab23
{

    public interface IPrinter
    {
        void Print(string document);
    }

    public interface IScanner
    {
        void Scan(string document);
    }

    // Пристрій, що вміє тільки друкувати
    public class BasicPrinter : IPrinter
    {
        public void Print(string document) => 
            Console.WriteLine($"[BasicPrinter] Друк документа: {document}");
    }

    // Багатофункціональний пристрій (БФП)
    public class MultiFunctionDevice : IPrinter, IScanner
    {
        public void Print(string document) => 
            Console.WriteLine($"[MFD] Високоякісний друк: {document}");

        public void Scan(string document) => 
            Console.WriteLine($"[MFD] Сканування: {document} у PDF");
    }

    public class OfficeWorkstation
    {
        private readonly IPrinter _printer;

        public OfficeWorkstation(IPrinter printer)
        {
            _printer = printer ?? throw new ArgumentNullException(nameof(printer));
        }

        public void Run(string docName)
        {
            Console.WriteLine($"--- Робота станції з документом '{docName}' ---");
            _printer.Print(docName);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // 1. Працюємо зі звичайним принтером
            IPrinter simplePrinter = new BasicPrinter();
            OfficeWorkstation station1 = new OfficeWorkstation(simplePrinter);
            station1.Run("Звіт_№1.docx");

            Console.WriteLine();

            // 2. Легко замінюємо принтер на БФП без зміни коду OfficeWorkstation
            IPrinter advancedPrinter = new MultiFunctionDevice();
            OfficeWorkstation station2 = new OfficeWorkstation(advancedPrinter);
            station2.Run("Презентація.pptx");

            // Демонстрація ISP: БФП можна використовувати і як сканер окремо
            if (advancedPrinter is IScanner scanner)
            {
                scanner.Scan("Контракт");
            }

            Console.WriteLine("\nРефакторинг завершено: ISP та DIP застосовано успішно.");
        }
    }
}