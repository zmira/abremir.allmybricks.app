using abremir.AllMyBricks.DatabaseSeeder.Configuration;
using abremir.AllMyBricks.DatabaseSeeder.Enumerations;
using abremir.AllMyBricks.DataSynchronizer.Events.SetSynchronizer;
using abremir.AllMyBricks.DataSynchronizer.Events.ThemeSynchronizer;
using abremir.AllMyBricks.DataSynchronizer.Synchronizers;
using Easy.MessageHub;
using Microsoft.Extensions.Logging;

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

                if (Logging.LogVerbosity is LogVerbosity.Full)
                {
                    logger.LogInformation("Started theme synchronizer");
                }
            });

            messageHub.Subscribe<ThemesAcquired>(message => _themeCount = message.Count);

            messageHub.Subscribe<SynchronizingThemeStart>(message =>
            {
                _themeIndex++;
                _themeProgressFraction = _themeIndex / _themeCount;

                if (Logging.LogVerbosity is LogVerbosity.Full)
                {
                    logger.LogInformation("Started synchronizing theme '{Theme}': index {Index}, progress {Progress:##0.00%}", message.Theme, _themeIndex, _themeProgressFraction);
                }
            });

            messageHub.Subscribe<SynchronizingThemeEnd>(message =>
            {
                if (Logging.LogVerbosity is LogVerbosity.Full)
                {
                    logger.LogInformation("Finished synchronizing theme '{Theme}'", message.Theme);
                }
            });

            messageHub.Subscribe<ThemeSynchronizerException>(message => logger.LogError(message.Exception, "Theme Synchronizer Exception"));

            messageHub.Subscribe<ThemeSynchronizerEnd>(_ =>
            {
                _themeIndex = 0;
                _themeProgressFraction = 0;

                if (Logging.LogVerbosity is LogVerbosity.Full)
                {
                    logger.LogInformation("Finished theme synchronizer");
                }
            });
        }
    }
}
