using abremir.AllMyBricks.DatabaseSeeder.Configuration;
using abremir.AllMyBricks.DatabaseSeeder.Enumerations;
using abremir.AllMyBricks.DataSynchronizer.Events.SetSynchronizer;
using Easy.MessageHub;
using Microsoft.Extensions.Logging;

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

                logger.LogInformation("Started set synchronizer of type '{Type}'", message.Type);
            });

            messageHub.Subscribe<AcquiringSetsStart>(message =>
            {
                _setIndex = 0;
                _setProgressFraction = 0;

                if (Logging.LogVerbosity is LogVerbosity.Full)
                {
                    logger.LogInformation("Acquiring sets for type '{Type}' with parameters '{Parameters}'", message.Type, message.Parameters.GetParams());
                }
            });

            messageHub.Subscribe<AcquiringSetsEnd>(message =>
            {
                _setCount = message.Count;
                _setIndex = 0;

                if (Logging.LogVerbosity is LogVerbosity.Full)
                {
                    logger.LogInformation("Acquired {Count} sets for type '{Type}' with parameters '{Parameters}'", message.Count, message.Type, message.Parameters.GetParams());
                }
            });

            messageHub.Subscribe<SynchronizingSetStart>(message =>
            {
                _setIndex++;
                _setProgressFraction = _setIndex / _setCount;

                if (Logging.LogVerbosity is LogVerbosity.Full)
                {
                    logger.LogInformation("Started synchronizing set '{Identifier}': index {Index}, progress {Progress:##0.00%}", message.IdentifierLong, _setIndex, _setProgressFraction);
                }
            });

            messageHub.Subscribe<SynchronizingSetException>(message => logger.LogError(message.Exception, "Synchronizing Set '{Identifier}' Exception", message.IdentifierLong));

            messageHub.Subscribe<SynchronizingSetEnd>(message =>
            {
                if (Logging.LogVerbosity is LogVerbosity.Full)
                {
                    logger.LogInformation("Finished synchronizing set '{Identifier}'", message.IdentifierLong);
                }
            });

            messageHub.Subscribe<SetSynchronizerException>(message => logger.LogError(message.Exception, "Set Synchronizer Exception"));

            messageHub.Subscribe<SetSynchronizerEnd>(message =>
            {
                _setIndex = 0;
                _setProgressFraction = 0;

                logger.LogInformation("Finished set synchronizer of type '{Type}'", message.Type);
            });

            messageHub.Subscribe<MismatchingNumberOfSetsWarning>(message => logger.LogWarning("Mismatched number of sets! Expected: {Expected}; Actual: {Actual}", message.Expected, message.Actual));

            messageHub.Subscribe<InsightsAcquired>(message => logger.LogInformation("Last Updated: {Timestamp}", message.SynchronizationTimestamp.HasValue ? message.SynchronizationTimestamp.Value.ToString("yyyy-MM-dd HH:mm:ss") : "Never"));

            messageHub.Subscribe<DeletingSetsStart>(message => logger.LogInformation("Started deleting sets: {Sets}", string.Join(", ", message.AffectedSets)));

            messageHub.Subscribe<DeletingSetsEnd>(message =>
            {
                if (Logging.LogVerbosity is LogVerbosity.Full)
                {
                    logger.LogInformation("Finished deleting sets: {Sets}", string.Join(", ", message.AffectedSets));
                }
            });

            messageHub.Subscribe<ThemesAcquired>(message =>
            {
                if (Logging.LogVerbosity is LogVerbosity.Full)
                {
                    logger.LogInformation("Acquired {Count} themes", message.Count);
                }
            });
        }
    }
}
