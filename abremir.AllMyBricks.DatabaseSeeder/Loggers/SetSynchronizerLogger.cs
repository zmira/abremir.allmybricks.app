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

            messageHub.Subscribe<SetSynchronizerStart>(message =>
            {
                _setIndex = 0;
                _setProgressFraction = 0;

                if (Logging.LogVerbosity == LogVerbosityEnum.FullLogging)
                {
                    logger.LogInformation($"Started set synchronizer{(message.ForSubtheme ? " for subtheme" : string.Empty)}");
                }
            });

            messageHub.Subscribe<AcquiringSetsStart>(message =>
            {
                _setIndex = 0;
                _setProgressFraction = 0;

                if (Logging.LogVerbosity == LogVerbosityEnum.FullLogging)
                {
                    logger.LogInformation($"Acquiring sets from '{message.Theme}-{message.Subtheme}' to process for year {message.Year}");
                }
            });

            messageHub.Subscribe<AcquiringSetsEnd>(message =>
            {
                _setCount = message.Count;
                _setIndex = 0;

                logger.LogInformation($"Acquired {message.Count} sets {(message.Year.HasValue ? $"from '{message.Theme}-{message.Subtheme}' " : string.Empty)}to process{(message.Year.HasValue ? $" for year {message.Year}" : string.Empty)}");
            });

            messageHub.Subscribe<SynchronizingSetStart>(message =>
            {
                _setIndex++;
                _setProgressFraction = _setIndex / _setCount;

                if (Logging.LogVerbosity == LogVerbosityEnum.FullLogging)
                {
                    logger.LogInformation(Invariant($"Started synchronizing set '{message.IdentifierLong}': index {_setIndex}, progress {_setProgressFraction:##0.00%}"));
                }
            });

            messageHub.Subscribe<SynchronizingSetException>(message => logger.LogError(message.Exception, $"Synchronizing Set '{message.IdentifierLong}' Exception"));

            messageHub.Subscribe<SynchronizingSetEnd>(message =>
            {
                if (Logging.LogVerbosity == LogVerbosityEnum.FullLogging)
                {
                    logger.LogInformation($"Finished synchronizing set '{message.IdentifierLong}'");
                }
            });

            messageHub.Subscribe<SetSynchronizerException>(message => logger.LogError(message.Exception, "Set Synchronizer Exception"));

            messageHub.Subscribe<SetSynchronizerEnd>(message =>
            {
                _setIndex = 0;
                _setProgressFraction = 0;

                if (Logging.LogVerbosity == LogVerbosityEnum.FullLogging)
                {
                    logger.LogInformation($"Finished set synchronizer{(message.ForSubtheme ? " for subtheme" : string.Empty)}");
                }
            });
        }
    }
}
