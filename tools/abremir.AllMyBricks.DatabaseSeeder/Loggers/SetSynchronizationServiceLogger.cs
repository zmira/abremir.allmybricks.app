﻿using System;
using abremir.AllMyBricks.DatabaseSeeder.Configuration;
using abremir.AllMyBricks.DatabaseSeeder.Enumerations;
using abremir.AllMyBricks.DataSynchronizer.Events.SetSynchronizationService;
using abremir.AllMyBricks.DataSynchronizer.Services;
using Easy.MessageHub;
using Microsoft.Extensions.Logging;

namespace abremir.AllMyBricks.DatabaseSeeder.Loggers
{
    public class SetSynchronizationServiceLogger : IDatabaseSeederLogger
    {
        public SetSynchronizationServiceLogger(
            ILoggerFactory loggerFactory,
            IMessageHub messageHub)
        {
            var logger = loggerFactory.CreateLogger<SetSynchronizationService>();

            messageHub.Subscribe<SetSynchronizationServiceStart>(_ => logger.LogInformation($"Started set synchronization{(Logging.LogDestination == LogDestination.Console ? $" {DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss}" : string.Empty)}"));

            messageHub.Subscribe<InsightsAcquired>(message => logger.LogInformation($"Last Updated: {(message.SynchronizationTimestamp.HasValue ? message.SynchronizationTimestamp.Value.ToString("yyyy-MM-dd HH:mm:ss") : "Never")}"));

            messageHub.Subscribe<SetSynchronizationServiceException>(message => logger.LogError(message.Exception, "Set Synchronization Exception"));

            messageHub.Subscribe<SetSynchronizationServiceEnd>(_ => logger.LogInformation($"Finished set synchronization{(Logging.LogDestination == LogDestination.Console ? $" {DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss}" : string.Empty)}"));
        }
    }
}
