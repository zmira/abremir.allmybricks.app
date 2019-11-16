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

            messageHub.Subscribe<UserSynchronizerStart>(ev =>
            {
            });

            messageHub.Subscribe<AllMyBricksToBricksetStart>(_ =>
            {
            });

            messageHub.Subscribe<UserSynchronizerSetsAcquired>(ev =>
            {
            });

            messageHub.Subscribe<UserSynchronizerSynchronizingSet>(ev =>
            {
            });

            messageHub.Subscribe<UserSynchronizerSynchronizedSet>(ev =>
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

            messageHub.Subscribe<UserSynchronizerException>(ev =>
            {
            });

            messageHub.Subscribe<UserSynchronizerEnd>(ev =>
            {
            });
        }
    }
}
