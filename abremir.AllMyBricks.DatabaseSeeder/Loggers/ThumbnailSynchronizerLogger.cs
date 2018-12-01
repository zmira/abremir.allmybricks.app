using abremir.AllMyBricks.DataSynchronizer.Events.ThumbnailSynchronizer;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using abremir.AllMyBricks.DataSynchronizer.Synchronizers;
using Microsoft.Extensions.Logging;

namespace abremir.AllMyBricks.DatabaseSeeder.Loggers
{
    public class ThumbnailSynchronizerLogger
    {
        private readonly ILogger _logger;

        public ThumbnailSynchronizerLogger(ILoggerFactory loggerFactory, IDataSynchronizerEventManager dataSynchronizerEventManager)
        {
            _logger = loggerFactory.CreateLogger<ThumbnailSynchronizer>();

            dataSynchronizerEventManager.Register<ThumbnailSynchronizerStart>(_ => _logger.LogInformation("Thumbnail Synchronizer Started"));

            dataSynchronizerEventManager.Register<ThumbnailAcquired>(ev => _logger.LogInformation($"Acquired thumbnail '{ev.Thumbnail}' to process"));

            dataSynchronizerEventManager.Register<SynchronizingThumbnail>(ev => _logger.LogInformation($"Synchronizing Thumbnail '{ev.Thumbnail}'"));

            dataSynchronizerEventManager.Register<SynchronizedThumbnail>(ev => _logger.LogInformation($"Finished Synchronizing Thumbnail '{ev.Thumbnail}'"));

            dataSynchronizerEventManager.Register<ThumbnailSynchronizerException>(ev => _logger.LogError(ev.Exception, "Thumbnail Synchronizer Exception"));

            dataSynchronizerEventManager.Register<ThumbnailSynchronizerEnd>(_ => _logger.LogInformation("Finished Thumbnail Synchronizer"));
        }
    }
}