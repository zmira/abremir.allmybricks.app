using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using abremir.AllMyBricks.DataSynchronizer.Synchronizers;
using SimpleInjector;

namespace abremir.AllMyBricks.DataSynchronizer.Configuration
{
    public static class IoC
    {
        public static Container Configure(Container container = null)
        {
            container = container ?? new Container();

            container.Register<IThemeSynchronizer, ThemeSynchronizer>(Lifestyle.Transient);

            return container;
        }
    }
}