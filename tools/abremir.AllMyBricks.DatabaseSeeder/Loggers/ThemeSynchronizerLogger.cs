using abremir.AllMyBricks.DatabaseSeeder.Configuration;
using abremir.AllMyBricks.DatabaseSeeder.Enumerations;
using abremir.AllMyBricks.DataSynchronizer.Events.ThemeSynchronizer;
using abremir.AllMyBricks.DataSynchronizer.Synchronizers;
using Easy.MessageHub;
using Microsoft.Extensions.Logging;

using static System.FormattableString;

namespace abremir.AllMyBricks.DatabaseSeeder.Loggers
{
    public class ThemeSynchronizerLogger : IDatabaseSeederLogger
    {
        private static float _themeIndex;
        private static float _themeProgressFraction;
        private static float _themeCount;

        public ThemeSynchronizerLogger(
            ILoggerFactory loggerFactory,
            IMessageHub messageHub)
        {
            var logger = loggerFactory.CreateLogger<ThemeSynchronizer>();

            messageHub.Subscribe<ThemeSynchronizerStart>(_ =>
            {
                _themeIndex = 0;
                _themeProgressFraction = 0;

                if (Logging.LogVerbosity == LogVerbosity.Full)
                {
                    logger.LogInformation("Started theme synchronizer");
                }
            });

            messageHub.Subscribe<ThemesAcquired>(message =>
            {
                _themeCount = message.Count;

                logger.LogInformation($"Acquired {message.Count} themes to process");
            });

            messageHub.Subscribe<SynchronizingThemeStart>(message =>
            {
                _themeIndex++;
                _themeProgressFraction = _themeIndex / _themeCount;

                if (Logging.LogVerbosity == LogVerbosity.Full)
                {
                    logger.LogInformation(Invariant($"Started synchronizing theme '{message.Theme}': index {_themeIndex}, progress {_themeProgressFraction:##0.00%}"));
                }
            });

            messageHub.Subscribe<SynchronizingThemeEnd>(message =>
            {
                if (Logging.LogVerbosity == LogVerbosity.Full)
                {
                    logger.LogInformation($"Finished synchronizing theme '{message.Theme}'");
                }
            });

            messageHub.Subscribe<ThemeSynchronizerException>(message => logger.LogError(message.Exception, "Theme Synchronizer Exception"));

            messageHub.Subscribe<ThemeSynchronizerEnd>(_ =>
            {
                _themeIndex = 0;
                _themeProgressFraction = 0;

                if (Logging.LogVerbosity == LogVerbosity.Full)
                {
                    logger.LogInformation("Finished theme synchronizer");
                }
            });
        }
    }
}
