using abremir.AllMyBricks.DatabaseSeeder.Configuration;
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

                if (Logging.LogVerbosity == LogVerbosityEnum.FullLogging)
                {
                    logger.LogInformation("Theme Synchronizer Started");
                }
            });

            messageHub.Subscribe<ThemesAcquired>(ev =>
            {
                _themeCount = ev.Count;

                logger.LogInformation($"Acquired {ev.Count} themes to process");
            });

            messageHub.Subscribe<SynchronizingTheme>(ev =>
            {
                _themeIndex++;
                _themeProgressFraction = _themeIndex / _themeCount;

                if (Logging.LogVerbosity == LogVerbosityEnum.FullLogging)
                {
                    logger.LogInformation(Invariant($"Synchronizing Theme '{ev.Theme}': index {_themeIndex}, progress {_themeProgressFraction:##0.00%}"));
                }
            });

            messageHub.Subscribe<SynchronizingThemeException>(ev => logger.LogError(ev.Exception, $"Synchronizing Theme '{ev.Theme}' Exception"));

            messageHub.Subscribe<SynchronizedTheme>(ev =>
            {
                if (Logging.LogVerbosity == LogVerbosityEnum.FullLogging)
                {
                    logger.LogInformation($"Finished Synchronizing Theme '{ev.Theme}'");
                }
            });

            messageHub.Subscribe<ThemeSynchronizerException>(ev => logger.LogError(ev.Exception, "Theme Synchronizer Exception"));

            messageHub.Subscribe<ThemeSynchronizerEnd>(_ =>
            {
                _themeIndex = 0;
                _themeProgressFraction = 0;

                logger.LogInformation("Finished Theme Synchronizer");
            });
        }
    }
}