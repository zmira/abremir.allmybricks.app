using abremir.AllMyBricks.UserManagement.Interfaces;
using abremir.AllMyBricks.UserManagement.Services;
using LightInject;

namespace abremir.AllMyBricks.UserManagement.Configuration
{
    public static class IoC
    {
        public static IServiceRegistry Configure(IServiceRegistry container = null)
        {
            container ??= new ServiceContainer();

            container.Register<IUserService, UserService>();

            return container;
        }
    }
}
