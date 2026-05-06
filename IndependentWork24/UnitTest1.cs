using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace IndependentWork24
{
    // ==========================================
    // СИСТЕМА ДЛЯ ТЕСТУВАННЯ (SUT)
    // ==========================================

    // 1. СПІЛЬНИЙ ІНТЕРФЕЙС
    public interface IFileSystemItem
    {
        string Name { get; }
        int GetSize();
        string Read();
    }

    // ==========================================
    // 2. COMPOSITE (Листок та Композит)
    // ==========================================
    
    // Leaf (Окремий файл)
    public class FileItem : IFileSystemItem
    {
        public string Name { get; }
        private readonly string _content;

        public FileItem(string name, string content)
        {
            Name = name;
            _content = content;
        }

        public int GetSize() => _content.Length;
        public string Read() => _content;
    }

    // Composite (Папка, яка може містити файли або інші папки)
    public class FolderItem : IFileSystemItem
    {
        public string Name { get; }
        private readonly List<IFileSystemItem> _children = new();

        public FolderItem(string name) => Name = name;

        public void Add(IFileSystemItem item) => _children.Add(item);

        public int GetSize() => _children.Sum(c => c.GetSize());
        
        public string Read() => $"[Папка: {Name}] містить {_children.Count} елементів.";
    }

    // ==========================================
    // 3. DECORATOR (Додавання функціоналу)
    // ==========================================
    
    // Декоратор, який імітує стиснення файлу/папки
    public class ZipDecorator : IFileSystemItem
    {
        private readonly IFileSystemItem _wrapper;

        public ZipDecorator(IFileSystemItem wrapper)
        {
            _wrapper = wrapper;
        }

        public string Name => $"{_wrapper.Name}.zip";

        // Імітуємо стиснення розміру вдвічі
        public int GetSize() => _wrapper.GetSize() / 2;

        public string Read() => $"[UNZIP...] {_wrapper.Read()}";
    }

    // ==========================================
    // 4. PROXY (Контроль доступу)
    // ==========================================
    
    // Проксі-обгортка для захисту доступу до файлової системи
    public class SecureProxy : IFileSystemItem
    {
        private readonly IFileSystemItem _realSubject;
        private readonly string _userRole;

        public SecureProxy(IFileSystemItem realSubject, string userRole)
        {
            _realSubject = realSubject;
            _userRole = userRole;
        }

        public string Name => _realSubject.Name;

        public int GetSize() => _realSubject.GetSize(); // Розмір можуть бачити всі

        public string Read()
        {
            // Читати контент можуть лише адміністратори
            if (_userRole != "Admin")
            {
                throw new UnauthorizedAccessException($"Доступ заборонено для ролі: {_userRole}");
            }
            return _realSubject.Read();
        }
    }


    // ==========================================
    // ІНТЕГРАЦІЙНІ ТЕСТИ (xUnit)
    // ==========================================
    public class FileSystemIntegrationTests
    {
        [Fact]
        public void Test1_Composite_CalculatesTotalSizeCorrectly()
        {
            // Arrange (Позитивний тест: перевірка Композиту)
            var folder = new FolderItem("Root");
            folder.Add(new FileItem("file1.txt", "12345")); // Розмір 5
            folder.Add(new FileItem("file2.txt", "1234567890")); // Розмір 10

            // Act
            int totalSize = folder.GetSize();

            // Assert
            Assert.Equal(15, totalSize);
        }

        [Fact]
        public void Test2_Decorator_CompressesSizeAndAltersRead()
        {
            // Arrange (Позитивний тест: перевірка Декоратора)
            var file = new FileItem("data.txt", "1234567890"); // Розмір 10
            var zippedFile = new ZipDecorator(file);

            // Act & Assert
            Assert.Equal("data.txt.zip", zippedFile.Name);
            Assert.Equal(5, zippedFile.GetSize()); // Розмір має бути зменшений вдвічі
            Assert.StartsWith("[UNZIP...]", zippedFile.Read());
        }

        [Fact]
        public void Test3_Integration_ProxyWithCompositeAndDecorator_AdminAccess()
        {
            // Arrange (Позитивний тест: Інтеграція 3-х патернів)
            var folder = new FolderItem("SecretDocs");
            folder.Add(new FileItem("passwords.txt", "admin:1234;"));
            
            // Загортаємо папку в архів (Decorator), а потім захищаємо її паролем (Proxy)
            IFileSystemItem zippedFolder = new ZipDecorator(folder);
            IFileSystemItem secureArchive = new SecureProxy(zippedFolder, "Admin");

            // Act
            int size = secureArchive.GetSize();
            string content = secureArchive.Read(); // Адмін має право

            // Assert
            Assert.True(size > 0);
            Assert.StartsWith("[UNZIP...]", content);
        }

        [Fact]
        public void Test4_Proxy_DeniesReadAccessForGuest()
        {
            // Arrange (Негативний тест / Граничний кейс: перевірка Proxy на відмову)
            var file = new FileItem("salary.txt", "10000$");
            var secureFile = new SecureProxy(file, "Guest");

            // Act
            // Розмір доступний
            int size = secureFile.GetSize();

            // Assert
            Assert.Equal(6, size);
            // Спроба прочитати викликає виключення
            var ex = Assert.Throws<UnauthorizedAccessException>(() => secureFile.Read());
            Assert.Contains("Доступ заборонено", ex.Message);
        }
    }
}