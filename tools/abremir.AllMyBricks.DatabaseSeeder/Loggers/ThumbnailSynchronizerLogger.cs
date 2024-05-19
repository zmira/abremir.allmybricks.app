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
                if (Logging.LogVerbosity is LogVerbosity.Full)
                {
                    logger.LogInformation("Started thumbnail synchronizer");
                }
            });

            messageHub.Subscribe<ThumbnailAcquired>(message =>
            {
                if (Logging.LogVerbosity is LogVerbosity.Full)
                {
                    logger.LogInformation("Acquired thumbnail '{Thumbnail}' to process", message.Thumbnail);
                }
            });

            messageHub.Subscribe<SynchronizingThumbnailStart>(message =>
            {
                if (Logging.LogVerbosity is LogVerbosity.Full)
                {
                    logger.LogInformation("Started synchronizing thumbnail '{Thumbnail}'", message.Thumbnail);
                }
            });

            messageHub.Subscribe<SynchronizingThumbnailEnd>(message =>
            {
                if (Logging.LogVerbosity is LogVerbosity.Full)
                {
                    logger.LogInformation("Finished synchronizing thumbnail '{Thumbnail}'", message.Thumbnail);
                }
            });

            messageHub.Subscribe<ThumbnailSynchronizerException>(message => logger.LogError(message.Exception, "Thumbnail Synchronizer Exception"));

            messageHub.Subscribe<ThumbnailSynchronizerEnd>(_ =>
            {
                if (Logging.LogVerbosity is LogVerbosity.Full)
                {
                    logger.LogInformation("Finished thumbnail synchronizer");
                }
            });
        }
    }
}
