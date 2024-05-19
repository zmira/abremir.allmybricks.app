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
        IMessageHub messageHub)
        : ISetSanitizeService
    {
        public async Task Synchronize()
        {
            messageHub.Publish(new SetSanitizeServiceStart());

            try
            {
                await themeSanitizer.Synchronize().ConfigureAwait(false);
                await setSanitizer.Synchronize().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                messageHub.Publish(new SetSanitizeServiceException { Exception = ex });
            }

            messageHub.Publish(new SetSanitizeServiceEnd());
        }
    }
}
