using abremir.AllMyBricks.ThirdParty.Brickset.Interfaces;
using abremir.AllMyBricks.ThirdParty.Brickset.Services;
using LightInject;

namespace abremir.AllMyBricks.ThirdParty.Brickset.Configuration
{
    public static class IoC
    {
        public static IServiceRegistry Configure(IServiceRegistry container = null)
        {
            container ??= new ServiceContainer();

            container.Register<IBricksetApiService, BricksetApiService>();

            return container;
        }
    }
}
