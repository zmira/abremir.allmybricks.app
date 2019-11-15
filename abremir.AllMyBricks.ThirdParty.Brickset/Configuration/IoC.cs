using abremir.AllMyBricks.ThirdParty.Brickset.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Services;
using SimpleInjector;

namespace abremir.AllMyBricks.ThirdParty.Brickset.Configuration
{
    public static class IoC
    {
        public static Container Configure(Container container = null)
        {
            container = container ?? new Container();

            container.Register<IBricksetApiService, BricksetApiService>(Lifestyle.Transient);

            return container;
        }
    }
}
