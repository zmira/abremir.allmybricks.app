﻿using System.Text.Json;
using abremir.AllMyBricks.DatabaseSeeder.Configuration;
using abremir.AllMyBricks.DatabaseSeeder.Enumerations;
using abremir.AllMyBricks.DataSynchronizer.Events.SetSanitizer;
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
            var logger = loggerFactory.CreateLogger<SetSanitizerLogger>();

            messageHub.Subscribe<SetSanitizerStart>(_ => logger.LogInformation("Started set sanitizer"));

            messageHub.Subscribe<SetSanitizerException>(message => logger.LogError(message.Exception, "Set Sanitizer Exception"));

            messageHub.Subscribe<SetSanitizerEnd>(_ => logger.LogInformation("Finished set sanitizer"));

            messageHub.Subscribe<AdjustingThemesWithDifferencesStart>(message =>
            {
                if (Logging.LogVerbosity is LogVerbosity.Full)
                {
                    logger.LogInformation("Started adjusting themes with differences '{Themes}'", JsonSerializer.Serialize(message.AffectedThemes));
                }
            });

            messageHub.Subscribe<AdjustingThemesWithDifferencesEnd>(message =>
            {
                if (Logging.LogVerbosity is LogVerbosity.Full)
                {
                    logger.LogInformation("Finished adjusting themes with differences '{Themes}'", JsonSerializer.Serialize(message.AffectedThemes));
                }
            });
        }
    }
}
