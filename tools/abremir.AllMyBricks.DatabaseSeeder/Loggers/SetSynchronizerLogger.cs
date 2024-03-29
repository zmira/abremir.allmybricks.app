﻿using abremir.AllMyBricks.DatabaseSeeder.Configuration;
using abremir.AllMyBricks.DatabaseSeeder.Enumerations;
using abremir.AllMyBricks.DataSynchronizer.Events.SetSynchronizer;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
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
            var logger = loggerFactory.CreateLogger<ISetSynchronizer>();

            messageHub.Subscribe<SetSynchronizerStart>(message =>
            {
                _setIndex = 0;
                _setProgressFraction = 0;

                if (Logging.LogVerbosity == LogVerbosity.Full)
                {
                    logger.LogInformation($"Started set synchronizer for {(message.Complete ? "full" : "partial")} dataset");
                }
            });

            messageHub.Subscribe<AcquiringSetsStart>(message =>
            {
                _setIndex = 0;
                _setProgressFraction = 0;

                if (Logging.LogVerbosity == LogVerbosity.Full)
                {
                    logger.LogInformation($"Acquiring{(message.Complete ? " all" : string.Empty)} sets {(message.Complete ? "from" : "updated since")} '{(message.Complete ? message.Years : message.From.Value.ToString("yyyy-MM-dd HH:mm:ss.ffff"))}' to process");
                }
            });

            messageHub.Subscribe<AcquiringSetsEnd>(message =>
            {
                _setCount = message.Count;
                _setIndex = 0;

                if (Logging.LogVerbosity is LogVerbosity.Full)
                {
                    logger.LogInformation($"Acquired {message.Count} sets {(message.Complete ? "from" : "updated since")} '{(message.Complete ? message.Years : message.From.Value.ToString("yyyy-MM-dd HH:mm:ss.ffff"))}' to process");
                }
            });

            messageHub.Subscribe<SynchronizingSetStart>(message =>
            {
                _setIndex++;
                _setProgressFraction = _setIndex / _setCount;

                if (Logging.LogVerbosity == LogVerbosity.Full)
                {
                    logger.LogInformation(Invariant($"Started synchronizing set '{message.IdentifierLong}': index {_setIndex}, progress {_setProgressFraction:##0.00%}"));
                }
            });

            messageHub.Subscribe<SynchronizingSetException>(message => logger.LogError(message.Exception, $"Synchronizing Set '{message.IdentifierLong}' Exception"));

            messageHub.Subscribe<SynchronizingSetEnd>(message =>
            {
                if (Logging.LogVerbosity == LogVerbosity.Full)
                {
                    logger.LogInformation($"Finished synchronizing set '{message.IdentifierLong}'");
                }
            });

            messageHub.Subscribe<SetSynchronizerException>(message => logger.LogError(message.Exception, "Set Synchronizer Exception"));

            messageHub.Subscribe<SetSynchronizerEnd>(message =>
            {
                _setIndex = 0;
                _setProgressFraction = 0;

                if (Logging.LogVerbosity == LogVerbosity.Full)
                {
                    logger.LogInformation($"Finished set synchronizer for {(message.Complete ? "full" : "partial")} dataset");
                }
            });

            messageHub.Subscribe<MismatchingNumberOfSetsWarning>(message => logger.LogWarning("Mismatched number of sets! Expected: {0}; Actual: {1}", message.Expected, message.Actual));
        }
    }
}
