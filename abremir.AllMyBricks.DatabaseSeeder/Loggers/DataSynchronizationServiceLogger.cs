using abremir.AllMyBricks.DatabaseSeeder.Configuration;
using abremir.AllMyBricks.DataSynchronizer.Events.DataSynchronizationService;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using abremir.AllMyBricks.DataSynchronizer.Services;
using Microsoft.Extensions.Logging;
using System;

namespace abremir.AllMyBricks.DatabaseSeeder.Loggers
{
    public class DataSynchronizationServiceLogger : IDatabaseSeederLogger
    {
        private readonly ILogger _logger;

        public DataSynchronizationServiceLogger(ILoggerFactory loggerFactory, IDataSynchronizerEventManager dataSynchronizerEventManager)
        {
            _logger = loggerFactory.CreateLogger<DataSynchronizationService>();

            dataSynchronizerEventManager.Register<DataSynchronizationStart>(_ => _logger.LogInformation($"Data Synchronization Started{(Logging.LogDestination == LogDestinationEnum.Console ? $" {DateTimeOffset.Now.ToString("yyyy-MM-dd hh:mm:ss")}" : string.Empty)}"));

            dataSynchronizerEventManager.Register<InsightsAcquired>(ev => _logger.LogInformation($"Last Updated: {(ev.SynchronizationTimestamp.HasValue ? ev.SynchronizationTimestamp.Value.ToString("yyyy-MM-dd HH:mm:ss") : "Never")}"));

            dataSynchronizerEventManager.Register<ProcessingTheme>(ev =>
            {
                if (Logging.LogVerbosity == LogVerbosityEnum.FullLogging)
                {
                    _logger.LogInformation($"Processing theme: {ev.Name}");
                }
            });

            dataSynchronizerEventManager.Register<ProcessingSubtheme>(ev =>
            {
                if (Logging.LogVerbosity == LogVerbosityEnum.FullLogging)
                {
                    _logger.LogInformation($"Processing subtheme: {ev.Name}");
                }
            });

            dataSynchronizerEventManager.Register<ProcessedSubtheme>(ev =>
            {
                if (Logging.LogVerbosity == LogVerbosityEnum.FullLogging)
                {
                    _logger.LogInformation($"Finished processing subtheme: {ev.Name}");
                }
            });

            dataSynchronizerEventManager.Register<ProcessingThemeException>(ev => _logger.LogError(ev.Exception, $"Processing Theme '{ev.Name}' Exception"));

            dataSynchronizerEventManager.Register<ProcessedTheme>(ev =>
            {
                if (Logging.LogVerbosity == LogVerbosityEnum.FullLogging)
                {
                    _logger.LogInformation($"Finished processing theme: {ev.Name}");
                }
            });

            dataSynchronizerEventManager.Register<DataSynchronizationException>(ev => _logger.LogError(ev.Exception, "Data Synchronization Exception"));

            dataSynchronizerEventManager.Register<DataSynchronizationEnd>(_ => _logger.LogInformation($"Finished Data Synchronization{(Logging.LogDestination == LogDestinationEnum.Console ? $" {DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss")}" : string.Empty)}"));
        }
    }
}