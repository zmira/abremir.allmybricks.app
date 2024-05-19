using System;
using System.Threading.Tasks;
using abremir.AllMyBricks.DataSynchronizer.Events.SetSynchronizationService;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using Easy.MessageHub;

namespace abremir.AllMyBricks.DataSynchronizer.Services
{
    public class SetSynchronizationService(
        IThemeSynchronizer themeSynchronizer,
        ISubthemeSynchronizer subthemeSynchronizer,
        IFullSetSynchronizer fullSetSynchronizer,
        IPartialSetSynchronizer partialSetSynchronizer,
        IMessageHub messageHub)
        : ISetSynchronizationService
    {
        public async Task Synchronize()
        {
            messageHub.Publish(new SetSynchronizationServiceStart());

            try
            {
                await themeSynchronizer.Synchronize().ConfigureAwait(false);
                await subthemeSynchronizer.Synchronize().ConfigureAwait(false);
                await fullSetSynchronizer.Synchronize().ConfigureAwait(false);
                await partialSetSynchronizer.Synchronize().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                messageHub.Publish(new SetSynchronizationServiceException { Exception = ex });
            }

            messageHub.Publish(new SetSynchronizationServiceEnd());
        }
    }
}
