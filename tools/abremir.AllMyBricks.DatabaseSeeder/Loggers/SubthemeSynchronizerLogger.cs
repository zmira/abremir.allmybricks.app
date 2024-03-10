using abremir.AllMyBricks.DatabaseSeeder.Configuration;
using abremir.AllMyBricks.DatabaseSeeder.Enumerations;
using abremir.AllMyBricks.DataSynchronizer.Events.SubthemeSynchronizer;
using abremir.AllMyBricks.DataSynchronizer.Synchronizers;
using Easy.MessageHub;
using Microsoft.Extensions.Logging;

using static System.FormattableString;

namespace abremir.AllMyBricks.DatabaseSeeder.Loggers
{
    public class SubthemeSynchronizerLogger : IDatabaseSeederLogger
    {
        private static float _subthemeIndex;
        private static float _subthemeProgressFraction;
        private static float _subthemeCount;

        public SubthemeSynchronizerLogger(
            ILoggerFactory loggerFactory,
            IMessageHub messageHub)
        {
            var logger = loggerFactory.CreateLogger<SubthemeSynchronizer>();

            messageHub.Subscribe<SubthemeSynchronizerStart>(_ =>
            {
                _subthemeIndex = 0;
                _subthemeProgressFraction = 0;

                if (Logging.LogVerbosity == LogVerbosity.Full)
                {
                    logger.LogInformation("Started subtheme synchronizer");
                }
            });

            messageHub.Subscribe<SubthemesAcquired>(message =>
            {
                _subthemeIndex = 0;
                _subthemeCount = message.Count;

                logger.LogInformation($"Acquired {message.Count} subthemes to process for theme '{message.Theme}'");
            });

            messageHub.Subscribe<SynchronizingSubthemeStart>(message =>
            {
                _subthemeIndex++;
                _subthemeProgressFraction = _subthemeIndex / _subthemeCount;

                if (Logging.LogVerbosity == LogVerbosity.Full)
                {
                    logger.LogInformation(Invariant($"Started synchronizing subtheme '{message.Theme}-{message.Subtheme}': index {_subthemeIndex}, progress {_subthemeProgressFraction:##0.00%}"));
                }
            });

            messageHub.Subscribe<SynchronizingSubthemeEnd>(message =>
            {
                if (Logging.LogVerbosity == LogVerbosity.Full)
                {
                    logger.LogInformation($"Finished synchronizing subtheme '{message.Theme}-{message.Subtheme}'");
                }
            });

            messageHub.Subscribe<SubthemeSynchronizerException>(message => logger.LogError(message.Exception, $"Subtheme Synchronizer Exception for theme '{message.Theme}'"));

            messageHub.Subscribe<SubthemeSynchronizerEnd>(_ =>
            {
                _subthemeIndex = 0;
                _subthemeProgressFraction = 0;

                if (Logging.LogVerbosity == LogVerbosity.Full)
                {
                    logger.LogInformation("Finished subtheme synchronizer");
                }
            });
        }
    }
}
