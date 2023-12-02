using abremir.AllMyBricks.AssetManagement.Implementations;
using abremir.AllMyBricks.DatabaseSeeder.Configuration;
using abremir.AllMyBricks.DatabaseSeeder.Enumerations;
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

            messageHub.Subscribe<ReaderExtractionEventArgs<IEntry>>(message =>
            {
                if (Logging.LogVerbosity == LogVerbosity.FullLogging)
                {
                    logger.LogInformation($"Uncompressing {message.Item.Key}: {message.ReaderProgress.PercentageRead}%");
                }
            });
        }
    }
}
