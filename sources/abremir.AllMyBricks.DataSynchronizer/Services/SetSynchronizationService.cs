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
        IMessageHub messageHub) : ISetSynchronizationService
    {
        private readonly IThemeSynchronizer _themeSynchronizer = themeSynchronizer;
        private readonly ISubthemeSynchronizer _subthemeSynchronizer = subthemeSynchronizer;
        private readonly IFullSetSynchronizer _fullSetSynchronizer = fullSetSynchronizer;
        private readonly IPartialSetSynchronizer _partialSetSynchronizer = partialSetSynchronizer;
        private readonly IMessageHub _messageHub = messageHub;

        public async Task Synchronize()
        {
            _messageHub.Publish(new SetSynchronizationServiceStart());

            try
            {
                await _themeSynchronizer.Synchronize().ConfigureAwait(false);
                await _subthemeSynchronizer.Synchronize().ConfigureAwait(false);
                await _fullSetSynchronizer.Synchronize().ConfigureAwait(false);
                await _partialSetSynchronizer.Synchronize().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _messageHub.Publish(new SetSynchronizationServiceException { Exception = ex });
            }

            _messageHub.Publish(new SetSynchronizationServiceEnd());
        }
    }
}
