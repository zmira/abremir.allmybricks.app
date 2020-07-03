using abremir.AllMyBricks.Data.Interfaces;
using abremir.AllMyBricks.Data.Repositories;
using abremir.AllMyBricks.Data.Services;
using LightInject;

namespace abremir.AllMyBricks.Data.Configuration
{
    public static class IoC
    {
        public static IServiceRegistry Configure(IServiceRegistry container = null)
        {
            container ??= new ServiceContainer();

            container.Register<IRepositoryService, RepositoryService>();
            container.Register<IThemeRepository, ThemeRepository>();
            container.Register<ISubthemeRepository, SubthemeRepository>();
            container.Register<IReferenceDataRepository, ReferenceDataRepository>();
            container.Register<ISetRepository, SetRepository>();
            container.Register<IInsightsRepository, InsightsRepository>();
            container.Register<IBricksetUserRepository, BricksetUserRepository>();

            return container;
        }
    }
}
