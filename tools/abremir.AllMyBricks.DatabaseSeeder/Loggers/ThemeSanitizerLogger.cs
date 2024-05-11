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

            messageHub.Subscribe<ThemeSanitizerStart>(message =>
            {
                if (Logging.LogVerbosity is LogVerbosity.Full)
                {
                    logger.LogInformation("Started theme sanitizer");
                }
            });

            messageHub.Subscribe<ThemeSanitizerException>(message => logger.LogError(message.Exception, "Theme Sanitizer Exception"));

            messageHub.Subscribe<ThemeSanitizerEnd>(_ =>
            {
                if (Logging.LogVerbosity is LogVerbosity.Full)
                {
                    logger.LogInformation("Finished theme sanitizer");
                }
            });

            messageHub.Subscribe<DeletingThemesStart>(message =>
            {
                if (Logging.LogVerbosity is LogVerbosity.Full)
                {
                    logger.LogInformation($"Started deleting themes: {string.Join(", ", message.AffectedThemes)}");
                }
            });

            messageHub.Subscribe<DeletingSubthemesStart>(message =>
            {
                if (Logging.LogVerbosity is LogVerbosity.Full)
                {
                    logger.LogInformation($"Started deleting subthemes for theme '{message.AffectedTheme}': {string.Join(", ", message.AffectedSubthemes)}");
                }
            });

            messageHub.Subscribe<DeletingSubthemesEnd>(message =>
            {
                if (Logging.LogVerbosity is LogVerbosity.Full)
                {
                    logger.LogInformation($"Finished deleting subthemes for theme '{message.AffectedTheme}': {string.Join(", ", message.AffectedSubthemes)}");
                }
            });

            messageHub.Subscribe<DeletingThemesEnd>(message =>
            {
                if (Logging.LogVerbosity is LogVerbosity.Full)
                {
                    logger.LogInformation($"Finished deleting themes: {string.Join(", ", message.AffectedThemes)}");
                }
            });
        }
    }
}
