﻿using abremir.AllMyBricks.Data.Interfaces;
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
            container.Register<IReferenceDataRepository, ReferenceDataRepository>(Lifestyle.Transient);
            container.Register<ISetRepository, SetRepository>(Lifestyle.Transient);
            container.Register<IInsightsRepository, InsightsRepository>(Lifestyle.Transient);
            container.Register<IBricksetUserRepository, BricksetUserRepository>(Lifestyle.Transient);

            return container;
        }
    }
}
