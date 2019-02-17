using abremir.AllMyBricks.AssetManagement.Implementations;
using abremir.AllMyBricks.DatabaseSeeder.Configuration;
using Easy.MessageHub;
using Microsoft.Extensions.Logging;
using SharpCompress.Common;

namespace abremir.AllMyBricks.DatabaseSeeder.Loggers
{
    public class AssetUncompressionLogger : IDatabaseSeederLogger
    {
        public AssetUncompressionLogger(
            ILoggerFactory loggerFactory,
            IMessageHub messageHub)
        {
            var logger = loggerFactory.CreateLogger<AssetUncompression>();

            messageHub.Subscribe<ReaderExtractionEventArgs<IEntry>>(ev =>
            {
                if (Logging.LogVerbosity == LogVerbosityEnum.FullLogging)
                {
                    logger.LogInformation($"Uncompressing {ev.Item.Key}: {ev.ReaderProgress.PercentageRead}%");
                }
            });
        }
    }
}
