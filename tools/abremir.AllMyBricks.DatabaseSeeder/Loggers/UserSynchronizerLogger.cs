using abremir.AllMyBricks.DatabaseSeeder.Configuration;
using abremir.AllMyBricks.DatabaseSeeder.Enumerations;
using abremir.AllMyBricks.DataSynchronizer.Events.UserSynchronizer;
using abremir.AllMyBricks.DataSynchronizer.Synchronizers;
using Easy.MessageHub;
using Microsoft.Extensions.Logging;

namespace abremir.AllMyBricks.DatabaseSeeder.Loggers
{
    public class UserSynchronizerLogger : IDatabaseSeederLogger
    {
        private static float _setIndex;
        private static float _setProgressFraction;
        private static float _setCount;

        public UserSynchronizerLogger(
            ILoggerFactory loggerFactory,
            IMessageHub messageHub)
        {
            var logger = loggerFactory.CreateLogger<UserSynchronizer>();

            messageHub.Subscribe<UserSynchronizerStart>(message =>
            {
                _setIndex = 0;
                _setProgressFraction = 0;

                if (Logging.LogVerbosity is LogVerbosity.Full)
                {
                    logger.LogInformation("Started user synchronizer for {Type} user '{Username}'", message.UserType, message.Username);
                }
            });

            messageHub.Subscribe<AllMyBricksToBricksetStart>(_ =>
            {
                _setIndex = 0;
                _setProgressFraction = 0;

                if (Logging.LogVerbosity is LogVerbosity.Full)
                {
                    logger.LogInformation("Started update of Brickset.com with changes made in All My Bricks");
                }
            });

            messageHub.Subscribe<AllMyBricksToBricksetAcquiringSetsStart>(_ =>
            {
                _setIndex = 0;
                _setProgressFraction = 0;

                if (Logging.LogVerbosity is LogVerbosity.Full)
                {
                    logger.LogInformation("Acquiring All My Bricks user sets to update in Brickset.com");
                }
            });

            messageHub.Subscribe<AllMyBricksToBricksetAcquiringSetsEnd>(message =>
            {
                _setCount = message.Count;
                _setIndex = 0;

                logger.LogInformation("Acquired {Count} All My Bricks user sets to update in Brickset.com", message.Count);
            });

            messageHub.Subscribe<UserSynchronizerSynchronizingSetStart>(message =>
            {
                _setIndex++;
                _setProgressFraction = _setIndex / _setCount;

                if (Logging.LogVerbosity is LogVerbosity.Full)
                {
                    logger.LogInformation("Started synchronizing set {Id}: index {Index}, progress {Progress:##0.00%}", message.SetId, _setIndex, _setProgressFraction);
                }
            });

            messageHub.Subscribe<UserSynchronizerSynchronizingSetEnd>(message =>
            {
                if (Logging.LogVerbosity is LogVerbosity.Full)
                {
                    logger.LogInformation("Finished synchronizing set {Id}", message.SetId);
                }
            });

            messageHub.Subscribe<AllMyBricksToBricksetEnd>(_ =>
            {
                _setIndex = 0;
                _setProgressFraction = 0;

                if (Logging.LogVerbosity is LogVerbosity.Full)
                {
                    logger.LogInformation("Finished update of Brickset.com with changes made in All My Bricks");
                }
            });

            messageHub.Subscribe<BricksetToAllMyBricksStart>(_ =>
            {
                _setIndex = 0;
                _setProgressFraction = 0;

                if (Logging.LogVerbosity is LogVerbosity.Full)
                {
                    logger.LogInformation("Started adding Brickset.com user sets missing in All My Bricks");
                }
            });

            messageHub.Subscribe<BricksetToAllMyBricksAcquiringSetsStart>(_ =>
            {
                _setIndex = 0;
                _setProgressFraction = 0;

                if (Logging.LogVerbosity is LogVerbosity.Full)
                {
                    logger.LogInformation("Acquiring Brickset.com user sets missing in All My Bricks");
                }
            });

            messageHub.Subscribe<BricksetToAllMyBricksAcquiringSetsEnd>(message =>
            {
                _setCount = message.Count;
                _setIndex = 0;

                logger.LogInformation("Acquired {Count} user sets from Brickset.com missing in All My Bricks", message.Count);
            });

            messageHub.Subscribe<BricksetToAllMyBricksEnd>(_ =>
            {
                _setIndex = 0;
                _setProgressFraction = 0;

                if (Logging.LogVerbosity is LogVerbosity.Full)
                {
                    logger.LogInformation("Finished adding Brickset.com user sets missing in All My Bricks");
                }
            });

            messageHub.Subscribe<UserSynchronizerException>(message => logger.LogError(message.Exception, "Synchronizing {Type} User '{Username}' Exception", message.UserType, message.Username));

            messageHub.Subscribe<UserSynchronizerEnd>(message =>
            {
                _setIndex = 0;
                _setProgressFraction = 0;

                if (Logging.LogVerbosity is LogVerbosity.Full)
                {
                    logger.LogInformation("Finished user synchronizer for {Type} user '{Username}'", message.UserType, message.Username);
                }
            });
        }
    }
}
