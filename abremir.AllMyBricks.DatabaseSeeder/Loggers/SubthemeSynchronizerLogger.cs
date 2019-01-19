using abremir.AllMyBricks.DatabaseSeeder.Configuration;
using abremir.AllMyBricks.DataSynchronizer.Events.SubthemeSynchronizer;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using abremir.AllMyBricks.DataSynchronizer.Synchronizers;
using Microsoft.Extensions.Logging;

using static System.FormattableString;

namespace abremir.AllMyBricks.DatabaseSeeder.Loggers
{
    public class SubthemeSynchronizerLogger : IDatabaseSeederLogger
    {
        private static float _subthemeIndex;
        private static float _subthemeProgressFraction;
        private static float _subthemeCount;

        private readonly ILogger _logger;

        public SubthemeSynchronizerLogger(ILoggerFactory loggerFactory, IDataSynchronizerEventManager dataSynchronizerEventManager)
        {
            _logger = loggerFactory.CreateLogger<SubthemeSynchronizer>();

            dataSynchronizerEventManager.Register<SubthemeSynchronizerStart>(_ =>
            {
                _subthemeIndex = 0;
                _subthemeProgressFraction = 0;

                if (Logging.LogVerbosity == LoggingVerbosityEnum.FullLogging)
                {
                    _logger.LogInformation("Subtheme Synchronizer Started");
                }
            });

            dataSynchronizerEventManager.Register<SubthemesAcquired>(ev =>
            {
                _subthemeIndex = 0;
                _subthemeCount = ev.Count;

                _logger.LogInformation($"Acquired {ev.Count} subthemes to process for theme '{ev.Theme}'");
            });

            dataSynchronizerEventManager.Register<SynchronizingSubtheme>(ev =>
            {
                _subthemeIndex++;
                _subthemeProgressFraction = _subthemeIndex / _subthemeCount;

                if (Logging.LogVerbosity == LoggingVerbosityEnum.FullLogging)
                {
                    _logger.LogInformation(Invariant($"Synchronizing Subtheme '{ev.Theme}-{ev.Subtheme}': index {_subthemeIndex}, progress {_subthemeProgressFraction:##0.00%}"));
                }
            });

            dataSynchronizerEventManager.Register<SynchronizingSubthemeException>(ev => _logger.LogError(ev.Exception, $"Synchronizing Subtheme '{ev.Theme}-{ev.Subtheme}' Exception"));

            dataSynchronizerEventManager.Register<SynchronizedSubtheme>(ev =>
            {
                if (Logging.LogVerbosity == LoggingVerbosityEnum.FullLogging)
                {
                    _logger.LogInformation($"Finished Synchronizing Subtheme '{ev.Theme}-{ev.Subtheme}'");
                }
            });

            dataSynchronizerEventManager.Register<SubthemeSynchronizerException>(ev => _logger.LogError(ev.Exception, $"Subtheme Synchronizer Exception for theme '{ev.Theme}'"));

            dataSynchronizerEventManager.Register<SubthemeSynchronizerEnd>(_ =>
            {
                _subthemeIndex = 0;
                _subthemeProgressFraction = 0;

                if (Logging.LogVerbosity == LoggingVerbosityEnum.FullLogging)
                {
                    _logger.LogInformation("Finished Subtheme Synchronizer");
                }
            });
        }
    }
}