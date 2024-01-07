using abremir.AllMyBricks.AssetManagement.Implementations;
using abremir.AllMyBricks.DatabaseSeeder.Configuration;
using abremir.AllMyBricks.DatabaseSeeder.Enumerations;
using Easy.MessageHub;
using Microsoft.Extensions.Logging;
using SharpCompress.Common;

namespace abremir.AllMyBricks.DatabaseSeeder.Loggers
{
    public class AssetExpansionLogger : IDatabaseSeederLogger
    {
        public AssetExpansionLogger(
            ILoggerFactory loggerFactory,
            IMessageHub messageHub)
        {
            var logger = loggerFactory.CreateLogger<AssetExpansion>();

            messageHub.Subscribe<ReaderExtractionEventArgs<IEntry>>(message =>
            {
                if (Logging.LogVerbosity == LogVerbosity.Full)
                {
                    logger.LogInformation($"Expanding {message.Item.Key}: {message.ReaderProgress.PercentageRead}%");
                }
            });
        }
    }
}
