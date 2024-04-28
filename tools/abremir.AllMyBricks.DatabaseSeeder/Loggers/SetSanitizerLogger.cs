using System.Text.Json;
using abremir.AllMyBricks.DatabaseSeeder.Configuration;
using abremir.AllMyBricks.DatabaseSeeder.Enumerations;
using abremir.AllMyBricks.DataSynchronizer.Events.SetSanitizer;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using Easy.MessageHub;
using Microsoft.Extensions.Logging;

namespace abremir.AllMyBricks.DatabaseSeeder.Loggers
{
    internal class SetSanitizerLogger : IDatabaseSeederLogger
    {
        public SetSanitizerLogger(
            ILoggerFactory loggerFactory,
            IMessageHub messageHub)
        {
            var logger = loggerFactory.CreateLogger<ISetSanitizer>();

            messageHub.Subscribe<SetSanitizerStart>(_ =>
            {
                if (Logging.LogVerbosity is LogVerbosity.Full)
                {
                    logger.LogInformation("Started set sanitizer");
                }
            });

            messageHub.Subscribe<SetSanitizerException>(message => logger.LogError(message.Exception, "Set Sanitizer Exception"));

            messageHub.Subscribe<SetSanitizerEnd>(_ =>
            {
                if (Logging.LogVerbosity is LogVerbosity.Full)
                {
                    logger.LogInformation("Finished set sanitizer");
                }
            });

            messageHub.Subscribe<AdjustingThemesWithDifferencesStart>(message =>
            {
                if (Logging.LogVerbosity is LogVerbosity.Full)
                {
                    logger.LogInformation($"Started adjusting themes with differences '{JsonSerializer.Serialize(message.AffectedThemes)}'");
                }
            });

            messageHub.Subscribe<AdjustingThemesWithDifferencesEnd>(message =>
            {
                if (Logging.LogVerbosity is LogVerbosity.Full)
                {
                    logger.LogInformation($"Finished adjusting themes with differences '{JsonSerializer.Serialize(message.AffectedThemes)}'");
                }
            });
        }
    }
}
