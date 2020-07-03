using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using abremir.AllMyBricks.DataSynchronizer.Services;
using abremir.AllMyBricks.DataSynchronizer.Synchronizers;
using LightInject;

namespace abremir.AllMyBricks.DataSynchronizer.Configuration
{
    public static class IoC
    {
        public static IServiceRegistry Configure(IServiceRegistry container = null)
        {
            container ??= new ServiceContainer();

            container.Register<ISetSynchronizationService, SetSynchronizationService>();
            container.Register<IThemeSynchronizer, ThemeSynchronizer>();
            container.Register<ISubthemeSynchronizer, SubthemeSynchronizer>();
            container.Register<ISetSynchronizer, SetSynchronizer>();
            container.Register<IThumbnailSynchronizer, ThumbnailSynchronizer>();
            container.Register<IUserSynchronizationService, UserSynchronizationService>();
            container.Register<IUserSynchronizer, UserSynchronizer>();

            return container;
        }
    }
}
