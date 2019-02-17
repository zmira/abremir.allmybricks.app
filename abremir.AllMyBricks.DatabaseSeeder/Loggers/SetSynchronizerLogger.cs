using abremir.AllMyBricks.DatabaseSeeder.Configuration;
using abremir.AllMyBricks.DataSynchronizer.Events.SetSynchronizer;
using abremir.AllMyBricks.DataSynchronizer.Synchronizers;
using Easy.MessageHub;
using Microsoft.Extensions.Logging;

using static System.FormattableString;

namespace abremir.AllMyBricks.DatabaseSeeder.Loggers
{
    public class SetSynchronizerLogger : IDatabaseSeederLogger
    {
        private static float _setIndex;
        private static float _setProgressFraction;
        private static float _setCount;

        public SetSynchronizerLogger(
            ILoggerFactory loggerFactory,
            IMessageHub messageHub)
        {
            var logger = loggerFactory.CreateLogger<SetSynchronizer>();

            messageHub.Subscribe<SetSynchronizerStart>(ev =>
            {
                _setIndex = 0;
                _setProgressFraction = 0;

                if (Logging.LogVerbosity == LogVerbosityEnum.FullLogging)
                {
                    logger.LogInformation($"Set Synchronizer Started{(ev.ForSubtheme ? " for subtheme" : string.Empty)}");
                }
            });

            messageHub.Subscribe<AcquiringSets>(ev =>
            {
                _setIndex = 0;
                _setProgressFraction = 0;

                if (Logging.LogVerbosity == LogVerbosityEnum.FullLogging)
                {
                    logger.LogInformation($"Acquiring sets from '{ev.Theme}-{ev.Subtheme}' to process for year {ev.Year}");
                }
            });

            messageHub.Subscribe<SetsAcquired>(ev =>
            {
                _setCount = ev.Count;
                _setIndex = 0;

                logger.LogInformation($"Acquired {ev.Count} sets {(ev.Year.HasValue ? $"from '{ev.Theme}-{ev.Subtheme}' " : string.Empty)}to process{(ev.Year.HasValue ? $" for year {ev.Year}" : string.Empty)}");
            });

            messageHub.Subscribe<SynchronizingSet>(ev =>
            {
                _setIndex++;
                _setProgressFraction = _setIndex / _setCount;

                if(Logging.LogVerbosity == LogVerbosityEnum.FullLogging)
                {
                    logger.LogInformation(Invariant($"Synchronizing Set '{ev.IdentifierLong}': index {_setIndex}, progress {_setProgressFraction:##0.00%}"));
                }
            });

            messageHub.Subscribe<SynchronizingSetException>(ev => logger.LogError(ev.Exception, $"Synchronizing Set '{ev.IdentifierLong}' Exception"));

            messageHub.Subscribe<SynchronizedSet>(ev =>
            {
                if (Logging.LogVerbosity == LogVerbosityEnum.FullLogging)
                {
                    logger.LogInformation($"Finished Synchronizing Set '{ev.IdentifierLong}'");
                }
            });

            messageHub.Subscribe<SetSynchronizerException>(ev => logger.LogError(ev.Exception, "Set Synchronizer Exception"));

            messageHub.Subscribe<SetSynchronizerEnd>(ev =>
            {
                _setIndex = 0;
                _setProgressFraction = 0;

                if (Logging.LogVerbosity == LogVerbosityEnum.FullLogging)
                {
                    logger.LogInformation($"Finished Set Synchronizer {(ev.ForSubtheme ? " for subtheme" : string.Empty)}");
                }
            });
        }
    }
}