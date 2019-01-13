using abremir.AllMyBricks.DatabaseSeeder.Configuration;
using Microsoft.Extensions.Logging;

namespace abremir.AllMyBricks.DatabaseSeeder
{
    class Program
    {
        private static ILogger Logger;

        static void Main(string[] args)
        {
            Logging.Configure();

            Logger = Logging.CreateLogger<Program>();

            Logger.LogInformation("Starting database seeder...");

            IoC.Configure();
            IoC.ConfigureOnboarding(Settings.OnboardingUrl);
                var fileSystem = IoC.IoCContainer.GetInstance<IFileSystemService>();
                fileSystem.EnsureLocalDataFolder();

            InteractiveConsole.Run();
        }
    }
}