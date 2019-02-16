using abremir.AllMyBricks.DatabaseSeeder.Configuration;
using abremir.AllMyBricks.DataSynchronizer.Events.ThumbnailSynchronizer;
using abremir.AllMyBricks.DataSynchronizer.Synchronizers;
using Easy.MessageHub;
using Microsoft.Extensions.Logging;

namespace abremir.AllMyBricks.DatabaseSeeder.Loggers
{
    public class ThumbnailSynchronizerLogger : IDatabaseSeederLogger
    {
        private readonly ILogger _logger;

        public ThumbnailSynchronizerLogger(
            ILoggerFactory loggerFactory,
            IMessageHub messageHub)
        {
            _logger = loggerFactory.CreateLogger<ThumbnailSynchronizer>();

            messageHub.Subscribe<ThumbnailSynchronizerStart>(_ =>
            {
                if(Logging.LogVerbosity == LogVerbosityEnum.FullLogging)
                {
                    _logger.LogInformation("Thumbnail Synchronizer Started");
                }
            });

            messageHub.Subscribe<ThumbnailAcquired>(ev =>
            {
                if (Logging.LogVerbosity == LogVerbosityEnum.FullLogging)
                {
                    _logger.LogInformation($"Acquired thumbnail '{ev.Thumbnail}' to process");
                }
            });

            messageHub.Subscribe<SynchronizingThumbnail>(ev =>
            {
                if (Logging.LogVerbosity == LogVerbosityEnum.FullLogging)
                {
                    _logger.LogInformation($"Synchronizing Thumbnail '{ev.Thumbnail}'");
                }
            });

            messageHub.Subscribe<SynchronizedThumbnail>(ev =>
            {
                if (Logging.LogVerbosity == LogVerbosityEnum.FullLogging)
                {
                    _logger.LogInformation($"Finished Synchronizing Thumbnail '{ev.Thumbnail}'");
                }
            });

            messageHub.Subscribe<ThumbnailSynchronizerException>(ev =>  _logger.LogError(ev.Exception, "Thumbnail Synchronizer Exception"));

            messageHub.Subscribe<ThumbnailSynchronizerEnd>(_ =>
            {
                if (Logging.LogVerbosity == LogVerbosityEnum.FullLogging)
                {
                    _logger.LogInformation("Finished Thumbnail Synchronizer");
                }
            });
        }
    }
}