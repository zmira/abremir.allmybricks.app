using abremir.AllMyBricks.DatabaseSeeder.Configuration;
using abremir.AllMyBricks.DatabaseSeeder.Enumerations;
using abremir.AllMyBricks.DataSynchronizer.Events.SubthemeSynchronizer;
using abremir.AllMyBricks.DataSynchronizer.Synchronizers;
using Easy.MessageHub;
using Microsoft.Extensions.Logging;

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

                if (Logging.LogVerbosity is LogVerbosity.Full)
                {
                    logger.LogInformation("Started subtheme synchronizer");
                }
            });

            messageHub.Subscribe<SubthemesAcquired>(message =>
            {
                _subthemeIndex = 0;
                _subthemeCount = message.Count;

                logger.LogInformation("Acquired {Count} subthemes to process for theme '{Theme}'", message.Count, message.Theme);
            });

            messageHub.Subscribe<SynchronizingSubthemeStart>(message =>
            {
                _subthemeIndex++;
                _subthemeProgressFraction = _subthemeIndex / _subthemeCount;

                if (Logging.LogVerbosity is LogVerbosity.Full)
                {
                    logger.LogInformation("Started synchronizing subtheme '{Theme}-{Subtheme}': index {Index}, progress {Progress:##0.00%}", message.Theme, message.Subtheme, _subthemeIndex, _subthemeProgressFraction);
                }
            });

            messageHub.Subscribe<SynchronizingSubthemeEnd>(message =>
            {
                if (Logging.LogVerbosity is LogVerbosity.Full)
                {
                    logger.LogInformation("Finished synchronizing subtheme '{Theme}-{Subtheme}'", message.Theme, message.Subtheme);
                }
            });

            messageHub.Subscribe<SubthemeSynchronizerException>(message => logger.LogError(message.Exception, "Subtheme Synchronizer Exception for theme '{Theme}'", message.Theme));

            messageHub.Subscribe<SubthemeSynchronizerEnd>(_ =>
            {
                _subthemeIndex = 0;
                _subthemeProgressFraction = 0;

                if (Logging.LogVerbosity is LogVerbosity.Full)
                {
                    logger.LogInformation("Finished subtheme synchronizer");
                }
            });
        }
    }
}
