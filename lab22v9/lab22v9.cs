using System;
using System.Text;

namespace lab22
{
    // --- ПОРУШЕННЯ LSP ---
    public class MediaPlayer
    {
        public virtual void PlayAudio() => Console.WriteLine("Відтворення аудіо...");
        public virtual void PlayVideo() => Console.WriteLine("Відтворення відео...");
    }

    public class AudioOnlyPlayer : MediaPlayer
    {
        public override void PlayVideo()
        {
            throw new NotSupportedException("LSP Violation: AudioOnlyPlayer cannot play video!");
        }
    }

    // --- РЕФАКТОРИНГ (LSP-сумісне рішення) ---
    public interface IAudioPlayer
    {
        void PlayAudio();
    }

    public interface IVideoPlayer : IAudioPlayer
    {
        void PlayVideo();
    }

    public class SimpleAudioPlayer : IAudioPlayer
    {
        public void PlayAudio() => Console.WriteLine("Аудіо: Відтворення звукового потоку...");
    }

    public class FullMediaPlayer : IVideoPlayer
    {
        public void PlayAudio() => Console.WriteLine("Медіа: Відтворення аудіо-доріжки...");
        public void PlayVideo() => Console.WriteLine("Медіа: Відтворення відео-потоку...");
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine("=== 1. Демонстрація порушення LSP ===");
            MediaPlayer badPlayer = new AudioOnlyPlayer();
            try
            {
                badPlayer.PlayAudio();
                badPlayer.PlayVideo(); // Викличе Exception
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка (Runtime): {ex.Message}");
            }

            Console.WriteLine("\n=== 2. Демонстрація LSP-сумісного рішення ===");
            IAudioPlayer audioOnly = new SimpleAudioPlayer();
            IVideoPlayer videoAndAudio = new FullMediaPlayer();

            // Клієнтський код працює стабільно через інтерфейси
            ExecuteAudio(audioOnly);
            ExecuteAudio(videoAndAudio); // Підстановка працює коректно
            ExecuteVideo(videoAndAudio);

            Console.WriteLine("\nПрограма завершена успішно.");
        }

        static void ExecuteAudio(IAudioPlayer player) => player.PlayAudio();
        static void ExecuteVideo(IVideoPlayer player) => player.PlayVideo();
    }
}