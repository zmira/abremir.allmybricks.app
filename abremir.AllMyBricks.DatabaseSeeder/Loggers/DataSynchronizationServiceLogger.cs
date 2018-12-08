﻿using abremir.AllMyBricks.DataSynchronizer.Events.DataSynchronizationService;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using abremir.AllMyBricks.DataSynchronizer.Services;
using Microsoft.Extensions.Logging;

namespace abremir.AllMyBricks.DatabaseSeeder.Loggers
{
    public class DataSynchronizationServiceLogger
    {
        private readonly ILogger _logger;

        public DataSynchronizationServiceLogger(ILoggerFactory loggerFactory, IDataSynchronizerEventManager dataSynchronizerEventManager)
        {
            _logger = loggerFactory.CreateLogger<DataSynchronizationService>();

            dataSynchronizerEventManager.Register<DataSynchronizationStart>(_ => _logger.LogInformation("Data Synchronization Started"));

            dataSynchronizerEventManager.Register<ProcessingTheme>(ev => _logger.LogInformation($"Processing theme: {ev.Name}"));

            dataSynchronizerEventManager.Register<ProcessingSubtheme>(ev => _logger.LogInformation($"Processing subtheme: {ev.Name}"));

            dataSynchronizerEventManager.Register<ProcessedSubtheme>(ev => _logger.LogInformation($"Finished processing subtheme: {ev.Name}"));

            dataSynchronizerEventManager.Register<ProcessingThemeException>(ev => _logger.LogError(ev.Exception, $"Processing Theme '{ev.Name}' Exception"));

            dataSynchronizerEventManager.Register<ProcessedTheme>(ev => _logger.LogInformation($"Finished processing theme: {ev.Name}"));

            dataSynchronizerEventManager.Register<DataSynchronizationException>(ev => _logger.LogError(ev.Exception, "Data Synchronization Exception"));

            dataSynchronizerEventManager.Register<DataSynchronizationEnd>(_ => _logger.LogInformation("Finished Data Synchronization"));
        }
    }
}