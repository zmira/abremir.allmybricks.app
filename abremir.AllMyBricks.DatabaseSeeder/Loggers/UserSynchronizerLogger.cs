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
        }
    }
}
