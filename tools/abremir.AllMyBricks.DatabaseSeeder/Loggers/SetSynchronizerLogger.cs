using abremir.AllMyBricks.DatabaseSeeder.Configuration;
using abremir.AllMyBricks.DatabaseSeeder.Enumerations;
using abremir.AllMyBricks.DataSynchronizer.Events.SetSynchronizer;
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
            var logger = loggerFactory.CreateLogger<SetSynchronizerLogger>();

            messageHub.Subscribe<SetSynchronizerStart>(message =>
            {
                _setIndex = 0;
                _setProgressFraction = 0;

                if (Logging.LogVerbosity is LogVerbosity.Full)
                {
                    logger.LogInformation($"Started set synchronizer of type '{message.Type}'");
                }
            });

            messageHub.Subscribe<AcquiringSetsStart>(message =>
            {
                _setIndex = 0;
                _setProgressFraction = 0;

                if (Logging.LogVerbosity is LogVerbosity.Full)
                {
                    logger.LogInformation($"Acquiring sets for type '{message.Type}' with parameters '{message.Parameters.GetParams()}'");
                }
            });

            messageHub.Subscribe<AcquiringSetsEnd>(message =>
            {
                _setCount = message.Count;
                _setIndex = 0;

                if (Logging.LogVerbosity is LogVerbosity.Full)
                {
                    logger.LogInformation($"Acquired {message.Count} sets for type '{message.Type}' with parameters '{message.Parameters.GetParams()}'");
                }
            });

            messageHub.Subscribe<SynchronizingSetStart>(message =>
            {
                _setIndex++;
                _setProgressFraction = _setIndex / _setCount;

                if (Logging.LogVerbosity is LogVerbosity.Full)
                {
                    logger.LogInformation(Invariant($"Started synchronizing set '{message.IdentifierLong}': index {_setIndex}, progress {_setProgressFraction:##0.00%}"));
                }
            });

            messageHub.Subscribe<SynchronizingSetException>(message => logger.LogError(message.Exception, $"Synchronizing Set '{message.IdentifierLong}' Exception"));

            messageHub.Subscribe<SynchronizingSetEnd>(message =>
            {
                if (Logging.LogVerbosity is LogVerbosity.Full)
                {
                    logger.LogInformation($"Finished synchronizing set '{message.IdentifierLong}'");
                }
            });

            messageHub.Subscribe<SetSynchronizerException>(message => logger.LogError(message.Exception, "Set Synchronizer Exception"));

            messageHub.Subscribe<SetSynchronizerEnd>(message =>
            {
                _setIndex = 0;
                _setProgressFraction = 0;

                if (Logging.LogVerbosity is LogVerbosity.Full)
                {
                    logger.LogInformation($"Finished set synchronizer of type '{message.Type}'");
                }
            });

            messageHub.Subscribe<MismatchingNumberOfSetsWarning>(message => logger.LogWarning($"Mismatched number of sets! Expected: {message.Expected}; Actual: {message.Actual}"));

            messageHub.Subscribe<InsightsAcquired>(message => logger.LogInformation($"Last Updated: {(message.SynchronizationTimestamp.HasValue ? message.SynchronizationTimestamp.Value.ToString("yyyy-MM-dd HH:mm:ss") : "Never")}"));

            messageHub.Subscribe<DeletingSetsStart>(message =>
            {
                if (Logging.LogVerbosity is LogVerbosity.Full)
                {
                    logger.LogInformation($"Started deleting sets: {string.Join(", ", message.AffectedSets)}");
                }
            });

            messageHub.Subscribe<DeletingSetsEnd>(message =>
            {
                if (Logging.LogVerbosity is LogVerbosity.Full)
                {
                    logger.LogInformation($"Finished deleting sets: {string.Join(", ", message.AffectedSets)}");
                }
            });

            messageHub.Subscribe<ThemesAcquired>(message =>
            {
                if (Logging.LogVerbosity is LogVerbosity.Full)
                {
                    logger.LogInformation($"Acquired {message.Count} themes");
                }
            });
        }
    }
}
