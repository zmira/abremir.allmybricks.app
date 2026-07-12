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
        private readonly IThemeSynchronizer _themeSynchronizer = themeSynchronizer;
        private readonly ISubthemeSynchronizer _subthemeSynchronizer = subthemeSynchronizer;
        private readonly IFullSetSynchronizer _fullSetSynchronizer = fullSetSynchronizer;
        private readonly IPartialSetSynchronizer _partialSetSynchronizer = partialSetSynchronizer;
        private readonly IMessageHub _messageHub = messageHub;

        public async Task<int> Synchronize()
        {
            int result = 0;
            _messageHub.Publish(new SetSynchronizationServiceStart());

            try
            {
                result += await _themeSynchronizer.Synchronize().ConfigureAwait(false);
                result += await _subthemeSynchronizer.Synchronize().ConfigureAwait(false);
                result += await _fullSetSynchronizer.Synchronize().ConfigureAwait(false);
                result += await _partialSetSynchronizer.Synchronize().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _messageHub.Publish(new SetSynchronizationServiceException { Exception = ex });

                return 1;
            }

            _messageHub.Publish(new SetSynchronizationServiceEnd());

            return result;
        }
    }
}
