using System;
using System.Linq;
using abremir.AllMyBricks.DatabaseSeeder.Configuration;
using abremir.AllMyBricks.DatabaseSeeder.Enumerations;
using abremir.AllMyBricks.DataSynchronizer.Events.UserSynchronizationService;
using abremir.AllMyBricks.DataSynchronizer.Services;
using Easy.MessageHub;
using Microsoft.Extensions.Logging;

namespace abremir.AllMyBricks.DatabaseSeeder.Loggers
{
    public class UserSynchronizationServiceLogger : IDatabaseSeederLogger
    {
        public UserSynchronizationServiceLogger(
            ILoggerFactory loggerFactory,
            IMessageHub messageHub)
        {
            var logger = loggerFactory.CreateLogger<UserSynchronizationService>();

            messageHub.Subscribe<UserSynchronizationServiceStart>(message => logger.LogInformation($"Started {message.UserType} user synchronization{(Logging.LogDestination == LogDestination.Console ? $" {DateTimeOffset.Now:yyyy-MM-dd hh:mm:ss}" : string.Empty)}"));

            messageHub.Subscribe<UsersAcquired>(message => logger.LogInformation($"Synchronizing {message.Count} {message.UserType} Users"));

            messageHub.Subscribe<UserSynchronizationServiceException>(message => message.Exceptions.ToList().ForEach(exception => logger.LogError(exception, $"{message.UserType} User Synchronization Exception")));

            messageHub.Subscribe<UserSynchronizationServiceEnd>(message => logger.LogInformation($"Finished {message.UserType} user synchronization{(Logging.LogDestination == LogDestination.Console ? $" {DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss}" : string.Empty)}"));
        }
    }
}
