using abremir.AllMyBricks.DatabaseSeeder.Configuration;
using abremir.AllMyBricks.DatabaseSeeder.Enumerations;
using abremir.AllMyBricks.DataSynchronizer.Events.ThemeSanitizer;
using Easy.MessageHub;
using Microsoft.Extensions.Logging;

namespace abremir.AllMyBricks.DatabaseSeeder.Loggers
{
    internal class ThemeSanitizerLogger : IDatabaseSeederLogger
    {
        public ThemeSanitizerLogger(
            ILoggerFactory loggerFactory,
            IMessageHub messageHub)
        {
            var logger = loggerFactory.CreateLogger<ThemeSanitizerLogger>();

            messageHub.Subscribe<ThemeSanitizerStart>(_ => logger.LogInformation("Started theme sanitizer"));

            messageHub.Subscribe<ThemeSanitizerException>(message => logger.LogError(message.Exception, "Theme Sanitizer Exception"));

            messageHub.Subscribe<ThemeSanitizerEnd>(_ => logger.LogInformation("Finished theme sanitizer"));

            messageHub.Subscribe<DeletingThemesStart>(message => logger.LogInformation("Started deleting themes: {Themes}", string.Join(", ", message.AffectedThemes)));

            messageHub.Subscribe<DeletingSubthemesStart>(message => logger.LogInformation("Started deleting subthemes for theme '{Theme}': {Subthemes}", message.AffectedTheme, string.Join(", ", message.AffectedSubthemes)));

            messageHub.Subscribe<DeletingSubthemesEnd>(message =>
            {
                if (Logging.LogVerbosity is LogVerbosity.Full)
                {
                    logger.LogInformation("Finished deleting subthemes for theme '{Theme}': {Subthemes}", message.AffectedTheme, string.Join(", ", message.AffectedSubthemes));
                }
            });

            messageHub.Subscribe<DeletingThemesEnd>(message =>
            {
                if (Logging.LogVerbosity is LogVerbosity.Full)
                {
                    logger.LogInformation("Finished deleting themes: {Themes}", string.Join(", ", message.AffectedThemes));
                }
            });
        }
    }
}
