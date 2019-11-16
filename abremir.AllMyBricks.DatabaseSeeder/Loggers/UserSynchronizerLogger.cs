using abremir.AllMyBricks.DataSynchronizer.Events.UserSynchronizer;
using abremir.AllMyBricks.DataSynchronizer.Synchronizers;
using Easy.MessageHub;
using Microsoft.Extensions.Logging;

namespace abremir.AllMyBricks.DatabaseSeeder.Loggers
{
    public class UserSynchronizerLogger : IDatabaseSeederLogger
    {
        public UserSynchronizerLogger(
            ILoggerFactory loggerFactory,
            IMessageHub messageHub)
        {
            var logger = loggerFactory.CreateLogger<UserSynchronizer>();

            messageHub.Subscribe<UserSynchronizerStart>(message =>
            {
            });

            messageHub.Subscribe<AllMyBricksToBricksetStart>(_ =>
            {
            });

            messageHub.Subscribe<UserSynchronizerSetsAcquired>(message =>
            {
            });

            messageHub.Subscribe<UserSynchronizerSynchronizingSet>(message =>
            {
            });

            messageHub.Subscribe<UserSynchronizerSynchronizedSet>(message =>
            {
            });

            messageHub.Subscribe<AllMyBricksToBricksetEnd>(_ =>
            {
            });

            messageHub.Subscribe<BricksetToAllMyBricksStart>(_ =>
            {
            });

            messageHub.Subscribe<BricksetToAllMyBricksEnd>(_ =>
            {
            });

            messageHub.Subscribe<UserSynchronizerException>(message =>
            {
            });

            messageHub.Subscribe<UserSynchronizerEnd>(message =>
            {
            });
        }
    }
}
