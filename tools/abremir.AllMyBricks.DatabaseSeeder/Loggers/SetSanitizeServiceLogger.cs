using System;
using abremir.AllMyBricks.DatabaseSeeder.Configuration;
using abremir.AllMyBricks.DatabaseSeeder.Enumerations;
using abremir.AllMyBricks.DataSynchronizer.Events.SetSanitizeService;
using abremir.AllMyBricks.DataSynchronizer.Services;
using Easy.MessageHub;
using Microsoft.Extensions.Logging;

namespace abremir.AllMyBricks.DatabaseSeeder.Loggers
{
    public class SetSanitizeServiceLogger : IDatabaseSeederLogger
    {
        public SetSanitizeServiceLogger(
            ILoggerFactory loggerFactory,
            IMessageHub messageHub)
        {
            var logger = loggerFactory.CreateLogger<SetSanitizeService>();

            messageHub.Subscribe<SetSanitizeServiceStart>(_ => logger.LogInformation($"Started set sanitize{(Logging.LogDestination is LogDestination.Console ? $" {DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss}" : string.Empty)}"));

            messageHub.Subscribe<SetSanitizeServiceException>(message => logger.LogError(message.Exception, "Set Sanitize Exception"));

            messageHub.Subscribe<SetSanitizeServiceEnd>(_ => logger.LogInformation($"Finished set sanitize{(Logging.LogDestination is LogDestination.Console ? $" {DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss}" : string.Empty)}"));
        }
    }
}
