using abremir.AllMyBricks.DataSynchronizer.Events.ThemeSynchronizer;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using abremir.AllMyBricks.DataSynchronizer.Synchronizers;
using Microsoft.Extensions.Logging;

using static System.FormattableString;

namespace abremir.AllMyBricks.DatabaseSeeder.Loggers
{
    public class ThemeSynchronizerLogger
    {
        private static float _themeIndex;
        private static float _themeProgressFraction;
        private static float _themeCount;

        private readonly ILogger _logger;

        public ThemeSynchronizerLogger(ILoggerFactory loggerFactory, IDataSynchronizerEventManager dataSynchronizerEventManager)
        {
            _logger = loggerFactory.CreateLogger<ThemeSynchronizer>();

            dataSynchronizerEventManager.Register<ThemeSynchronizerStart>(_ =>
            {
                _themeIndex = 0;
                _themeProgressFraction = 0;

                _logger.LogInformation("Theme Synchronizer Started");
            });

            dataSynchronizerEventManager.Register<ThemesAcquired>(ev =>
            {
                _themeCount = ev.Count;

                _logger.LogInformation($"Acquired {ev.Count} themes to process");
            });

            dataSynchronizerEventManager.Register<SynchronizingTheme>(ev =>
            {
                _themeIndex++;
                _themeProgressFraction = _themeIndex / _themeCount;

                _logger.LogInformation(Invariant($"Synchronizing Theme '{ev.Theme}': index {_themeIndex}, progress {_themeProgressFraction:##0.00%}"));
            });

            dataSynchronizerEventManager.Register<SynchronizingThemeException>(ev => _logger.LogError(ev.Exception, $"Synchronizing Theme '{ev.Theme}' Exception"));

            dataSynchronizerEventManager.Register<SynchronizedTheme>(ev => _logger.LogInformation($"Finished Synchronizing Theme '{ev.Theme}'"));

            dataSynchronizerEventManager.Register<ThemeSynchronizerException>(ev => _logger.LogError(ev.Exception, "Theme Synchronizer Exception"));

            dataSynchronizerEventManager.Register<ThemeSynchronizerEnd>(_ =>
            {
                _themeIndex = 0;
                _themeProgressFraction = 0;

                _logger.LogInformation("Finished Theme Synchronizer");
            });
        }
    }
}