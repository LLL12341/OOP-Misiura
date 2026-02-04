using System;
using System.Collections.Generic;

namespace IndependentWork16
{

    public class BlogPost
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }

    public class BadBlogPostService
    {
        public void CreatePost(BlogPost post)
        {
            if (string.IsNullOrEmpty(post.Title)) 
                throw new Exception("Title is required");
            Console.WriteLine("Post validated.");

            Console.WriteLine($"Saving post '{post.Title}' to Database...");

            Console.WriteLine($"Publishing '{post.Title}' to Facebook and Twitter...");
        }
    }

    public interface IPostValidator
    {
        bool IsValid(BlogPost post);
    }

    public interface IPostRepository
    {
        void Save(BlogPost post);
    }

    public interface ISocialMediaPublisher
    {
        void Publish(BlogPost post);
    }

    public class PostValidator : IPostValidator
    {
        public bool IsValid(BlogPost post) => !string.IsNullOrEmpty(post.Title);
    }

    public class SqlPostRepository : IPostRepository
    {
        public void Save(BlogPost post) => 
            Console.WriteLine($"[DB] Пост '{post.Title}' успішно збережено в SQL базу даних.");
    }

    public class SocialMediaPublisher : ISocialMediaPublisher
    {
        public void Publish(BlogPost post) => 
            Console.WriteLine($"[Social] Пост '{post.Title}' опубліковано у Facebook та Instagram.");
    }


    public class BlogPostService
    {
        private readonly IPostValidator _validator;
        private readonly IPostRepository _repository;
        private readonly ISocialMediaPublisher _publisher;

        public BlogPostService(IPostValidator validator, IPostRepository repository, ISocialMediaPublisher publisher)
        {
            _validator = validator;
            _repository = repository;
            _publisher = publisher;
        }

        public void ProcessPost(BlogPost post)
        {
            if (!_validator.IsValid(post))
            {
                Console.WriteLine("Помилка: Невалідний пост!");
                return;
            }

            _repository.Save(post);
            _publisher.Publish(post);
            Console.WriteLine("Процес створення посту завершено успішно.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var myPost = new BlogPost { Id = 1, Title = "Мій перший пост", Content = "Привіт, світ!" };


            IPostValidator validator = new PostValidator();
            IPostRepository repository = new SqlPostRepository();
            ISocialMediaPublisher publisher = new SocialMediaPublisher();

            var postService = new BlogPostService(validator, repository, publisher);

            Console.WriteLine("--- Початок обробки посту ---");
            postService.ProcessPost(myPost);

            Console.WriteLine("\nНатисніть будь-яку клавішу для завершення...");
            Console.ReadKey();
        }
    }
}