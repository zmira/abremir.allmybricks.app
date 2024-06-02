using System;
using System.Threading.Tasks;
using abremir.AllMyBricks.DataSynchronizer.Events.SetSanitizeService;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using Easy.MessageHub;

namespace abremir.AllMyBricks.DataSynchronizer.Services
{
    public class SetSanitizeService(
        IThemeSanitizer themeSanitizer,
        ISetSanitizer setSanitizer,
        IMessageHub messageHub) : ISetSanitizeService
    {
        private readonly IThemeSanitizer _themeSanitizer = themeSanitizer;
        private readonly ISetSanitizer _setSanitizer = setSanitizer;
        private readonly IMessageHub _messageHub = messageHub;

        public async Task Synchronize()
        {
            _messageHub.Publish(new SetSanitizeServiceStart());

            try
            {
                await _themeSanitizer.Synchronize().ConfigureAwait(false);
                await _setSanitizer.Synchronize().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _messageHub.Publish(new SetSanitizeServiceException { Exception = ex });
            }

            _messageHub.Publish(new SetSanitizeServiceEnd());
        }
    }
}
