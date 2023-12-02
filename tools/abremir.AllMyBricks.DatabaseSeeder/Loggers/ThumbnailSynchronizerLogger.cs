using abremir.AllMyBricks.DatabaseSeeder.Configuration;
using abremir.AllMyBricks.DatabaseSeeder.Enumerations;
using abremir.AllMyBricks.DataSynchronizer.Events.ThumbnailSynchronizer;
using abremir.AllMyBricks.DataSynchronizer.Synchronizers;
using Easy.MessageHub;
using Microsoft.Extensions.Logging;

namespace abremir.AllMyBricks.DatabaseSeeder.Loggers
{
    public class ThumbnailSynchronizerLogger : IDatabaseSeederLogger
    {
        public ThumbnailSynchronizerLogger(
            ILoggerFactory loggerFactory,
            IMessageHub messageHub)
        {
            var logger = loggerFactory.CreateLogger<ThumbnailSynchronizer>();

            messageHub.Subscribe<ThumbnailSynchronizerStart>(_ =>
            {
                if (Logging.LogVerbosity == LogVerbosity.FullLogging)
                {
                    logger.LogInformation("Started thumbnail synchronizer");
                }
            });

            messageHub.Subscribe<ThumbnailAcquired>(message =>
            {
                if (Logging.LogVerbosity == LogVerbosity.FullLogging)
                {
                    logger.LogInformation($"Acquired thumbnail '{message.Thumbnail}' to process");
                }
            });

            messageHub.Subscribe<SynchronizingThumbnailStart>(message =>
            {
                if (Logging.LogVerbosity == LogVerbosity.FullLogging)
                {
                    logger.LogInformation($"Started synchronizing thumbnail '{message.Thumbnail}'");
                }
            });

            messageHub.Subscribe<SynchronizingThumbnailEnd>(message =>
            {
                if (Logging.LogVerbosity == LogVerbosity.FullLogging)
                {
                    logger.LogInformation($"Finished synchronizing thumbnail '{message.Thumbnail}'");
                }
            });

            messageHub.Subscribe<ThumbnailSynchronizerException>(message => logger.LogError(message.Exception, "Thumbnail Synchronizer Exception"));

            messageHub.Subscribe<ThumbnailSynchronizerEnd>(_ =>
            {
                if (Logging.LogVerbosity == LogVerbosity.FullLogging)
                {
                    logger.LogInformation("Finished thumbnail synchronizer");
                }
            });
        }
    }
}
