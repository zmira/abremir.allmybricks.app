using abremir.AllMyBricks.DataSynchronizer.Events.UserSynchronizationService;
using abremir.AllMyBricks.DataSynchronizer.Services;
using Easy.MessageHub;
using Microsoft.Extensions.Logging;

namespace abremir.AllMyBricks.DatabaseSeeder.Loggers
{
    public class UserSynchronizationServiceLogger : IDatabaseSeederLogger
    {
        public UserSynchronizationServiceLogger(
            ILoggerFactory loggerFactory,
            IMessageHub messageHub)
        {
            var logger = loggerFactory.CreateLogger<UserSynchronizationService>();

            messageHub.Subscribe<UserSynchronizationServiceStart>(ev =>
            {
            });

            messageHub.Subscribe<UserSynchronizationServiceException>(ev =>
            {
            });

            messageHub.Subscribe<UserSynchronizationServiceEnd>(ev =>
            {
            });
        }
    }
}
