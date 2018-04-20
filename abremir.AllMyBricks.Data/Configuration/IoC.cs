using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Repositories;
using abremir.AllMyBricks.Data.Services;
using SimpleInjector;

namespace abremir.AllMyBricks.Data.Configuration
{
    public static class IoC
    {
        public static Container Configure(Container container = null)
        {
            container = container ?? new Container();

            container.Register<IRepositoryService, RepositoryService>(Lifestyle.Transient);
            container.Register<IThemeRepository, ThemeRepository>(Lifestyle.Transient);
            container.Register<ISubthemeRepository, SubthemeRepository>(Lifestyle.Transient);

            return container;
        }
    }
}