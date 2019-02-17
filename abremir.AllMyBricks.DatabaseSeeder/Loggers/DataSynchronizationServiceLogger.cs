using abremir.AllMyBricks.DatabaseSeeder.Configuration;
using abremir.AllMyBricks.DataSynchronizer.Events.DataSynchronizationService;
using abremir.AllMyBricks.DataSynchronizer.Services;
using Easy.MessageHub;
using Microsoft.Extensions.Logging;
using System;

namespace abremir.AllMyBricks.DatabaseSeeder.Loggers
{
    public class DataSynchronizationServiceLogger : IDatabaseSeederLogger
    {
        public DataSynchronizationServiceLogger(
            ILoggerFactory loggerFactory,
            IMessageHub messageHub)
        {
            var logger = loggerFactory.CreateLogger<DataSynchronizationService>();

            messageHub.Subscribe<DataSynchronizationStart>(_ => logger.LogInformation($"Data Synchronization Started{(Logging.LogDestination == LogDestinationEnum.Console ? $" {DateTimeOffset.Now.ToString("yyyy-MM-dd hh:mm:ss")}" : string.Empty)}"));

            messageHub.Subscribe<InsightsAcquired>(ev => logger.LogInformation($"Last Updated: {(ev.SynchronizationTimestamp.HasValue ? ev.SynchronizationTimestamp.Value.ToString("yyyy-MM-dd HH:mm:ss") : "Never")}"));

            messageHub.Subscribe<ProcessingTheme>(ev =>
            {
                if (Logging.LogVerbosity == LogVerbosityEnum.FullLogging)
                {
                    logger.LogInformation($"Processing theme: {ev.Name}");
                }
            });

            messageHub.Subscribe<ProcessingSubtheme>(ev =>
            {
                if (Logging.LogVerbosity == LogVerbosityEnum.FullLogging)
                {
                    logger.LogInformation($"Processing subtheme: {ev.Name}");
                }
            });

            messageHub.Subscribe<ProcessedSubtheme>(ev =>
            {
                if (Logging.LogVerbosity == LogVerbosityEnum.FullLogging)
                {
                    logger.LogInformation($"Finished processing subtheme: {ev.Name}");
                }
            });

            messageHub.Subscribe<ProcessingThemeException>(ev => logger.LogError(ev.Exception, $"Processing Theme '{ev.Name}' Exception"));

            messageHub.Subscribe<ProcessedTheme>(ev =>
            {
                if (Logging.LogVerbosity == LogVerbosityEnum.FullLogging)
                {
                    logger.LogInformation($"Finished processing theme: {ev.Name}");
                }
            });

            messageHub.Subscribe<DataSynchronizationException>(ev => logger.LogError(ev.Exception, "Data Synchronization Exception"));

            messageHub.Subscribe<DataSynchronizationEnd>(_ => logger.LogInformation($"Finished Data Synchronization{(Logging.LogDestination == LogDestinationEnum.Console ? $" {DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss")}" : string.Empty)}"));
        }
    }
}