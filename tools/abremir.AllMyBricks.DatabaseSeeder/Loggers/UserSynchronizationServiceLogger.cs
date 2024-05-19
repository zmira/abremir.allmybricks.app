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

            messageHub.Subscribe<UserSynchronizationServiceStart>(message => logger.LogInformation("Started {Type} user synchronization{Timestamp}", message.UserType, Logging.LogDestination is LogDestination.Console ? $" {DateTimeOffset.Now:yyyy-MM-dd hh:mm:ss}" : string.Empty));

            messageHub.Subscribe<UsersAcquired>(message => logger.LogInformation("Synchronizing {Count} {Type} Users", message.Count, message.UserType));

            messageHub.Subscribe<UserSynchronizationServiceException>(message => message.Exceptions.ToList().ForEach(exception => logger.LogError(exception, "{Type} User Synchronization Exception", message.UserType)));

            messageHub.Subscribe<UserSynchronizationServiceEnd>(message => logger.LogInformation("Finished {Type} user synchronization{Timestamp}", message.UserType, Logging.LogDestination is LogDestination.Console ? $" {DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss}" : string.Empty));
        }
    }
}
