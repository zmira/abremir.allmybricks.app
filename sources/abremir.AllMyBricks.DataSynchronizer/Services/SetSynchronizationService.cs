using System;
using System.Threading.Tasks;
using abremir.AllMyBricks.DataSynchronizer.Events.SetSynchronizationService;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using Easy.MessageHub;

namespace abremir.AllMyBricks.DataSynchronizer.Services
{
    public class SetSynchronizationService : ISetSynchronizationService
    {
        private readonly IThemeSynchronizer _themeSynchronizer;
        private readonly ISubthemeSynchronizer _subthemeSynchronizer;
        private readonly IFullSetSynchronizer _fullSetSynchronizer;
        private readonly IPartialSetSynchronizer _partialSetSynchronizer;
        private readonly IMessageHub _messageHub;

        public SetSynchronizationService(
            IThemeSynchronizer themeSynchronizer,
            ISubthemeSynchronizer subthemeSynchronizer,
            IFullSetSynchronizer fullSetSynchronizer,
            IPartialSetSynchronizer partialSetSynchronizer,
            IMessageHub messageHub)
        {
            _themeSynchronizer = themeSynchronizer;
            _subthemeSynchronizer = subthemeSynchronizer;
            _fullSetSynchronizer = fullSetSynchronizer;
            _partialSetSynchronizer = partialSetSynchronizer;
            _messageHub = messageHub;
        }

        public async Task Synchronize()
        {
            _messageHub.Publish(new SetSynchronizationServiceStart());

            try
            {
                await _themeSynchronizer.Synchronize();
                await _subthemeSynchronizer.Synchronize();
                await _fullSetSynchronizer.Synchronize();
                await _partialSetSynchronizer.Synchronize();
            }
            catch (Exception ex)
            {
                _messageHub.Publish(new SetSynchronizationServiceException { Exception = ex });
            }

            _messageHub.Publish(new SetSynchronizationServiceEnd());
        }
    }
}
