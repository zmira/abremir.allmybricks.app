using abremir.AllMyBricks.DatabaseSeeder.Configuration;
using abremir.AllMyBricks.DataSynchronizer.Events.UserSynchronizationService;
using abremir.AllMyBricks.DataSynchronizer.Services;
using Easy.MessageHub;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace abremir.AllMyBricks.DatabaseSeeder.Loggers
{
    public class UserSynchronizationServiceLogger : IDatabaseSeederLogger
    {
        public UserSynchronizationServiceLogger(
            ILoggerFactory loggerFactory,
            IMessageHub messageHub)
        {
            var logger = loggerFactory.CreateLogger<UserSynchronizationService>();

            messageHub.Subscribe<UserSynchronizationServiceStart>(ev => logger.LogInformation($"{ev.UserType} User Synchronization Started{(Logging.LogDestination == LogDestinationEnum.Console ? $" {DateTimeOffset.Now.ToString("yyyy-MM-dd hh:mm:ss")}" : string.Empty)}"));

            messageHub.Subscribe<UsersAcquired>(ev => logger.LogInformation($"Synchronizing {ev.Count} {ev.UserType} Users"));

            messageHub.Subscribe<UserSynchronizationServiceException>(ev => ev.Exceptions.ToList().ForEach(exception => logger.LogError(exception, $"{ev.UserType} User Synchronization Exception")));

            messageHub.Subscribe<UserSynchronizationServiceEnd>(ev => logger.LogInformation($"Finished {ev.UserType} User Synchronization{(Logging.LogDestination == LogDestinationEnum.Console ? $" {DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss")}" : string.Empty)}"));
        }
    }
}
