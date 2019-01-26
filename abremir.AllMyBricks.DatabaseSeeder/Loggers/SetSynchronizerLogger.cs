using abremir.AllMyBricks.DatabaseSeeder.Configuration;
using abremir.AllMyBricks.DataSynchronizer.Events.SetSynchronizer;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using abremir.AllMyBricks.DataSynchronizer.Synchronizers;
using Microsoft.Extensions.Logging;

using static System.FormattableString;

namespace abremir.AllMyBricks.DatabaseSeeder.Loggers
{
    public class SetSynchronizerLogger : IDatabaseSeederLogger
    {
        private static float _setIndex;
        private static float _setProgressFraction;
        private static float _setCount;

        private readonly ILogger _logger;

        public SetSynchronizerLogger(ILoggerFactory loggerFactory, IDataSynchronizerEventManager dataSynchronizerEventManager)
        {
            _logger = loggerFactory.CreateLogger<SetSynchronizer>();

            dataSynchronizerEventManager.Register<SetSynchronizerStart>(ev =>
            {
                _setIndex = 0;
                _setProgressFraction = 0;

                if (Logging.LogVerbosity == LogVerbosityEnum.FullLogging)
                {
                    _logger.LogInformation($"Set Synchronizer Started{(ev.ForSubtheme ? " for subtheme" : string.Empty)}");
                }
            });

            dataSynchronizerEventManager.Register<AcquiringSets>(ev =>
            {
                _setIndex = 0;
                _setProgressFraction = 0;

                if (Logging.LogVerbosity == LogVerbosityEnum.FullLogging)
                {
                    _logger.LogInformation($"Acquiring sets from '{ev.Theme}-{ev.Subtheme}' to process for year {ev.Year}");
                }
            });

            dataSynchronizerEventManager.Register<SetsAcquired>(ev =>
            {
                _setCount = ev.Count;
                _setIndex = 0;

                _logger.LogInformation($"Acquired {ev.Count} sets {(ev.Year.HasValue ? $"from '{ev.Theme}-{ev.Subtheme}' " : string.Empty)}to process{(ev.Year.HasValue ? $" for year {ev.Year}" : string.Empty)}");
            });

            dataSynchronizerEventManager.Register<SynchronizingSet>(ev =>
            {
                _setIndex++;
                _setProgressFraction = _setIndex / _setCount;

                if(Logging.LogVerbosity == LogVerbosityEnum.FullLogging)
                {
                    _logger.LogInformation(Invariant($"Synchronizing Set '{ev.IdentifierLong}': index {_setIndex}, progress {_setProgressFraction:##0.00%}"));
                }
            });

            dataSynchronizerEventManager.Register<SynchronizingSetException>(ev => _logger.LogError(ev.Exception, $"Synchronizing Set '{ev.IdentifierLong}' Exception"));

            dataSynchronizerEventManager.Register<SynchronizedSet>(ev =>
            {
                if (Logging.LogVerbosity == LogVerbosityEnum.FullLogging)
                {
                    _logger.LogInformation($"Finished Synchronizing Set '{ev.IdentifierLong}'");
                }
            });

            dataSynchronizerEventManager.Register<SetSynchronizerException>(ev => _logger.LogError(ev.Exception, "Set Synchronizer Exception"));

            dataSynchronizerEventManager.Register<SetSynchronizerEnd>(ev =>
            {
                _setIndex = 0;
                _setProgressFraction = 0;

                if (Logging.LogVerbosity == LogVerbosityEnum.FullLogging)
                {
                    _logger.LogInformation($"Finished Set Synchronizer {(ev.ForSubtheme ? " for subtheme" : string.Empty)}");
                }
            });
        }
    }
}